using System;
using UnityEngine;
using PuzzleApp.App.Bootstrap;
using PuzzleApp.Features.MatchObjects;
using PuzzleApp.Features.GameCatalog;
using PuzzleApp.MatchObjects;

namespace PuzzleApp.UI
{
    public class MatchObjectsController : MonoBehaviour, IGameController
    {
        [SerializeField] MatchObjectsLobbyView _lobbyView;
        [SerializeField] Transform _gameParent;
        [SerializeField] Canvas _rootCanvas;
        [SerializeField] MatchObjectsBoardView _boardPrefab;

        IMatchObjectsLevelService _levelService;
        IMatchObjectsProgress _progress;
        MatchObjectsBoardView _activeBoard;
        int _activeLevelIndex = -1;

        public event Action CloseRequested;

        void Awake()
        {
            var services = AppBootstrap.Services;
            _levelService = services?.Resolve<IMatchObjectsLevelService>();
            _progress = services?.Resolve<IMatchObjectsProgress>();

            if (_levelService == null || _progress == null)
            {
                Debug.LogError("MatchObjectsController: level service or progress not available. Ensure MatchObjectsModule is registered.");
                return;
            }

            if (_lobbyView != null)
                _lobbyView.SetLevels(_levelService.TotalLevels, _progress.IsCompleted);
        }

        void OnEnable()
        {
            if (_lobbyView != null)
            {
                _lobbyView.BackClicked += OnBackClicked;
                _lobbyView.LevelSelected += OnLevelSelected;
            }
        }

        void OnDisable()
        {
            if (_lobbyView != null)
            {
                _lobbyView.BackClicked -= OnBackClicked;
                _lobbyView.LevelSelected -= OnLevelSelected;
            }
        }

        void OnBackClicked()
        {
            if (_activeBoard != null)
            {
                DestroyActiveGame();
                _lobbyView.SetScrollViewActive(true);
            }
            else
            {
                CloseRequested?.Invoke();
            }
        }

        void OnLevelSelected(int levelIndex)
        {
            if (_boardPrefab == null)
            {
                Debug.LogError("MatchObjectsController: no board prefab assigned.");
                return;
            }

            if (_levelService == null)
                return;

            var pairs = _levelService.GetPairsForLevel(levelIndex);
            if (pairs == null || pairs.Length == 0)
            {
                Debug.LogError($"MatchObjectsController: failed to build pairs for level {levelIndex + 1}.");
                return;
            }

            _activeLevelIndex = levelIndex;
            _activeBoard = Instantiate(_boardPrefab, _gameParent != null ? _gameParent : transform);
            _lobbyView.SetScrollViewActive(false);

            _activeBoard.GameWon += OnGameWon;
            _activeBoard.StartGame(pairs, _rootCanvas ?? GetComponentInParent<Canvas>());
        }

        void OnGameWon()
        {
            if (_activeLevelIndex >= 0 && _progress != null)
            {
                _progress.MarkCompleted(_activeLevelIndex);
                if (_lobbyView != null)
                    _lobbyView.RefreshCompletion(_activeLevelIndex, true);
            }

            if (_activeBoard != null)
            {
                DestroyActiveGame();
                if (_lobbyView != null)
                    _lobbyView.SetScrollViewActive(true);
            }
        }

        void DestroyActiveGame()
        {
            if (_activeBoard != null)
            {
                _activeBoard.GameWon -= OnGameWon;
                Destroy(_activeBoard.gameObject);
                _activeBoard = null;
            }
            _activeLevelIndex = -1;
        }

        void OnDestroy()
        {
            DestroyActiveGame();
        }
    }
}
