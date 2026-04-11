using System.Collections.Generic;
using UnityEngine;

namespace WaterSortPuzzleGame.BottleCodes
{
    [System.Serializable]
    public class BottleData : MonoBehaviour
    {
        public int BottleIndex;
        public int[] BottleColorsIndex  = new int[4];
        public int NumberOfTopColorLayers = 1;
        [Range(0, 4)] [SerializeField] public int NumberOfColorsInBottle = 4;
        
        public List<BottleController> ActionBottles = new List<BottleController>();

        public bool BottleSorted;
        public int PreviousTopColorIndex;
        public int TopColorIndex;
        public void DecreaseNumberOfColorsInBottle(int decreaseAmount)
        {
            NumberOfColorsInBottle -= decreaseAmount;
        }
        
        public void IncreaseNumberOfColorsInBottle(int increaseAmount)
        {
            NumberOfColorsInBottle += increaseAmount;
        }

        public void UpdatePreviousTopColor()
        {
            PreviousTopColorIndex = TopColorIndex;
        }
    }
}
