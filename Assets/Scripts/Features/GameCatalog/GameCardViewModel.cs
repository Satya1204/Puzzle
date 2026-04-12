namespace PuzzleApp.Features.GameCatalog
{
    public readonly struct GameCardViewModel
    {
        public GameCardViewModel(int id, string title)
        {
            Id = id;
            Title = title;
        }

        public int Id { get; }
        public string Title { get; }
    }
}
