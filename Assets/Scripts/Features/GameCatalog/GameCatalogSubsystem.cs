using System.Collections.Generic;

namespace PuzzleApp.Features.GameCatalog
{
    public interface IGameCatalogSubsystem
    {
        IReadOnlyList<GameCardViewModel> GetGames();
    }

    public sealed class GameCatalogSubsystem : IGameCatalogSubsystem
    {
        static readonly IReadOnlyList<GameCardViewModel> Games = new[]
        {
            new GameCardViewModel(1, "Memory Match"),
            new GameCardViewModel(2, "Water Sort"),
            new GameCardViewModel(3, "Jigsaw Puzzle"),
            new GameCardViewModel(4, "Find It"),
        };

        public IReadOnlyList<GameCardViewModel> GetGames() => Games;
    }
}
