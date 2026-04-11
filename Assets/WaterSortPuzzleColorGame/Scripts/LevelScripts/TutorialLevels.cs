using DG.Tweening;
using System.Collections.Generic;
using WaterSortPuzzleGame.Tutorial;
using UnityEngine;
using WaterSortPuzzleGame.Managers;


namespace WaterSortPuzzleGame.LevelScripts
{
    public class TutorialLevels : MonoBehaviour
    {
        [SerializeField] private LevelManager levelManager;

        [SerializeField] private List<GameObject> tutorialLevels = new List<GameObject>();

        [SerializeField] private GameObject handIcon;
        [SerializeField] private Sprite hand;

        private void OnEnable()
        {
            EventManager.TutorialAnimation += TutorialAnimation;
            EventManager.BackToHomeScreen += BackToHomeScreen;
        }

        private void OnDisable()
        {
            EventManager.TutorialAnimation -= TutorialAnimation;
            EventManager.BackToHomeScreen -= BackToHomeScreen;
        }
        private Tween _tween;

        private void BackToHomeScreen()
        {
            _tween?.Kill();
            if (handIcon.activeSelf)
            {
                handIcon.SetActive(false);
            }
        }
        private void TutorialAnimation()
        {
            if (GameManager.TotalCompletedLevelIndex > (GameManager.LevelIndex - 1))
            {
                BackToHomeScreen();
                return;
            }


            if (GameManager.LevelIndex == 0)
            {
                GameObject LevelParent = GameObject.Find("LevelParent");

                if (LevelParent != null)
                {
                    GameObject firstBottle = LevelParent.transform.GetChild(0).gameObject;
                    GameObject secondBottle = LevelParent.transform.GetChild(1).gameObject;
                    firstBottle.AddComponent<ColliderController>();
                    firstBottle.AddComponent<ClickController>();
                    secondBottle.AddComponent<ColliderController>();
                    secondBottle.AddComponent<ClickController>();
                    secondBottle.GetComponent<ClickController>().CallCloseCollider();


                    Vector3 firstBottlePosition = firstBottle.transform.localPosition;
                    Vector3 secondBottlePosition = secondBottle.transform.localPosition;

                    handIcon.GetComponent<HandPositionController>().transferPosition = new Vector3(secondBottlePosition.x, secondBottlePosition.y - 4f, secondBottlePosition.z); ;
                    handIcon.GetComponent<HandPositionController>().leftBottleClicker = firstBottle.GetComponent<ClickController>();
                    handIcon.GetComponent<HandPositionController>().rightBottleClicker = secondBottle.GetComponent<ClickController>();

                    handIcon.transform.localPosition = new Vector3(firstBottlePosition.x, firstBottlePosition.y - 4f, firstBottlePosition.z);
                    _tween?.Kill();
                    _tween = handIcon.transform.DOMoveY(firstBottlePosition.y - 3.5f, 1f).SetEase(Ease.Unset).SetLoops(-1, LoopType.Yoyo);

                    handIcon.SetActive(true);
                }
            }
            else
            {
                BackToHomeScreen();
            }
        }
    }
}