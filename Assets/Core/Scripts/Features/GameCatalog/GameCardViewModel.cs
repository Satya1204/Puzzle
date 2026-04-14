using UnityEngine;

namespace PuzzleApp.Features.GameCatalog
{
    public readonly struct GameCardViewModel
    {
        public GameCardViewModel(int id, string title, GameObject catalogCardPrefab = null, Sprite icon = null)
        {
            Id = id;
            Title = title;
            CatalogCardPrefab = catalogCardPrefab;
            Icon = icon;
        }

        public int Id { get; }
        public string Title { get; }
        /// <summary>When set, the game grid uses this prefab instead of the view default.</summary>
        public GameObject CatalogCardPrefab { get; }
        public Sprite Icon { get; }
    }
}
