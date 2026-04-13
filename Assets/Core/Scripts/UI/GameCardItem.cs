using System;
using UnityEngine;
using UnityEngine.UI;
using PuzzleApp.Features.GameCatalog;

namespace PuzzleApp.UI
{
    /// <summary>
    /// View for a single game card.
    /// </summary>
    public class GameCardItem : MonoBehaviour
    {
        [SerializeField] Button _button;

        public event Action<int> Clicked;

        public int GameId { get; private set; }

        void Awake()
        {
            if (_button == null)
                _button = GetComponent<Button>();
        }

        void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnPressed);
        }

        public void Bind(GameCardViewModel viewModel)
        {
            GameId = viewModel.Id;

            if (_button == null)
                _button = GetComponent<Button>();

            if (_button == null)
                return;

            _button.onClick.RemoveListener(OnPressed);
            _button.onClick.AddListener(OnPressed);

            // Extend here: bind title, icon, lock state, or badge views.
        }

        void OnPressed() => Clicked?.Invoke(GameId);
    }
}
