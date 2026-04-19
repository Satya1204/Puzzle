namespace PuzzleApp.App.Services
{
    public interface IGameDataProvider
    {
        T LoadJson<T>(string resourcePath) where T : class;
    }
}
