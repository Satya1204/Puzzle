using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WaterSortPuzzleGame.DataClass;
using WaterSortPuzzleGame.Enum;

namespace WaterSortPuzzleGame
{
    public class BoosterUIController : MonoBehaviour
    {
        [SerializeField] RectTransform anim;
        [SerializeField] Transform containerTransform;

        [SerializeField] GameObject itemPrefab;
        [SerializeField] Button restartButton;

        private BoosterUIBehavior[] uiBehaviors;
      
        public void Init()
        {
            restartButton.onClick.AddListener(OnRestartButton);
        }
        private void Update()
        {
            foreach (BoosterUIBehavior uiBehavior in uiBehaviors)
            {
                if (uiBehavior.Behavior.IsDirty)
                {
                    uiBehavior.Redraw();
                }
            }
        }
        public void Initialise()
        {
            // Create UI elements
            BoosterBehavior[] activeBoosters = BoosterController.ActiveBoosters;
            uiBehaviors = new BoosterUIBehavior[activeBoosters.Length];

            for (int i = 0; i < activeBoosters.Length; i++)
            {
                GameObject itemObject = Instantiate(itemPrefab, containerTransform);

                uiBehaviors[i] = itemObject.GetComponent<BoosterUIBehavior>();
                uiBehaviors[i].Initialise(activeBoosters[i]);
            }
            restartButton.transform.SetAsFirstSibling();
        }
        public void Show()
        {
            anim.anchoredPosition = new Vector2(0, 0f);
            anim.DOAnchorPosY(260f, 0.3f).SetDelay(0.2f).SetEase(Ease.OutBack);
        }
        public void Hide()
        {
            anim.DOAnchorPosY(0f, 0.3f).SetEase(Ease.InBack);
        }
        
        public void RedrawPanels()
        {
            for (int i = 0; i < uiBehaviors.Length; i++)
            {
                uiBehaviors[i].Redraw();
            }
        }
        
        public void OnLevelStarted()
        {
            for (int i = 0; i < uiBehaviors.Length; i++)
            {
                if (uiBehaviors[i].Behavior.IsActive())
                {
                    uiBehaviors[i].Activate();
                }
                else
                {
                    uiBehaviors[i].Disable();
                }
            }
        }
        public void UnlockBoosters()
        {
            List<BoosterSettings> unlockedBoosters = new List<BoosterSettings>();

            for (int i = 0; i < uiBehaviors.Length; i++)
            {
                if (uiBehaviors[i].Behavior.IsActive())
                {
                    BoosterSettings settings = uiBehaviors[i].Settings;
                    if (settings.UnlockLevel != 0 && !settings.IsUnlocked && settings.UnlockLevel <= (GameManager.LevelIndex + 1))
                    {
                        unlockedBoosters.Add(settings);
                    }
                }

            }
            if (unlockedBoosters != null && unlockedBoosters.Count > 0)
            {
                UnlockedBoosterPanel gameUI = UIController.GetPage<UnlockedBoosterPanel>();
                gameUI.SetData(unlockedBoosters);

                UIController.ShowPage<UnlockedBoosterPanel>();
            }
        }
        public void OnRestartButton()
        {

            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
            if (!IAPManager.IsNoAdsPurchased && !GameManager.IsInternetConnection())
            {
                SuccessErrorPanel gameUI = UIController.GetPage<SuccessErrorPanel>();
                gameUI.SetData(ToasterState.InternetError);

                UIController.ShowPage<SuccessErrorPanel>();
            }
            else
            {
                AdsManager.Instance.ShowInterstitialAd();
                EventManager.RestartLevel?.Invoke();
            }
        }
        public bool HasUnlockableBoosters()
        {
            for (int i = 0; i < uiBehaviors.Length; i++)
            {
                var behavior = uiBehaviors[i];
                if (behavior.Behavior.IsActive())
                {
                    var settings = behavior.Settings;
                    if (settings.UnlockLevel != 0 &&
                        !settings.IsUnlocked &&
                        settings.UnlockLevel == (GameManager.LevelIndex + 1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}