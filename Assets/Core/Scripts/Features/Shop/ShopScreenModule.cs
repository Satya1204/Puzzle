using PuzzleApp.App.DI;
using PuzzleApp.App.Modules;
using PuzzleApp.App.Subsystems;
using PuzzleApp.UI;

namespace PuzzleApp.Features.Shop
{
    public sealed class ShopScreenModule : IAppModule
    {
        readonly ShopScreenController _controller;

        public ShopScreenModule(ShopScreenController controller)
        {
            _controller = controller;
        }

        public void Register(IServiceRegistry services) { }

        public void Initialize(IServiceRegistry services)
        {
            services.Resolve<IHudScreenSubsystem>().RegisterScreen(MainTab.Shop, _controller.ScreenRoot);
        }
    }
}
