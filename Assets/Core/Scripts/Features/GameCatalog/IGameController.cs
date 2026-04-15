using System;

namespace PuzzleApp.Features.GameCatalog
{
    /// <summary>
    /// Implemented by each game's controller component on its prefab.
    /// <see cref="GameScreenController"/> discovers this at runtime and subscribes to
    /// <see cref="CloseRequested"/> to know when to return to the game catalog.
    /// </summary>
    public interface IGameController
    {
        /// <summary>Raised when the game wants to close and return to the catalog.</summary>
        event Action CloseRequested;
    }
}
