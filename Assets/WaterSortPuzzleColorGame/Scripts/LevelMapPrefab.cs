using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WaterSortPuzzleGame
{
    public class LevelMapPrefab : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text LevelNo;

        [SerializeField] private Sprite _lockLevelUIPrefab;
        [SerializeField] private Sprite _unlockLevelUIPrefab;
        [SerializeField] private Sprite _activeLevelUIPrefab;

        [Header("Buttons")]
        [SerializeField] Button buttonClick;

        public void SetData(int level)
        {
            buttonClick.onClick.RemoveAllListeners();
            buttonClick.onClick.AddListener(() => OnClickLevel(LevelNo));

            LevelNo.text = (level + 1).ToString();

            if (GameManager.TotalCompletedLevelIndex < 0)
            {
                if (level == 0)
                {
                    gameObject.GetComponent<Image>().sprite = _activeLevelUIPrefab;
                }
                else
                {
                    LevelNo.gameObject.SetActive(false);
                    gameObject.GetComponent<Image>().sprite = _lockLevelUIPrefab;
                }

            }
            else
            {
                if (level == (GameManager.TotalCompletedLevelIndex + 1))
                {
                    gameObject.GetComponent<Image>().sprite = _activeLevelUIPrefab;
                }
                else if (level <= GameManager.TotalCompletedLevelIndex)
                {
                    gameObject.GetComponent<Image>().sprite = _unlockLevelUIPrefab;
                }
                else
                {
                    LevelNo.gameObject.SetActive(false);
                    gameObject.GetComponent<Image>().sprite = _lockLevelUIPrefab;
                }
            }

        }

        public void OnClickLevel(TMP_Text level)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);

            int LevelIndex = int.Parse(level.text) - 1;
            if (LevelIndex <= (GameManager.TotalCompletedLevelIndex + 1))
            {
                GameManager.LevelIndex = LevelIndex;
                EventManager.CreatePrototypeOrLevel?.Invoke();
                UIController.HidePage<MapPanel>(() =>
                {
                    UIController.ShowPage<LevelPanel>();
                });
                GameManager.IsLevelScreen = true;
            }
        }
    }
}