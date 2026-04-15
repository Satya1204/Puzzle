using System;
using System.Collections.Generic;
using UnityEngine;
using PuzzleApp.Features.MatchingPair;

namespace PuzzleApp.UI
{
    /// <summary>
    /// Pure view for the Matching Pair lobby card grid.
    /// Holds <see cref="MatchingPairDefinition"/> entries configured in the inspector,
    /// spawns a card for each one, and forwards click events.
    /// Also discovers any pre-existing <see cref="MatchingPairCardItem"/> children
    /// already placed in the prefab.
    /// Game instantiation is handled by <see cref="MatchingPairController"/>.
    /// </summary>
    public class MatchingPairLobbyView : GameLobbyView
    {
        [SerializeField] RectTransform _scrollContent;
        [SerializeField] GameObject _scrollView;
        [SerializeField] MatchingPairDefinition[] _definitions;

        public event Action<int> CardClicked;

        readonly List<MatchingPairCardItem> _cardItems = new();

        public IReadOnlyList<MatchingPairDefinition> GetDefinitions() =>
            _definitions != null ? _definitions : Array.Empty<MatchingPairDefinition>();

        public void SetScrollViewActive(bool active)
        {
            if (_scrollView != null)
                _scrollView.SetActive(active);
        }

        void Awake()
        {
            SpawnCards();
            DiscoverAllCards();
        }

        void OnDestroy()
        {
            foreach (var card in _cardItems)
                card.Clicked -= OnItemClicked;
        }

        void SpawnCards()
        {
            if (_definitions == null || _scrollContent == null)
                return;

            for (int i = 0; i < _definitions.Length; i++)
            {
                var def = _definitions[i];
                if (def == null || def.cardPrefab == null)
                    continue;

                var go = Instantiate(def.cardPrefab, _scrollContent);
                if (go.TryGetComponent<MatchingPairCardItem>(out var item))
                    item.Bind(def);
            }
        }

        void DiscoverAllCards()
        {
            var cards = GetComponentsInChildren<MatchingPairCardItem>(true);
            foreach (var card in cards)
            {
                card.Clicked += OnItemClicked;
                _cardItems.Add(card);
            }
        }

        void OnItemClicked(int pieceCount) => CardClicked?.Invoke(pieceCount);
    }
}
