using WaterSortPuzzleGame;

namespace WaterSortPuzzleGame
{
    [System.Serializable]
    public class NoAdsReward : Reward
    {
        public override void ApplyReward(int quantity)
        {
            AdsManager.Instance.DisableForcedAd();
        }
    }
}