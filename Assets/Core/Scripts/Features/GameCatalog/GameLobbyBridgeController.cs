using System;
using UnityEngine;
using PuzzleApp.UI;

namespace PuzzleApp.Features.GameCatalog
{
    /// <summary>
    /// Runtime fallback bridge when a game root has <see cref="GameLobbyView"/> but no explicit
    /// <see cref="IGameController"/> component yet.
    /// </summary>
    public sealed class GameLobbyBridgeController : MonoBehaviour, IGameController
    {
        public event Action CloseRequested;

        GameLobbyView _view;

        public void Initialize(GameLobbyView view)
        {
            _view = view;
            if (_view != null)
                _view.BackClicked += OnBackClicked;
        }

        void OnDestroy()
        {
            if (_view != null)
                _view.BackClicked -= OnBackClicked;
        }

        void OnBackClicked() => CloseRequested?.Invoke();
    }
}
