using System;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.UI
{
    /// <summary>
    /// View-only: wires bottom bar buttons and raises <see cref="TabSelected"/>.
    /// Navigation policy lives in <c>INavigationSubsystem</c> / presenter (subscribes to this event).
    /// </summary>
    public class BottomBarView : MonoBehaviour
    {
        [SerializeField] Button _homeButton;
        [SerializeField] Button _gameButton;
        [SerializeField] Button _shopButton;

        public event Action<MainTab> TabSelected;

        void Awake() => Bind();

        void OnDestroy() => Unbind();

        void Bind()
        {
            if (_homeButton != null) _homeButton.onClick.AddListener(OnHomeClicked);
            if (_gameButton != null) _gameButton.onClick.AddListener(OnGameClicked);
            if (_shopButton != null) _shopButton.onClick.AddListener(OnShopClicked);
        }

        void Unbind()
        {
            if (_homeButton != null) _homeButton.onClick.RemoveListener(OnHomeClicked);
            if (_gameButton != null) _gameButton.onClick.RemoveListener(OnGameClicked);
            if (_shopButton != null) _shopButton.onClick.RemoveListener(OnShopClicked);
        }

        void OnHomeClicked() => Raise(MainTab.Home);
        void OnGameClicked() => Raise(MainTab.Game);
        void OnShopClicked() => Raise(MainTab.Shop);

        void Raise(MainTab tab) => TabSelected?.Invoke(tab);
    }
}
