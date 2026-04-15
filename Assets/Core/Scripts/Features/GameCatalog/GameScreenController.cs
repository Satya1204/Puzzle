using System;
using System.Collections.Generic;
using UnityEngine;
using PuzzleApp.App.Signals;
using PuzzleApp.App.Subsystems;
using PuzzleApp.UI;

namespace PuzzleApp.Features.GameCatalog
{
    public sealed class GameScreenController : MonoBehaviour
    {
        [SerializeField] GameObject _gameCatalogPrefab;
        [SerializeField] Transform _gameCatalogParent;
        [SerializeField] Transform _gameControllersParent;

        readonly Dictionary<int, GameDefinition> _definitionsById = new();
        readonly Dictionary<int, IGameController> _gameControllers = new();
        readonly Dictionary<int, GameObject> _gameInstancesById = new();

        GameObject _gameCatalogInstance;
        GameScreenCardsView _cardsView;
        ILobbySubsystem _screenVisibilitySubsystem;
        ISignalBus _signalBus;
        IDisposable _gameClosedSub;
        int _activeGameId = -1;
        bool _isInitialized;

        public GameObject ScreenRoot => gameObject;

        public IReadOnlyList<GameDefinition> GetGameDefinitions() =>
            ResolveCardsViewSource() != null ? ResolveCardsViewSource().GetGameDefinitions() : Array.Empty<GameDefinition>();

        public void Initialize(
            IGameCatalogSubsystem gameCatalogSubsystem,
            ILobbySubsystem screenVisibilitySubsystem,
            ISignalBus signalBus)
        {
            if (_isInitialized)
                return;

            EnsureGameCatalogInstantiated();
            if (_cardsView == null)
            {
                Debug.LogError("GameScreenController: catalog prefab must include GameScreenCardsView.");
                return;
            }

            if (screenVisibilitySubsystem == null || signalBus == null || gameCatalogSubsystem == null)
            {
                Debug.LogError("GameScreenController: missing startup dependencies.");
                return;
            }

            CacheDefinitions(gameCatalogSubsystem.GetDefinitions());
            _screenVisibilitySubsystem = screenVisibilitySubsystem;
            _signalBus = signalBus;
            _gameClosedSub = _signalBus.Subscribe<LobbyClosedSignal>(OnGameClosed);
            _cardsView.SetGames(gameCatalogSubsystem.GetGames());
            _cardsView.CardClicked += OnCardClicked;
            _isInitialized = true;
        }

        void CacheDefinitions(IReadOnlyList<GameDefinition> definitions)
        {
            _definitionsById.Clear();

            for (int i = 0; i < definitions.Count; i++)
            {
                var definition = definitions[i];
                if (definition == null)
                    continue;

                _definitionsById[definition.gameId] = definition;
            }
        }

        void EnsureGameCatalogInstantiated()
        {
            if (_gameCatalogInstance != null)
                return;

            if (_gameCatalogPrefab == null)
            {
                Debug.LogError("GameScreenController: assign game catalog prefab in the inspector.");
                return;
            }

            var parent = _gameCatalogParent != null ? _gameCatalogParent : transform;
            _gameCatalogInstance = Instantiate(_gameCatalogPrefab, parent);
            _cardsView = _gameCatalogInstance.GetComponentInChildren<GameScreenCardsView>(true);
        }

        GameScreenCardsView ResolveCardsViewSource()
        {
            if (_cardsView != null)
                return _cardsView;

            if (_gameCatalogPrefab == null)
                return null;

            return _gameCatalogPrefab.GetComponentInChildren<GameScreenCardsView>(true);
        }

        bool EnsureGameInstantiated(int gameId)
        {
            if (_gameControllers.ContainsKey(gameId))
                return true;

            if (!_definitionsById.TryGetValue(gameId, out var definition))
            {
                Debug.LogWarning($"GameScreenController: no game definition found for game id {gameId}.");
                return false;
            }

            var gameRootPrefab = definition.lobbyPrefab;
            if (gameRootPrefab == null)
            {
                Debug.LogWarning($"GameScreenController: no game prefab assigned for '{definition.title}' (id {gameId}).");
                return false;
            }

            var parent = _gameControllersParent != null
                ? _gameControllersParent
                : (_gameCatalogInstance != null ? _gameCatalogInstance.transform : transform);

            var instance = Instantiate(gameRootPrefab, parent);

            var controller = instance.GetComponent<IGameController>();
            if (controller == null)
                controller = instance.GetComponentInChildren<IGameController>(true);

            if (controller == null)
            {
                var view = instance.GetComponent<GameLobbyView>();
                if (view == null)
                    view = instance.GetComponentInChildren<GameLobbyView>(true);

                if (view != null)
                {
                    var bridge = instance.AddComponent<GameLobbyBridgeController>();
                    bridge.Initialize(view);
                    controller = bridge;
                    Debug.LogWarning(
                        $"GameScreenController: '{definition.title}' prefab has no IGameController; using GameLobbyBridgeController fallback. " +
                        "Assign an explicit XGameController prefab in GameDefinition for the intended architecture.");
                }
                else
                {
                    Debug.LogWarning(
                        $"GameScreenController: game prefab for '{definition.title}' has neither IGameController nor GameLobbyView.");
                    Destroy(instance);
                    return false;
                }
            }

            controller.CloseRequested += OnGameCloseRequested;
            _screenVisibilitySubsystem.RegisterLobby(definition.gameId, instance);
            _gameControllers[definition.gameId] = controller;
            _gameInstancesById[definition.gameId] = instance;
            return true;
        }

        void OnDestroy()
        {
            if (_cardsView != null)
                _cardsView.CardClicked -= OnCardClicked;

            _gameClosedSub?.Dispose();
            _gameClosedSub = null;

            foreach (var controller in _gameControllers.Values)
                controller.CloseRequested -= OnGameCloseRequested;

            foreach (var instance in _gameInstancesById.Values)
            {
                if (instance != null)
                    Destroy(instance);
            }

            _definitionsById.Clear();
            _gameControllers.Clear();
            _gameInstancesById.Clear();
            _screenVisibilitySubsystem = null;
            _signalBus = null;
            _activeGameId = -1;
            _isInitialized = false;
        }

        void OnCardClicked(int gameId)
        {
            if (_signalBus == null)
                return;

            if (EnsureGameInstantiated(gameId))
            {
                if (_gameCatalogParent != null)
                    _gameCatalogParent.gameObject.SetActive(false);

                _activeGameId = gameId;
                _signalBus.Publish(new GameCardSelectedSignal(gameId));
            }
        }

        void OnGameCloseRequested() => _signalBus?.Publish(new LobbyClosedSignal());

        void OnGameClosed(LobbyClosedSignal signal)
        {
            DestroyActiveGameController();

            if (_gameCatalogParent != null)
                _gameCatalogParent.gameObject.SetActive(true);
        }

        void DestroyActiveGameController()
        {
            if (_activeGameId < 0)
                return;

            if (_gameControllers.TryGetValue(_activeGameId, out var controller))
            {
                controller.CloseRequested -= OnGameCloseRequested;
                _gameControllers.Remove(_activeGameId);
            }

            if (_gameInstancesById.TryGetValue(_activeGameId, out var instance))
            {
                _gameInstancesById.Remove(_activeGameId);
                if (instance != null)
                    Destroy(instance);
            }

            _activeGameId = -1;
        }
    }
}
