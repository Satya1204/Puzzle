using UnityEngine;

namespace PuzzleApp.Features.GameCatalog
{
    [CreateAssetMenu(fileName = "GameDefinition", menuName = "PuzzleApp/Game Definition")]
    public class GameDefinition : ScriptableObject
    {
        public int gameId;
        public string title;
        public Sprite icon;
        public GameObject catalogCardPrefab;
        public GameObject lobbyPrefab;
    }
}
