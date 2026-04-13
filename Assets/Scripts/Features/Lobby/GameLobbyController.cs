using System;
using PuzzleApp.App.Signals;
using PuzzleApp.UI;

namespace PuzzleApp.Features.Lobby
{
    /// <summary>
    /// Generic lobby controller that works with any <see cref="GameLobbyView"/>.
    /// Back always publishes <see cref="LobbyClosedSignal"/>.
    /// Pass an optional <paramref name="onPlay"/> callback for game-specific launch logic.
    /// </summary>
    public sealed class GameLobbyController : IDisposable
    {
        readonly GameLobbyView _view;
        readonly ISignalBus _signalBus;
        readonly Action _onPlay;

        public GameLobbyController(GameLobbyView view, ISignalBus signalBus, Action onPlay = null)
        {
            _view = view;
            _signalBus = signalBus;
            _onPlay = onPlay;

            if (_view == null)
                return;

            _view.PlayClicked += OnPlay;
            _view.BackClicked += OnBack;
        }

        void OnPlay() => _onPlay?.Invoke();

        void OnBack() => _signalBus.Publish(new LobbyClosedSignal());

        public void Dispose()
        {
            if (_view != null)
            {
                _view.PlayClicked -= OnPlay;
                _view.BackClicked -= OnBack;
            }
        }
    }
}
