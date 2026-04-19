using UnityEngine;
using PuzzleApp.Features.GameCatalog;
using PuzzleApp.Features.MatchingPair;
using PuzzleApp.Features.MatchObjects;

namespace PuzzleApp.App.Bootstrap
{
    [CreateAssetMenu(fileName = "AppConfig", menuName = "PuzzleApp/App Config")]
    public class AppConfig : ScriptableObject
    {
        public GameCatalogConfig gameCatalog;
        public MatchingPairCatalogConfig matchingPairCatalog;
        public MatchObjectsLevelConfig matchObjectsLevels;
    }
}
