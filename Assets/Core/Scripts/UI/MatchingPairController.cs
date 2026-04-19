using System;
using UnityEngine;
using PuzzleApp.App.Bootstrap;
using PuzzleApp.Features.GameCatalog;
using PuzzleApp.Features.MatchingPair;
using PuzzleApp.MatchingPair;

namespace PuzzleApp.UI
{
    public class MatchingPairController : MonoBehaviour, IGameController
    {
        [SerializeField] MatchingPairLobbyView _lobbyView;
        [SerializeField] Transform _gameParent;

        public event Action CloseRequested;

        IMatchingPairCatalog _catalog;
        GameObject _activeGameInstance;
        MatchingPairBoardView _activeBoard;

        void Awake()
        {
            Debug.Log($"[MatchingPair] Controller.Awake on '{name}'. lobbyView={(_lobbyView != null ? _lobbyView.name : "NULL")} services={(AppBootstrap.Services != null ? "OK" : "NULL")}");

            if (_lobbyView == null)
            {
                Debug.LogError("MatchingPairController: assign the MatchingPairLobbyView in the inspector.");
                return;
            }

            _catalog = AppBootstrap.Services?.Resolve<IMatchingPairCatalog>();
            if (_catalog == null)
            {
                Debug.LogError("MatchingPairController: IMatchingPairCatalog not available. Ensure MatchingPairModule is registered.");
                return;
            }

            var variants = _catalog.GetVariants();
            Debug.Log($"[MatchingPair] Controller.Awake -> catalog resolved, variants.Count={(variants != null ? variants.Count : -1)}. Calling SetVariants.");
            _lobbyView.SetVariants(variants);
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

            UnsubscribeBoard();
        }

        void OnCardClicked(int pieceCount)
        {
            Debug.Log($"[MatchingPair] Controller.OnCardClicked pieceCount={pieceCount}");

            if (_catalog == null || !_catalog.TryGetVariant(pieceCount, out var definition))
            {
                Debug.LogWarning($"[MatchingPair] OnCardClicked: no definition found for {pieceCount} pieces.");
                return;
            }

            if (definition.gamePrefab == null)
            {
                Debug.LogWarning($"[MatchingPair] OnCardClicked: no game prefab assigned for '{definition.title}'.");
                return;
            }

            DestroyActiveGame();

            var parent = _gameParent != null ? _gameParent : transform;
            Debug.Log($"[MatchingPair] OnCardClicked: spawning gamePrefab='{definition.gamePrefab.name}' under parent='{parent.name}' (gameParent {(_gameParent != null ? "SET" : "NULL, using controller transform")})");
            _activeGameInstance = Instantiate(definition.gamePrefab, parent);
            Debug.Log($"[MatchingPair] OnCardClicked: spawned '{_activeGameInstance.name}', active={_activeGameInstance.activeInHierarchy} childCount={_activeGameInstance.transform.childCount}");

            _activeBoard = _activeGameInstance.GetComponentInChildren<MatchingPairBoardView>(true);
            if (_activeBoard != null)
            {
                Debug.Log($"[MatchingPair] OnCardClicked: board found on '{_activeBoard.name}', calling StartGame.");
                _activeBoard.GameWon += OnGameWon;
                _activeBoard.StartGame();
            }
            else
            {
                Debug.LogError($"[MatchingPair] OnCardClicked: spawned prefab '{definition.gamePrefab.name}' has NO MatchingPairBoardView component in its hierarchy.");
            }

            _lobbyView.SetScrollViewActive(false);
        }

        void OnBackClicked()
        {
            if (_activeGameInstance != null)
            {
                DestroyActiveGame();
                _lobbyView.SetScrollViewActive(true);
                return;
            }

            CloseRequested?.Invoke();
        }

        void OnGameWon()
        {
            Debug.Log("MatchingPairController: game won!");
        }

        void DestroyActiveGame()
        {
            UnsubscribeBoard();

            if (_activeGameInstance != null)
            {
                Destroy(_activeGameInstance);
                _activeGameInstance = null;
            }
        }

        void UnsubscribeBoard()
        {
            if (_activeBoard != null)
            {
                _activeBoard.GameWon -= OnGameWon;
                _activeBoard = null;
            }
        }
    }
}
