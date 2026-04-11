using UnityEngine;

namespace WaterSortPuzzleGame
{
    [CreateAssetMenu(fileName = "Booster Database", menuName = "Content/Booster/Database")]
    public class BoosterDatabase : ScriptableObject
    {
        [SerializeField] BoosterSettings[] boosters;
        public BoosterSettings[] Boosters => boosters;
    }
}
