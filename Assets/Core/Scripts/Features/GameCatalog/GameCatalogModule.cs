using PuzzleApp.App.DI;
using PuzzleApp.App.Modules;
using PuzzleApp.App.Signals;
using PuzzleApp.App.Subsystems;
using PuzzleApp.UI;

namespace PuzzleApp.Features.GameCatalog
{
    public sealed class GameCatalogModule : IAppModule
    {
        readonly GameScreenController _gameScreenController;

        public GameCatalogModule(GameScreenController gameScreenController)
        {
            _gameScreenController = gameScreenController;
        }

        public void Register(IServiceRegistry services)
        {
            services.RegisterSingleton<IGameCatalogSubsystem>(r =>
                new GameCatalogSubsystem(r.Resolve<GameCatalogConfig>().games));
        }

        public void Initialize(IServiceRegistry services)
        {
            services.Resolve<IHudScreenSubsystem>().RegisterScreen(MainTab.Game, _gameScreenController.ScreenRoot);
            _gameScreenController.Initialize(
                services.Resolve<IGameCatalogSubsystem>(),
                services.Resolve<ILobbySubsystem>(),
                services.Resolve<ISignalBus>());
            services.Resolve<INavigationSubsystem>().PublishCurrent();
        }
    }
}
