using System;
using System.Collections.Generic;
using UnityEngine;
using PuzzleApp.App.Signals;

namespace PuzzleApp.App.Subsystems
{
    public interface ILobbySubsystem
    {
        bool IsLobbyActive { get; }
        void RegisterLobby(int gameId, GameObject lobbyRoot);
        void ShowLobby(int gameId);
        void HideLobby();
    }

    public sealed class LobbySubsystem : ILobbySubsystem, IDisposable
    {
        readonly Dictionary<int, GameObject> _lobbyRoots = new();
        readonly ISignalBus _signalBus;
        readonly INavigationSubsystem _navigation;
        readonly IDisposable _cardSelectedSub;
        readonly IDisposable _lobbyClosedSub;

        int _activeLobbyId = -1;

        public bool IsLobbyActive => _activeLobbyId >= 0;

        public LobbySubsystem(
            ISignalBus signalBus,
            INavigationSubsystem navigation)
        {
            _signalBus = signalBus;
            _navigation = navigation;

            _cardSelectedSub = signalBus.Subscribe<GameCardSelectedSignal>(OnCardSelected);
            _lobbyClosedSub = signalBus.Subscribe<LobbyClosedSignal>(OnLobbyClosed);
        }

        public void RegisterLobby(int gameId, GameObject lobbyRoot)
        {
            if (lobbyRoot == null)
                return;

            _lobbyRoots[gameId] = lobbyRoot;
            lobbyRoot.SetActive(false);
        }

        public void ShowLobby(int gameId)
        {
            if (!_lobbyRoots.TryGetValue(gameId, out var root))
            {
                Debug.LogWarning($"No lobby registered for gameId {gameId}");
                return;
            }

            HideAllLobbies();
            root.SetActive(true);
            _activeLobbyId = gameId;
        }

        public void HideLobby()
        {
            HideAllLobbies();
            _activeLobbyId = -1;
            _navigation.PublishCurrent();
        }

        void HideAllLobbies()
        {
            foreach (var pair in _lobbyRoots)
            {
                if (pair.Value != null)
                    pair.Value.SetActive(false);
            }
        }

        void OnCardSelected(GameCardSelectedSignal signal) => ShowLobby(signal.GameId);

        void OnLobbyClosed(LobbyClosedSignal signal) => HideLobby();

        public void Dispose()
        {
            _cardSelectedSub?.Dispose();
            _lobbyClosedSub?.Dispose();
        }
    }
}
