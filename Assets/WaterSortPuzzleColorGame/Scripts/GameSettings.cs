using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    [CreateAssetMenu(fileName = "Game Settings", menuName = "Content/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Initial Game Data/ First Time Game Load")]
        [SerializeField] int levelIndex = 0;
        public int LevelIndex => levelIndex;
        [SerializeField] int totalCompletedLevelIndex = -1;
        public int TotalCompletedLevelIndex => totalCompletedLevelIndex;

        [SerializeField] int coins;
        public int Coins => coins;

        [SerializeField] bool isMusicEnable;
        public bool IsMusicEnable => isMusicEnable;

        [SerializeField] bool isSoundEnable;
        public bool IsSoundEnable => isSoundEnable;

        [Header("Setting Panel Data")]
        [SerializeField] string appPackageName;
        public string AppPackageName => appPackageName;

        [SerializeField] string developerId;
        public string DeveloperId => developerId;

        [SerializeField] string privacyPolicy;
        public string PrivacyPolicy => privacyPolicy;

    }
}