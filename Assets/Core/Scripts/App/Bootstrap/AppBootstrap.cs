using UnityEngine;
using PuzzleApp.App.DI;
using PuzzleApp.App.Modules;
using PuzzleApp.App.Services;
using PuzzleApp.App.Signals;
using PuzzleApp.Features.GameCatalog;
using PuzzleApp.Features.Home;
using PuzzleApp.Features.Lobby;
using PuzzleApp.Features.MatchingPair;
using PuzzleApp.Features.MatchObjects;
using PuzzleApp.Features.Shell;
using PuzzleApp.Features.Shop;
using PuzzleApp.UI;

namespace PuzzleApp.App.Bootstrap
{
    [DefaultExecutionOrder(-1000)]
    public sealed class AppBootstrap : MonoBehaviour
    {
        [Header("Shell")]
        [SerializeField] MainTab _initialTab = MainTab.Game;
        [SerializeField] BottomBarView _bottomBarView;
        [SerializeField] GameScreenController _gameScreenController;
        [SerializeField] HomeScreenController _homeScreenController;
        [SerializeField] ShopScreenController _shopScreenController;

        [Header("Catalog")]
        [SerializeField] AppConfig _appConfig;

        public static IServiceRegistry Services { get; private set; }

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

            if (_bottomBarView == null || _gameScreenController == null)
            {
                Debug.LogError("AppBootstrap: assign required shell views in the inspector.");
                return;
            }

            if (_appConfig == null)
            {
                Debug.LogError("AppBootstrap: assign AppConfig in the inspector.");
                return;
            }

            if (_appConfig.gameCatalog == null || _appConfig.matchingPairCatalog == null || _appConfig.matchObjectsLevels == null)
            {
                Debug.LogError("AppBootstrap: AppConfig is missing one or more catalog references.");
                return;
            }

            _services = new ServiceRegistry();
            _services.RegisterInstance<ISignalBus>(new SignalBus());
            _services.RegisterInstance<IGameDataProvider>(new ResourcesGameDataProvider());
            _services.RegisterInstance<AppConfig>(_appConfig);
            _services.RegisterInstance<GameCatalogConfig>(_appConfig.gameCatalog);
            _services.RegisterInstance<MatchingPairCatalogConfig>(_appConfig.matchingPairCatalog);
            _services.RegisterInstance<MatchObjectsLevelConfig>(_appConfig.matchObjectsLevels);
            Debug.Log($"[MatchingPair] Bootstrap registered MatchingPairCatalogConfig='{_appConfig.matchingPairCatalog.name}' variants.Length={(_appConfig.matchingPairCatalog.variants != null ? _appConfig.matchingPairCatalog.variants.Length : -1)}");

            _modules = new IAppModule[]
            {
                new ShellModule(_bottomBarView, _initialTab),
                new LobbyModule(),
                new HomeScreenModule(_homeScreenController),
                new GameCatalogModule(_gameScreenController),
                new ShopScreenModule(_shopScreenController),
                new MatchingPairModule(),
                new MatchObjectsModule(),
            };

            foreach (var module in _modules)
                module.Register(_services);

            foreach (var module in _modules)
                module.Initialize(_services);

            Services = _services;
        }

        void OnDestroy()
        {
            Services = null;
            _services?.Dispose();
            _services = null;
            _modules = null;
        }
    }
}
