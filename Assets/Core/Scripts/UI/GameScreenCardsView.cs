using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PuzzleApp.Features.GameCatalog;

namespace PuzzleApp.UI
{
    /// <summary>
    /// Pure view for the game catalog grid.
    /// Game list and per-game catalog prefabs are configured here in the inspector.
    /// Controllers call <see cref="SetGames"/> with view models built from the same definitions.
    /// </summary>
    public class GameScreenCardsView : MonoBehaviour
    {
        [SerializeField] RectTransform _scrollContent;
        [SerializeField] GameDefinition[] _gameDefinitions;
        [SerializeField] [Min(1)] int _columnCount = 2;

        public event Action<int> CardClicked;

        /// <summary>Definitions edited on this view; shared with catalog subsystem and lobby module.</summary>
        public IReadOnlyList<GameDefinition> GetGameDefinitions() =>
            _gameDefinitions != null ? _gameDefinitions : Array.Empty<GameDefinition>();

        void Awake() => ApplyGridConstraint();

        void OnValidate() => ApplyGridConstraint();

        void ApplyGridConstraint()
        {
            if (_scrollContent == null)
                return;

            var grid = _scrollContent.GetComponent<GridLayoutGroup>();
            if (grid == null)
                return;

            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = Mathf.Max(1, _columnCount);
        }

        public void SetGames(IReadOnlyList<GameCardViewModel> games)
        {
            Clear();
            if (games == null || _scrollContent == null)
                return;

            for (int i = 0; i < games.Count; i++)
                SpawnOne(games[i]);
        }

        public void Clear()
        {
            if (_scrollContent == null)
                return;

            for (int i = _scrollContent.childCount - 1; i >= 0; i--)
            {
                var tr = _scrollContent.GetChild(i);
                if (tr.TryGetComponent<GameCardItem>(out var item))
                    item.Clicked -= OnItemClicked;

                var child = tr.gameObject;
                if (Application.isPlaying)
                    Destroy(child);
                else
                    DestroyImmediate(child);
            }
        }

        void SpawnOne(GameCardViewModel viewModel)
        {
            if (viewModel.CatalogCardPrefab == null)
            {
                Debug.LogWarning($"GameScreenCardsView: no catalogCardPrefab for game id {viewModel.Id} ('{viewModel.Title}').");
                return;
            }

            var go = Instantiate(viewModel.CatalogCardPrefab, _scrollContent);
            if (!go.TryGetComponent<GameCardItem>(out var item))
                return;

            item.Bind(viewModel);
            item.Clicked += OnItemClicked;
        }

        void OnItemClicked(int gameId) => CardClicked?.Invoke(gameId);
    }
}
