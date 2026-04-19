using System;
using System.Collections.Generic;
using UnityEngine;
using PuzzleApp.Features.MatchingPair;

namespace PuzzleApp.UI
{
    public class MatchingPairLobbyView : GameLobbyView
    {
        [SerializeField] RectTransform _scrollContent;
        [SerializeField] GameObject _scrollView;

        public event Action<int> CardClicked;

        readonly List<MatchingPairCardItem> _cardItems = new();

        public void SetScrollViewActive(bool active)
        {
            if (_scrollView != null)
                _scrollView.SetActive(active);
        }

        public void SetVariants(IReadOnlyList<MatchingPairDefinition> variants)
        {
            Debug.Log($"[MatchingPair] LobbyView.SetVariants on '{name}': variants.Count={(variants != null ? variants.Count : -1)} scrollContent={(_scrollContent != null ? _scrollContent.name : "NULL")}");
            Clear();

            if (variants == null)
            {
                Debug.LogWarning("[MatchingPair] LobbyView.SetVariants: variants is NULL, aborting.");
                return;
            }

            if (_scrollContent == null)
            {
                Debug.LogError("[MatchingPair] LobbyView.SetVariants: _scrollContent is NOT assigned in the inspector. No cards will be spawned.");
                return;
            }

            int spawned = 0;
            for (int i = 0; i < variants.Count; i++)
            {
                var def = variants[i];
                if (def == null)
                {
                    Debug.LogWarning($"[MatchingPair] LobbyView: variants[{i}] is NULL, skipping.");
                    continue;
                }

                if (def.cardPrefab == null)
                {
                    Debug.LogWarning($"[MatchingPair] LobbyView: variants[{i}] '{def.title}' has NULL cardPrefab, skipping.");
                    continue;
                }

                var go = Instantiate(def.cardPrefab, _scrollContent);
                if (!go.TryGetComponent<MatchingPairCardItem>(out var item))
                {
                    Debug.LogWarning($"[MatchingPair] LobbyView: spawned prefab '{def.cardPrefab.name}' for '{def.title}' has no MatchingPairCardItem component. Destroying.");
                    Destroy(go);
                    continue;
                }

                item.Bind(def);
                item.Clicked += OnItemClicked;
                _cardItems.Add(item);
                spawned++;
            }

            Debug.Log($"[MatchingPair] LobbyView.SetVariants complete: spawned {spawned}/{variants.Count} cards under '{_scrollContent.name}'.");
        }

        public void Clear()
        {
            for (int i = 0; i < _cardItems.Count; i++)
            {
                var card = _cardItems[i];
                if (card == null)
                    continue;

                card.Clicked -= OnItemClicked;
                if (Application.isPlaying)
                    Destroy(card.gameObject);
                else
                    DestroyImmediate(card.gameObject);
            }

            _cardItems.Clear();
        }

        void OnDestroy()
        {
            for (int i = 0; i < _cardItems.Count; i++)
            {
                var card = _cardItems[i];
                if (card != null)
                    card.Clicked -= OnItemClicked;
            }

            _cardItems.Clear();
        }

        void OnItemClicked(int pieceCount) => CardClicked?.Invoke(pieceCount);
    }
}
