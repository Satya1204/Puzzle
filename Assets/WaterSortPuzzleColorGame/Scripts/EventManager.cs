using System;
using UnityEngine;
using WaterSortPuzzleGame.BottleCodes;

namespace WaterSortPuzzleGame
{
    public static class EventManager
    {
        
        public static Action ChangeScreen;
        public static Action ChangeSoundSetting;
        public static Action ChangeMusicSetting;

        public static Action CheckIsLevelCompleted;
        public static Action CreatePrototypeOrLevel;
        public static Action LevelCompleted;
        public static Action WinEffect;
        public static Action<AllBottles> CreatePrototype;
        public static Action<GameObject> GetLevelParent;
        public static Action LoadNextLevel;
        public static Action CreateNewLevelForJson;
        public static Action RestartLevel;
        public static Action UpdateLevelText;
        public static Action CreateLevel;
        public static Action AddExtraEmptyBottle;
        public static Action TempEmptyTubesChange;
        public static Action IncreaseLevelIndex;
        public static Action TopMenuIconActivation;
        public static Action BackToHomeScreen;
        public static Action TutorialAnimation;
        public static Action LoadMapLevels;

        public static Action<int> AllLevelsGenerate;
    }
}