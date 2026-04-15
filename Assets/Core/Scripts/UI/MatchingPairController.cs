using System;
using System.Collections.Generic;
using UnityEngine;
using PuzzleApp.Features.GameCatalog;
using PuzzleApp.Features.MatchingPair;

namespace PuzzleApp.UI
{
    /// <summary>
    /// Game controller for the Matching Pair screen.
    /// <see cref="GameScreenController"/> discovers this via <see cref="IGameController"/>
    /// and subscribes to <see cref="CloseRequested"/> to return to the catalog.
    ///
    /// Holds a reference to the lobby view (card grid) and a game parent transform
    /// where selected game prefabs are instantiated.
    /// </summary>
    public class MatchingPairController : MonoBehaviour, IGameController
    {
        [SerializeField] MatchingPairLobbyView _lobbyView;
        [SerializeField] Transform _gameParent;

        public event Action CloseRequested;

        readonly Dictionary<int, MatchingPairDefinition> _definitionByPieceCount = new();
        GameObject _activeGameInstance;

        void Awake()
        {
            if (_lobbyView == null)
            {
                Debug.LogError("MatchingPairController: assign the MatchingPairLobbyView in the inspector.");
                return;
            }

            var definitions = _lobbyView.GetDefinitions();
            for (int i = 0; i < definitions.Count; i++)
            {
                var def = definitions[i];
                if (def != null)
                    _definitionByPieceCount[def.pieceCount] = def;
            }

            _lobbyView.CardClicked += OnCardClicked;
            _lobbyView.BackClicked += OnBackClicked;
        }

        void OnDestroy()
        {
            if (_lobbyView != null)
            {
                _lobbyView.CardClicked -= OnCardClicked;
                _lobbyView.BackClicked -= OnBackClicked;
            }
        }

        void OnCardClicked(int pieceCount)
        {
            if (!_definitionByPieceCount.TryGetValue(pieceCount, out var definition))
            {
                Debug.LogWarning($"MatchingPairController: no definition found for {pieceCount} pieces.");
                return;
            }

            if (definition.gamePrefab == null)
            {
                Debug.LogWarning($"MatchingPairController: no game prefab assigned for '{definition.title}'.");
                return;
            }

            if (_activeGameInstance != null)
                Destroy(_activeGameInstance);

            var parent = _gameParent != null ? _gameParent : transform;
            _activeGameInstance = Instantiate(definition.gamePrefab, parent);

            _lobbyView.SetScrollViewActive(false);
        }

        void OnBackClicked()
        {
            if (_activeGameInstance != null)
            {
                Destroy(_activeGameInstance);
                _activeGameInstance = null;
                _lobbyView.SetScrollViewActive(true);
                return;
            }

            CloseRequested?.Invoke();
        }
    }
}
