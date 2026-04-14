using System;
using PuzzleApp.App.DI;
using PuzzleApp.App.Modules;
using PuzzleApp.App.Signals;
using PuzzleApp.App.Subsystems;

namespace PuzzleApp.Features.Lobby
{
    /// <summary>
    /// Registers the lobby subsystem that shows and hides instantiated lobby roots.
    /// Lobby instances are created by the game screen controller during bootstrap.
    /// </summary>
    public sealed class LobbyModule : IAppModule
    {
        public void Register(IServiceRegistry services)
        {
            services.RegisterSingleton<ILobbySubsystem>(r =>
                new LobbySubsystem(
                    r.Resolve<ISignalBus>(),
                    r.Resolve<INavigationSubsystem>()));
        }

        public void Initialize(IServiceRegistry services)
        {
            services.Resolve<ILobbySubsystem>();
        }
    }
}
