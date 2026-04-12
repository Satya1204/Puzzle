using System;
using System.Collections.Generic;
using UnityEngine;
using PuzzleApp.App.Signals;
using PuzzleApp.UI;

namespace PuzzleApp.App.Subsystems
{
    public interface IHudScreenSubsystem
    {
        void RegisterScreen(MainTab tab, GameObject screenRoot);
        void Show(MainTab tab);
    }

    public sealed class HudScreenSubsystem : IHudScreenSubsystem, IDisposable
    {
        readonly Dictionary<MainTab, GameObject> _screenRoots = new();
        readonly IDisposable _subscription;

        public HudScreenSubsystem(ISignalBus signalBus)
        {
            _subscription = signalBus.Subscribe<ActiveHudScreenChangedSignal>(OnScreenChanged);
        }

        public void RegisterScreen(MainTab tab, GameObject screenRoot)
        {
            if (screenRoot == null)
                return;

            _screenRoots[tab] = screenRoot;
        }

        public void Show(MainTab tab)
        {
            foreach (var pair in _screenRoots)
            {
                if (pair.Value == null)
                    continue;

                pair.Value.SetActive(pair.Key == tab);
            }
        }

        void OnScreenChanged(ActiveHudScreenChangedSignal signal)
        {
            Show(signal.Tab);
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}
