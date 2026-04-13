using PuzzleApp.App.DI;

namespace PuzzleApp.App.Modules
{
    public interface IAppModule
    {
        void Register(IServiceRegistry services);
        void Initialize(IServiceRegistry services);
    }
}
