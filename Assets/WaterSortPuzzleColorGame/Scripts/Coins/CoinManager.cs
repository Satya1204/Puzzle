using System;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    public static class CoinManager
    {
        public static int GoldCoin
        {
            get => PrefManager.GetInt(PlayerPrefNames.GoldCoin, GameManager.Instance.GameSettings.Coins);
            private set
            {
                PrefManager.SetInt(PlayerPrefNames.GoldCoin, value);
                UpdateCoin?.Invoke();
            }
        }

        public static event Action UpdateCoin;

        public static bool IsEnoughCoin(int payCoin)
        {
            return GoldCoin >= payCoin;
        }
        public static void AddCoins(int amount)
        {
            GoldCoin += amount;
        }

        public static void SubtractCoins(int amount)
        {
            GoldCoin -= amount;
        }
    }

}