using UnityEngine;
using PuzzleApp.App.DI;
using PuzzleApp.App.Modules;
using PuzzleApp.App.Signals;
using PuzzleApp.Features.GameCatalog;
using PuzzleApp.Features.Shell;
using PuzzleApp.UI;

namespace PuzzleApp.App.Bootstrap
{
    [DefaultExecutionOrder(-1000)]
    public sealed class AppBootstrap : MonoBehaviour
    {
        [SerializeField] MainTab _initialTab = MainTab.Game;

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

            var bottomBarView = UnityEngine.Object.FindObjectOfType<BottomBarView>(true);
            var gameScreenView = UnityEngine.Object.FindObjectOfType<GameScreenCardsView>(true);

            if (bottomBarView == null || gameScreenView == null)
            {
                Debug.LogError("AppBootstrap could not find required shell views.");
                return;
            }

            _services = new ServiceRegistry();
            _services.RegisterInstance<ISignalBus>(new SignalBus());

            _modules = new IAppModule[]
            {
                new ShellModule(bottomBarView, _initialTab),
                new GameCatalogModule(gameScreenView),
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
