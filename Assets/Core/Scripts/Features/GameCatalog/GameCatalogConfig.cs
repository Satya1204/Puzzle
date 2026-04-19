using UnityEngine;

namespace PuzzleApp.Features.GameCatalog
{
    [CreateAssetMenu(fileName = "GameCatalogConfig", menuName = "PuzzleApp/Game Catalog Config")]
    public class GameCatalogConfig : ScriptableObject
    {
        public GameDefinition[] games;
    }
}
