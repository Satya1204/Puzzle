using PuzzleApp.App.DI;
using PuzzleApp.App.Modules;
using PuzzleApp.App.Subsystems;
using PuzzleApp.UI;

namespace PuzzleApp.Features.Home
{
    public sealed class HomeScreenModule : IAppModule
    {
        readonly HomeScreenController _controller;

        public HomeScreenModule(HomeScreenController controller)
        {
            _controller = controller;
        }

        public void Register(IServiceRegistry services) { }

        public void Initialize(IServiceRegistry services)
        {
            services.Resolve<IHudScreenSubsystem>().RegisterScreen(MainTab.Home, _controller.ScreenRoot);
        }
    }
}
