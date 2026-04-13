using UnityEngine;
using PuzzleApp.App.DI;
using PuzzleApp.App.Modules;
using PuzzleApp.App.Signals;
using PuzzleApp.Features.GameCatalog;
using PuzzleApp.Features.Lobby;
using PuzzleApp.Features.Shell;
using PuzzleApp.UI;

namespace PuzzleApp.App.Bootstrap
{
    [DefaultExecutionOrder(-1000)]
    public sealed class AppBootstrap : MonoBehaviour
    {
        [Header("Shell")]
        [SerializeField] MainTab _initialTab = MainTab.Game;
        [SerializeField] BottomBarView _bottomBarView;
        [SerializeField] GameScreenCardsView _gameScreenView;

        [Header("Lobbies")]
        [SerializeField] LobbyEntry[] _lobbyEntries;

        IServiceRegistry _services;
        IAppModule[] _modules;

        void Awake()
        {
            Bootstrap();
        }

        void Bootstrap()
        {
            if (_services != null)
                return;

            if (_bottomBarView == null || _gameScreenView == null)
            {
                Debug.LogError("AppBootstrap: assign required shell views in the inspector.");
                return;
            }

            _services = new ServiceRegistry();
            _services.RegisterInstance<ISignalBus>(new SignalBus());

            _modules = new IAppModule[]
            {
                new ShellModule(_bottomBarView, _initialTab),
                new GameCatalogModule(_gameScreenView),
                new LobbyModule(_lobbyEntries),
            };

            foreach (var module in _modules)
                module.Register(_services);

            foreach (var module in _modules)
                module.Initialize(_services);
        }

        void OnDestroy()
        {
            _services?.Dispose();
            _services = null;
            _modules = null;
        }
    }
}
