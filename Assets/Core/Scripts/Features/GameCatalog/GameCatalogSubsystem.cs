using System;
using System.Collections.Generic;

namespace PuzzleApp.Features.GameCatalog
{
    public interface IGameCatalogSubsystem
    {
        IReadOnlyList<GameCardViewModel> GetGames();
        IReadOnlyList<GameDefinition> GetDefinitions();
    }

    public sealed class GameCatalogSubsystem : IGameCatalogSubsystem
    {
        readonly IReadOnlyList<GameDefinition> _definitions;
        readonly IReadOnlyList<GameCardViewModel> _games;

        public GameCatalogSubsystem(IReadOnlyList<GameDefinition> definitions)
        {
            _definitions = definitions ?? Array.Empty<GameDefinition>();

            var vms = new GameCardViewModel[_definitions.Count];
            for (int i = 0; i < _definitions.Count; i++)
                vms[i] = new GameCardViewModel(
                    _definitions[i].gameId,
                    _definitions[i].title,
                    _definitions[i].catalogCardPrefab,
                    _definitions[i].icon);

            _games = vms;
        }

        public IReadOnlyList<GameCardViewModel> GetGames() => _games;
        public IReadOnlyList<GameDefinition> GetDefinitions() => _definitions;
    }
}
