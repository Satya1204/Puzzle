using System.Collections.Generic;
using UnityEngine;

namespace WaterSortPuzzleGame.BottleCodes
{
    [System.Serializable]
    public class Bottle
    {
        public Stack<int> NumberedBottleStack = new Stack<int>();
        public int NumberOfColorsInBottle = 0;
        public int[] BottleColorsIndex = new int[4];

        private int _bottleIndex = -1;

        public Bottle(int bottleIndex)
        {
            _bottleIndex = bottleIndex;
        }
    }
}