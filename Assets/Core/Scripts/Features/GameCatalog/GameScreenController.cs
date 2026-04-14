using System;
using System.Collections.Generic;
using UnityEngine;
using PuzzleApp.App.Signals;
using PuzzleApp.App.Subsystems;
using PuzzleApp.Features.Lobby;
using PuzzleApp.UI;

namespace PuzzleApp.Features.GameCatalog
{
    public sealed class GameScreenController : MonoBehaviour
    {
        [SerializeField] GameObject _gameLobbyPrefab;
        [SerializeField] Transform _cardLobbyParent;
        [SerializeField] Transform _gameLobbiesParent;

        readonly Dictionary<int, GameDefinition> _definitionsById = new();
        readonly Dictionary<int, GameLobbyController> _lobbyControllers = new();

        GameObject _gameLobbyInstance;
        GameScreenCardsView _cardsView;
        ILobbySubsystem _lobbySubsystem;
        ISignalBus _signalBus;
        IDisposable _lobbyClosedSub;
        bool _isInitialized;

        public GameObject ScreenRoot => gameObject;

        public IReadOnlyList<GameDefinition> GetGameDefinitions() =>
            ResolveCardsViewSource() != null ? ResolveCardsViewSource().GetGameDefinitions() : Array.Empty<GameDefinition>();

        public void Initialize(
            IGameCatalogSubsystem gameCatalogSubsystem,
            ILobbySubsystem lobbySubsystem,
            ISignalBus signalBus)
        {
            if (_isInitialized)
                return;

            EnsureGameLobbyInstantiated();
            if (_cardsView == null)
            {
                Debug.LogError("GameScreenController: GameLobby prefab must include GameScreenCardsView.");
                return;
            }

            if (lobbySubsystem == null || signalBus == null || gameCatalogSubsystem == null)
            {
                Debug.LogError("GameScreenController: missing startup dependencies.");
                return;
            }

            CacheDefinitions(gameCatalogSubsystem.GetDefinitions());
            _lobbySubsystem = lobbySubsystem;
            _signalBus = signalBus;
            _lobbyClosedSub = _signalBus.Subscribe<LobbyClosedSignal>(OnLobbyClosed);
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

        void EnsureGameLobbyInstantiated()
        {
            if (_gameLobbyInstance != null)
                return;

            if (_gameLobbyPrefab == null)
            {
                Debug.LogError("GameScreenController: assign GameLobby prefab in the inspector.");
                return;
            }

            var parent = _cardLobbyParent != null ? _cardLobbyParent : transform;
            _gameLobbyInstance = Instantiate(_gameLobbyPrefab, parent);
            _cardsView = _gameLobbyInstance.GetComponentInChildren<GameScreenCardsView>(true);
        }

        GameScreenCardsView ResolveCardsViewSource()
        {
            if (_cardsView != null)
                return _cardsView;

            if (_gameLobbyPrefab == null)
                return null;

            return _gameLobbyPrefab.GetComponentInChildren<GameScreenCardsView>(true);
        }

        bool EnsureLobbyInstantiated(int gameId)
        {
            if (_lobbyControllers.ContainsKey(gameId))
                return true;

            if (!_definitionsById.TryGetValue(gameId, out var definition))
            {
                Debug.LogWarning($"GameScreenController: no game definition found for game id {gameId}.");
                return false;
            }

            if (definition.lobbyPrefab == null)
            {
                Debug.LogWarning($"GameScreenController: no lobby prefab assigned for '{definition.title}' (id {gameId}).");
                return false;
            }

            var parent = _gameLobbiesParent != null
                ? _gameLobbiesParent
                : (_gameLobbyInstance != null ? _gameLobbyInstance.transform : transform);

            var instance = Instantiate(definition.lobbyPrefab, parent);
            var view = instance.GetComponent<GameLobbyView>();
            if (view == null)
                view = instance.GetComponentInChildren<GameLobbyView>(true);

            if (view == null)
            {
                Debug.LogWarning($"GameScreenController: lobby prefab for '{definition.title}' has no GameLobbyView.");
                Destroy(instance);
                return false;
            }

            _lobbySubsystem.RegisterLobby(definition.gameId, instance);
            _lobbyControllers[definition.gameId] = new GameLobbyController(view, _signalBus);
            return true;
        }

        void OnDestroy()
        {
            if (_cardsView != null)
                _cardsView.CardClicked -= OnCardClicked;

            _lobbyClosedSub?.Dispose();
            _lobbyClosedSub = null;

            foreach (var controller in _lobbyControllers.Values)
                controller.Dispose();

            _definitionsById.Clear();
            _lobbyControllers.Clear();
            _lobbySubsystem = null;
            _signalBus = null;
            _isInitialized = false;
        }

        void OnCardClicked(int gameId)
        {
            if (_signalBus == null)
                return;

            if (EnsureLobbyInstantiated(gameId))
            {
                if (_cardLobbyParent != null)
                    _cardLobbyParent.gameObject.SetActive(false);

                _signalBus.Publish(new GameCardSelectedSignal(gameId));
            }
        }

        void OnLobbyClosed(LobbyClosedSignal signal)
        {
            if (_cardLobbyParent != null)
                _cardLobbyParent.gameObject.SetActive(true);
        }
    }
}
