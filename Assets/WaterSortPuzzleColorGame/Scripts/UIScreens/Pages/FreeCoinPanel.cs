using WaterSortPuzzleGame.Enum;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WaterSortPuzzleGame
{
    public class FreeCoinPanel : UIPage
    {
        [SerializeField] private ScaleAnimation animPopup;
        [SerializeField] private int freeCoins;
        [SerializeField] private TMP_Text freeCoinMessage;
        [SerializeField] private Button freeCoinBack;
        [SerializeField] private Button freeCoinClose;
        [SerializeField] private RewardedAdUIController getFreeCoins;
        public override void Init()
        {
            freeCoinBack.onClick.AddListener(BackgroundClick);
            freeCoinClose.onClick.AddListener(BackClick);
            getFreeCoins.Init();
            getFreeCoins.onAdRewarded = OnRewardedAdCompleted;

            SetFreeCoinsData();
        }
        public override void PlayShowAnimation()
        {
            animPopup.Show();
        }
        public override void PlayHideAnimation()
        {
            animPopup.Hide();
        }
        private void OnRewardedAdCompleted()
        {
            CoinManager.AddCoins(freeCoins);
            BackHandler.BackEvent();
        }
        private void SetFreeCoinsData()
        {
            freeCoinMessage.text = $"Get {freeCoins} Free Coins";
        }
    }
}