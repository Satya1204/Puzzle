using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WaterSortPuzzleGame.Enum;
using WaterSortPuzzleGame.Navigations;
using TMPro;

namespace WaterSortPuzzleGame
{
    public class HomePanel : UIPage
    {
        public TMP_Text LevelText;

        [Header("Buttons")]
        [SerializeField] Button levelButton;
        [SerializeField] private IAPButton noAdsButton;
        [SerializeField] Button freeCoinButton;

        public void OnEnable()
        {
            LevelText.text = "Level " + (GameManager.LevelIndex + 1).ToString();
            IAPManager.PurchaseCompleted += OnAdPurchased;
            IsAdsPurchased();
        }
        private void OnDisable()
        {
            IAPManager.PurchaseCompleted -= OnAdPurchased;
        }
        private void OnAdPurchased(ProductKeyType productKeyType, int quantity)
        {
            if (productKeyType == ProductKeyType.NoAds)
            {
                IAPManager.IsNoAdsPurchased = true;
                IsAdsPurchased();
            }
        }
        private void IsAdsPurchased()
        {
            if (IAPManager.IsNoAdsPurchased)
            {
                noAdsButton.gameObject.SetActive(false);
            }
            else
            {
                noAdsButton.gameObject.SetActive(true);
            }
        }
        public override void Init()
        {
            levelButton.onClick.AddListener(LevelButtonClick);
            noAdsButton.Init(ProductKeyType.NoAds);
            freeCoinButton.onClick.AddListener(FreeCoinButtonClick);
        }
        public override void PlayShowAnimation()
        {
            AdsManager.Instance.HideBannerAd();
        }
        public override void PlayHideAnimation()
        {
            if(GameManager.IsLevelScreen) AdsManager.Instance.ShowBannerAd();

        }
        public void FreeCoinButtonClick()
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
            UIController.ShowPage<FreeCoinPanel>();
        }
        public void LevelButtonClick()
        {
            GameManager.IsLevelScreen = true;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
            UIController.HidePage<HomePanel>(() =>
            {
                UIController.ShowPage<LevelPanel>();
            });
            EventManager.CreatePrototypeOrLevel?.Invoke();
            //GameManager.IsLevelScreen = true;
        }
        public void NoAdsClick()
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
            if (!GameManager.IsInternetConnection())
            {
                SuccessErrorPanel gameUI = UIController.GetPage<SuccessErrorPanel>();
                gameUI.SetData(ToasterState.InternetError);

                UIController.ShowPage<SuccessErrorPanel>();
            }
        }
        public override void OnPageResume()
        {
            if (NavigationBar.Instance == null) return;

            NavigationBar.Instance.SelectTab(NavigationType.Home);
        }
    }
}