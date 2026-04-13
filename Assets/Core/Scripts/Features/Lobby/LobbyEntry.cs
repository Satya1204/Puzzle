using System;
using PuzzleApp.UI;

namespace PuzzleApp.Features.Lobby
{
    /// <summary>
    /// Inspector-friendly mapping of a game id to its lobby view.
    /// Add entries in the AppBootstrap inspector to register new lobbies — zero code changes.
    /// </summary>
    [Serializable]
    public struct LobbyEntry
    {
        public int gameId;
        public GameLobbyView lobbyView;
    }
}
