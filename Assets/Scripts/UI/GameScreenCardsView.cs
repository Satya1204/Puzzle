using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleApp.UI
{
    /// <summary>
    /// Data for one game tile; extend when you wire a real catalog.
    /// </summary>
    public struct GameCardDescriptor
    {
        public int Id;
        public string Title;

        public GameCardDescriptor(int id, string title = null)
        {
            Id = id;
            Title = title;
        }
    }

    /// <summary>
    /// Lives on the Game Screen prefab: fills the Scroll View Content with Game Card prefabs in a 2-column grid.
    /// Rows = ceil(cardCount / columnCount).
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
            if (_scrollContent == null) return;
            var grid = _scrollContent.GetComponent<GridLayoutGroup>();
            if (grid == null) return;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = Mathf.Max(1, _columnCount);
        }

        /// <summary>Spawn N cards (indices 0..count-1).</summary>
        public void SetCardCount(int count)
        {
            Clear();
            if (_gameCardPrefab == null || _scrollContent == null) return;
            count = Mathf.Max(0, count);
            for (int i = 0; i < count; i++)
                SpawnOne(new GameCardDescriptor(i), i);
        }

        /// <summary>Spawn one card per entry.</summary>
        public void SetGames(IReadOnlyList<GameCardDescriptor> games)
        {
            Clear();
            if (games == null || _gameCardPrefab == null || _scrollContent == null) return;
            for (int i = 0; i < games.Count; i++)
                SpawnOne(games[i], i);
        }

        public void Clear()
        {
            if (_scrollContent == null) return;
            for (int i = _scrollContent.childCount - 1; i >= 0; i--)
            {
                var tr = _scrollContent.GetChild(i);
                if (tr.TryGetComponent<GameCardItem>(out var item))
                    item.Clicked -= OnItemClicked;
                var go = tr.gameObject;
                if (Application.isPlaying) Destroy(go);
                else DestroyImmediate(go);
            }
        }

        void SpawnOne(in GameCardDescriptor data, int index)
        {
            var go = Instantiate(_gameCardPrefab, _scrollContent);
            if (go.TryGetComponent<GameCardItem>(out var item))
            {
                item.Bind(data, index);
                item.Clicked += OnItemClicked;
            }
        }

        void OnItemClicked(int index) => CardClicked?.Invoke(index);
    }
}
