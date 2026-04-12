using System;
using PuzzleApp.App.Signals;
using PuzzleApp.UI;

namespace PuzzleApp.Features.GameCatalog
{
    public sealed class GameScreenController : IDisposable
    {
        readonly GameScreenCardsView _view;
        readonly ISignalBus _signalBus;

        public GameScreenController(
            GameScreenCardsView view,
            IGameCatalogSubsystem gameCatalogSubsystem,
            ISignalBus signalBus)
        {
            _view = view;
            _signalBus = signalBus;

            _view.SetGames(gameCatalogSubsystem.GetGames());
            _view.CardClicked += OnCardClicked;
        }

        void OnCardClicked(int gameId)
        {
            _signalBus.Publish(new GameCardSelectedSignal(gameId));
        }

        public void Dispose()
        {
            if (_view != null)
                _view.CardClicked -= OnCardClicked;
        }
    }
}
