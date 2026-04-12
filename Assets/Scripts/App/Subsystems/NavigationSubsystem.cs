using System;
using PuzzleApp.App.Signals;
using PuzzleApp.UI;

namespace PuzzleApp.App.Subsystems
{
    public interface INavigationSubsystem
    {
        MainTab CurrentTab { get; }
        void SetTab(MainTab tab);
        void PublishCurrent();
    }

    public sealed class NavigationSubsystem : INavigationSubsystem, IDisposable
    {
        readonly ISignalBus _signalBus;
        readonly IDisposable _subscription;

        public NavigationSubsystem(ISignalBus signalBus, MainTab initialTab)
        {
            _signalBus = signalBus;
            CurrentTab = initialTab;
            _subscription = _signalBus.Subscribe<MainTabSelectedSignal>(OnMainTabSelected);
        }

        public MainTab CurrentTab { get; private set; }

        public void SetTab(MainTab tab)
        {
            CurrentTab = tab;
            PublishCurrent();
        }

        public void PublishCurrent()
        {
            _signalBus.Publish(new ActiveHudScreenChangedSignal(CurrentTab));
        }

        void OnMainTabSelected(MainTabSelectedSignal signal)
        {
            if (signal.Tab == CurrentTab)
            {
                PublishCurrent();
                return;
            }

            CurrentTab = signal.Tab;
            PublishCurrent();
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}
