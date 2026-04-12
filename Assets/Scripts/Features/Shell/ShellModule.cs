using PuzzleApp.App.Controllers;
using PuzzleApp.App.DI;
using PuzzleApp.App.Modules;
using PuzzleApp.App.Signals;
using PuzzleApp.App.Subsystems;
using PuzzleApp.UI;

namespace PuzzleApp.Features.Shell
{
    public sealed class ShellModule : IAppModule
    {
        readonly BottomBarView _bottomBarView;
        readonly MainTab _initialTab;

        public ShellModule(BottomBarView bottomBarView, MainTab initialTab)
        {
            _bottomBarView = bottomBarView;
            _initialTab = initialTab;
        }

        public void Register(IServiceRegistry services)
        {
            services.RegisterSingleton<INavigationSubsystem>(registry =>
                new NavigationSubsystem(registry.Resolve<ISignalBus>(), _initialTab));
            services.RegisterSingleton<IHudScreenSubsystem>(registry =>
                new HudScreenSubsystem(registry.Resolve<ISignalBus>()));
            services.RegisterSingleton<BottomBarController>(registry =>
                new BottomBarController(_bottomBarView, registry.Resolve<ISignalBus>()));
        }

        public void Initialize(IServiceRegistry services)
        {
            services.Resolve<IHudScreenSubsystem>();
            services.Resolve<BottomBarController>();
            services.Resolve<INavigationSubsystem>().PublishCurrent();
        }
    }
}
