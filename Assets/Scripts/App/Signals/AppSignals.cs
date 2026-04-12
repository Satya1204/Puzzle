using PuzzleApp.UI;

namespace PuzzleApp.App.Signals
{
    public readonly struct MainTabSelectedSignal
    {
        public MainTabSelectedSignal(MainTab tab)
        {
            Tab = tab;
        }

        public MainTab Tab { get; }
    }

    public readonly struct ActiveHudScreenChangedSignal
    {
        public ActiveHudScreenChangedSignal(MainTab tab)
        {
            Tab = tab;
        }

        public MainTab Tab { get; }
    }

    public readonly struct GameCardSelectedSignal
    {
        public GameCardSelectedSignal(int gameId)
        {
            GameId = gameId;
        }

        public int GameId { get; }
    }
}
