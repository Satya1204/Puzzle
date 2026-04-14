using System;
using PuzzleApp.App.Signals;
using PuzzleApp.UI;

namespace PuzzleApp.App.Controllers
{
    public sealed class BottomBarController : IDisposable
    {
        readonly BottomBarView _view;
        readonly ISignalBus _signalBus;

        public BottomBarController(BottomBarView view, ISignalBus signalBus)
        {
            _view = view;
            _signalBus = signalBus;
            _view.TabSelected += OnTabSelected;
        }

        void OnTabSelected(MainTab tab)
        {
            UnityEngine.Debug.LogError("tab " + tab.ToString());
            _signalBus.Publish(new MainTabSelectedSignal(tab));
        }

        public void Dispose()
        {
            if (_view != null)
                _view.TabSelected -= OnTabSelected;
        }
    }
}
