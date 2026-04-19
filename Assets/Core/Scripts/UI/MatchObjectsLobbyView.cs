using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleApp.UI
{
    public class MatchObjectsLobbyView : GameLobbyView
    {
        [SerializeField] RectTransform _scrollContent;
        [SerializeField] GameObject _scrollView;
        [SerializeField] MatchObjectsLevelItem _levelItemPrefab;

        public event Action<int> LevelSelected;

        readonly List<MatchObjectsLevelItem> _items = new();

        public void SetLevels(int totalLevels, Func<int, bool> isCompleted)
        {
            Clear();

            if (_scrollContent == null)
            {
                Debug.LogError("MatchObjectsLobbyView: _scrollContent is not assigned.");
                return;
            }

            if (_levelItemPrefab == null)
            {
                Debug.LogError("MatchObjectsLobbyView: _levelItemPrefab is not assigned.");
                return;
            }

            for (int i = 0; i < totalLevels; i++)
            {
                var item = Instantiate(_levelItemPrefab, _scrollContent);
                item.Bind(i, isCompleted != null && isCompleted(i));
                item.Clicked += OnItemClicked;
                _items.Add(item);
            }
        }

        public void RefreshCompletion(int levelIndex, bool isCompleted)
        {
            if (levelIndex < 0 || levelIndex >= _items.Count)
                return;

            var item = _items[levelIndex];
            if (item != null)
                item.SetCompleted(isCompleted);
        }

        public void SetScrollViewActive(bool active)
        {
            if (_scrollView != null)
                _scrollView.SetActive(active);
        }

        public void Clear()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (item == null)
                    continue;

                item.Clicked -= OnItemClicked;
                if (Application.isPlaying)
                    Destroy(item.gameObject);
                else
                    DestroyImmediate(item.gameObject);
            }

            _items.Clear();
        }

        void OnDestroy() => Clear();

        void OnItemClicked(int levelIndex) => LevelSelected?.Invoke(levelIndex);
    }
}
