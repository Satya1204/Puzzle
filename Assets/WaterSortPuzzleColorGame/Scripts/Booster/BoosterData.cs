using UnityEngine;

namespace WaterSortPuzzleGame
{
    [System.Serializable]
    public class BoosterData
    {
        public BoosterType type;
        public string title;
        public Sprite icon;
        public Sprite inActiveIcon;
        public string description;
        public int coins;
        public int noOfBuy;
        public int noOfClaim;
        public int unlockLevel;
        public string tutorialMessage;
    }
    [System.Serializable]
    public enum BoosterType
    {
        Undo, AddTube
    }
}