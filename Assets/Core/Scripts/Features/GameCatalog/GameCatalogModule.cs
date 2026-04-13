using PuzzleApp.App.DI;
using PuzzleApp.App.Modules;
using PuzzleApp.App.Signals;
using PuzzleApp.App.Subsystems;
using PuzzleApp.UI;

namespace PuzzleApp.Features.GameCatalog
{
    public sealed class GameCatalogModule : IAppModule
    {
        readonly GameScreenCardsView _gameScreenView;

        public GameCatalogModule(GameScreenCardsView gameScreenView)
        {
            _gameScreenView = gameScreenView;
        }

        public void Register(IServiceRegistry services)
        {
            services.RegisterSingleton<IGameCatalogSubsystem>(_ => new GameCatalogSubsystem());
            services.RegisterSingleton<GameScreenController>(registry =>
                new GameScreenController(
                    _gameScreenView,
                    registry.Resolve<IGameCatalogSubsystem>(),
                    registry.Resolve<ISignalBus>()));
        }

        public void Initialize(IServiceRegistry services)
        {
            services.Resolve<IHudScreenSubsystem>().RegisterScreen(MainTab.Game, _gameScreenView.gameObject);
            services.Resolve<GameScreenController>();
            services.Resolve<INavigationSubsystem>().PublishCurrent();
        }
    }
}
