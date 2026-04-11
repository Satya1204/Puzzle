using System.Collections.Generic;
using UnityEngine;

namespace WaterSortPuzzleGame.LevelGenerator
{
    [System.Serializable]
    public class Data
    {
        public long BottleColorRandomIndex = 0;
        public long ExtraBottleAmountRandomIndex = 0;
        public long ColorPickerRandomIndex = 0;

        [SerializeField] public List<GenerateBottle> CreatedBottles = new List<GenerateBottle>();

        private int _increaseVal = 50;

        public long GetBottleColorRandomIndex()
        {
            BottleColorRandomIndex += _increaseVal;
            return BottleColorRandomIndex - _increaseVal;
        }
        
        public long GetAmountOfExtraBottleIndex()
        {
            ExtraBottleAmountRandomIndex += _increaseVal;
            return ExtraBottleAmountRandomIndex - _increaseVal;
        }
        
        public long GetColorPickerRandomIndex()
        {
            ColorPickerRandomIndex += _increaseVal;
            return ColorPickerRandomIndex - _increaseVal;
        }
    }
}