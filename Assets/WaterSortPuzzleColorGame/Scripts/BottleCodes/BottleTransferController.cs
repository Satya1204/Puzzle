using UnityEngine;

namespace WaterSortPuzzleGame.BottleCodes
{
    public class BottleTransferController : MonoBehaviour
    {
        public BottleData BottleData { get; private set; }
        public BottleColorController BottleColorController { get; private set; }    
        
        public int NumberOfColorsToTransfer = 0;

        public BottleController BottleControllerRef;
        private void Awake()
        {
            BottleData = GetComponent<BottleData>();
            BottleColorController = GetComponent<BottleColorController>();
        }
        public bool FillBottleCheck(int colorToCheck)
        {
            var numberOfColorsInBottle = BottleData.NumberOfColorsInBottle;

            return numberOfColorsInBottle switch
            {
                0 => true,
                4 => false,
                _ => Equals(BottleData.TopColorIndex,colorToCheck)
            };
        }
    }
}
