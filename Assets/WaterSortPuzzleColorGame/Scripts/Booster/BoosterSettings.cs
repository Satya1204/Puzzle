using UnityEngine;

namespace WaterSortPuzzleGame
{
    public abstract class BoosterSettings : ScriptableObject
    {
        [SerializeField] BoosterType type;
        public BoosterType Type => type;

        [SerializeField] Sprite icon;
        public Sprite Icon => icon;

        [SerializeField] Sprite inActiveIcon;
        public Sprite InActiveIcon => inActiveIcon;

        [SerializeField] GameObject behaviorPrefab;
        public GameObject BehaviorPrefab => behaviorPrefab;

        [SerializeField] string title;
        public string Title => title;

        [SerializeField] string description;
        public string Description => description;

        [SerializeField] int unlockLevel;
        public int UnlockLevel => unlockLevel;

        [Header("Unlock Popup")]
        [SerializeField] int noOfClaim;
        public int NoOfClaim => noOfClaim;

        [Header("Coin Purchase Option")]
        [SerializeField] int coins; // e.g. 50
        public int Coins => coins;
        [SerializeField] int coinReward; // e.g. 3 boosters
        public int CoinReward => coinReward;

        [Header("Ads Purchase Option")]
        [SerializeField] int adsReward;        // e.g. 2 boosters
        public int AdsReward => adsReward;

        public int Save { get => PrefManager.GetInt($"Booster_{type}", 0); set => PrefManager.SetInt($"Booster_{type}", value); }

        public bool IsUnlocked { get => PrefManager.GetBool($"Unlocked_{type}", false); set => PrefManager.SetBool($"Unlocked_{type}", value); }
       
        public abstract void Init();
        public bool HasEnoughCurrency()
        {
            return CoinManager.IsEnoughCoin(coins);
        }

    }
}