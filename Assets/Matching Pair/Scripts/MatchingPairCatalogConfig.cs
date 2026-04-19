using UnityEngine;

namespace PuzzleApp.Features.MatchingPair
{
    [CreateAssetMenu(fileName = "MatchingPairCatalogConfig", menuName = "PuzzleApp/Matching Pair Catalog Config")]
    public class MatchingPairCatalogConfig : ScriptableObject
    {
        public MatchingPairDefinition[] variants;
    }
}
