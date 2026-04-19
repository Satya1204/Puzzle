using PuzzleApp.App.DI;
using PuzzleApp.App.Modules;
using PuzzleApp.App.Services;

namespace PuzzleApp.Features.MatchObjects
{
    public sealed class MatchObjectsModule : IAppModule
    {
        public void Register(IServiceRegistry services)
        {
            services.RegisterSingleton<IMatchObjectsDataService>(r =>
                new MatchObjectsDataService(r.Resolve<IGameDataProvider>()));

            services.RegisterSingleton<IMatchObjectsLevelService>(r =>
                new MatchObjectsLevelService(r.Resolve<MatchObjectsLevelConfig>()));

            services.RegisterSingleton<IMatchObjectsProgress>(r => new MatchObjectsProgress());
        }

        public void Initialize(IServiceRegistry services)
        {
            services.Resolve<IMatchObjectsDataService>();
            services.Resolve<IMatchObjectsLevelService>();
            services.Resolve<IMatchObjectsProgress>();
        }
    }
}
