using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace WaterSortPuzzleGame
{
    public class PurchaseBoosterPanel : UIPage
    {
        [SerializeField] private ScaleAnimation animPopup;
        [Header("Main Icon")]
        [SerializeField] private Image icon1;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;

        [Header("Coin Base UI")]
        [SerializeField] private Image icon2;
        [SerializeField] private TMP_Text coins;
        [SerializeField] private TMP_Text coinReward;
        [Header("Ads Base UI")]
        [SerializeField] private Image icon3;
        [SerializeField] private TMP_Text adsReward;

        [Header("Buttons")]
        [SerializeField] private Button onBackButton;
        [SerializeField] private Button onCloseButton;
        [SerializeField] private Button onCoinPurchaseButton;
        [SerializeField] private RewardedAdUIController onAdsPurchaseButton;
        private BoosterSettings settings;
        public override void Init()
        {
            onBackButton.onClick.AddListener(BackgroundClick);
            onCloseButton.onClick.AddListener(BackClick);
            onCoinPurchaseButton.onClick.AddListener(CoinPurchaseButton);
            onAdsPurchaseButton.Init();
            onAdsPurchaseButton.onAdRewarded = OnRewardedAdCompleted;
        }
        public void SetData(BoosterSettings settings)
        {
            this.settings = settings;
            icon1.sprite = settings.Icon;
            icon2.sprite = settings.Icon;
            icon3.sprite = settings.Icon;
            title.text = settings.Title;
            description.text = settings.Description;

            coinReward.text = "x " + settings.CoinReward.ToString();
            adsReward.text = "x " + settings.AdsReward.ToString();

            coins.text = settings.Coins.ToString();

        }
        public void CoinPurchaseButton()
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);

            bool purchaseSuccessful = BoosterController.Instance.PurchaseCoinBaseBooster(settings.Type);

            if (purchaseSuccessful)
                BackHandler.BackEvent();
        }
        private void OnRewardedAdCompleted()
        {
            BoosterController.Instance.PurchaseAdsBaseBooster(settings.Type);
            BackHandler.BackEvent();
        }
        public override void PlayShowAnimation()
        {
            animPopup.Show();
        }
        public override void PlayHideAnimation()
        {
            animPopup.Hide();
        }
    }
}