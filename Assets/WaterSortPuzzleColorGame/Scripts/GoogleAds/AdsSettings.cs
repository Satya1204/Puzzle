using UnityEngine;
namespace WaterSortPuzzleGame
{
    [CreateAssetMenu(fileName = "Ads Setting", menuName = "Content/Ads Setting")]
    public class AdsSettings : ScriptableObject
    {
        //ca-app-pub-3940256099942544~3347511713;
        [Header("Admob Ids")]
        [SerializeField] string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
        public string BannerAdUnitId => bannerAdUnitId;
        [SerializeField] string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
        public string InterstitialAdUnitId => interstitialAdUnitId;

        [SerializeField] string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
        public string RewardedAdUnitId => rewardedAdUnitId;
    }
}