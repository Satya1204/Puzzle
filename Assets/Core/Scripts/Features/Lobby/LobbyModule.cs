using System;
using System.Collections.Generic;
using PuzzleApp.App.DI;
using PuzzleApp.App.Modules;
using PuzzleApp.App.Signals;
using PuzzleApp.App.Subsystems;

namespace PuzzleApp.Features.Lobby
{
    /// <summary>
    /// Data-driven lobby module. Receives <see cref="LobbyEntry"/> array from bootstrap;
    /// adding a new game lobby requires zero code changes — just add an inspector entry.
    /// </summary>
    public sealed class LobbyModule : IAppModule
    {
        readonly LobbyEntry[] _entries;

        public LobbyModule(LobbyEntry[] entries)
        {
            _entries = entries ?? Array.Empty<LobbyEntry>();
        }

        public void Register(IServiceRegistry services)
        {
            services.RegisterSingleton<ILobbySubsystem>(r =>
                new LobbySubsystem(
                    r.Resolve<ISignalBus>(),
                    r.Resolve<IHudScreenSubsystem>(),
                    r.Resolve<INavigationSubsystem>()));
        }

        public void Initialize(IServiceRegistry services)
        {
            var lobby = services.Resolve<ILobbySubsystem>();
            var signalBus = services.Resolve<ISignalBus>();

            var controllers = new LobbyControllerGroup();

            foreach (var entry in _entries)
            {
                if (entry.lobbyView == null)
                    continue;

                lobby.RegisterLobby(entry.gameId, entry.lobbyView.gameObject);
                controllers.Add(new GameLobbyController(entry.lobbyView, signalBus));
            }

            services.RegisterInstance(controllers);
        }

        /// <summary>
        /// Holds all <see cref="GameLobbyController"/> instances so they are
        /// disposed when <see cref="IServiceRegistry"/> is disposed.
        /// </summary>
        sealed class LobbyControllerGroup : IDisposable
        {
            readonly List<GameLobbyController> _controllers = new();

            public void Add(GameLobbyController controller) => _controllers.Add(controller);

            public void Dispose()
            {
                foreach (var c in _controllers)
                    c.Dispose();

                _controllers.Clear();
            }
        }
    }
}
