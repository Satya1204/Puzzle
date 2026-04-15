using System;
using UnityEngine;
using PuzzleApp.Features.GameCatalog;

namespace PuzzleApp.UI
{
    /// <summary>
    /// Game controller for the Water Sort screen.
    /// Sits on the WaterSortGameLobby prefab alongside <see cref="WaterSortLobbyView"/>.
    /// <see cref="GameScreenController"/> discovers this via <see cref="IGameController"/>
    /// and subscribes to <see cref="CloseRequested"/> to return to the catalog.
    /// </summary>
    [RequireComponent(typeof(WaterSortLobbyView))]
    public class WaterSortController : MonoBehaviour, IGameController
    {
        public event Action CloseRequested;

        WaterSortLobbyView _view;

        void Awake()
        {
            _view = GetComponent<WaterSortLobbyView>();
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
