using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PuzzleApp.Features.GameCatalog;

namespace PuzzleApp.UI
{
    /// <summary>
    /// Pure view for the game catalog grid.
    /// Controllers provide card data and react to click events.
    /// </summary>
    public class GameScreenCardsView : MonoBehaviour
    {
        [SerializeField] RectTransform _scrollContent;
        [SerializeField] GameObject _gameCardPrefab;
        [SerializeField] [Min(1)] int _columnCount = 2;

        public event Action<int> CardClicked;

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

        public void SetCardCount(int count)
        {
            Clear();
            if (_gameCardPrefab == null || _scrollContent == null)
                return;

            count = Mathf.Max(0, count);

            for (int i = 0; i < count; i++)
                SpawnOne(new GameCardViewModel(i, $"Game {i + 1}"));
        }

        public void SetGames(IReadOnlyList<GameCardViewModel> games)
        {
            Clear();
            if (games == null || _gameCardPrefab == null || _scrollContent == null)
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
            var go = Instantiate(_gameCardPrefab, _scrollContent);
            if (!go.TryGetComponent<GameCardItem>(out var item))
                return;

            item.Bind(viewModel);
            item.Clicked += OnItemClicked;
        }

        void OnItemClicked(int gameId) => CardClicked?.Invoke(gameId);
    }
}
