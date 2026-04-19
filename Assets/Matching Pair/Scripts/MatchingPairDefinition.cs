using UnityEngine;

namespace PuzzleApp.Features.MatchingPair
{
    [CreateAssetMenu(fileName = "MatchingPairDefinition", menuName = "PuzzleApp/Matching Pair Definition")]
    public class MatchingPairDefinition : ScriptableObject
    {
        public int pieceCount;
        public string title;
        public Sprite icon;
        public GameObject cardPrefab;
        public GameObject gamePrefab;
    }
}
