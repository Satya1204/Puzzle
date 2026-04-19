using UnityEngine;

namespace PuzzleApp.Features.MatchObjects
{
    [CreateAssetMenu(fileName = "MatchObjectsLevelConfig", menuName = "PuzzleApp/Match Objects Level Config")]
    public class MatchObjectsLevelConfig : ScriptableObject
    {
        [Min(1)] public int totalLevels = 80;
        [Min(1)] public int pairsPerLevel = 5;
        public MatchObjectsItemPair[] pairPool;
    }
}
