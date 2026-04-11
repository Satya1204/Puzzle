using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace WaterSortPuzzleGame.BottleCodes
{
    [System.Serializable]
    public class AllBottles
    {
        public List<Bottle> _allBottles = new List<Bottle>();
        public int LevelIndex = 0;
        public int NumberOfColorInLevel = 0;

        public AllBottles(List<Bottle> tempBottles)
        {
            _allBottles = tempBottles.ToList();
        }
    }
}