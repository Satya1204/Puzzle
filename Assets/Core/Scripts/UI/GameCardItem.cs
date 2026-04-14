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
        [SerializeField] Image _iconImage;

        public event Action<int> Clicked;

        public int GameId { get;  set; }

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

            if (_iconImage != null)
            {
                _iconImage.sprite = viewModel.Icon;
                _iconImage.enabled = viewModel.Icon != null;
            }
        }

        void OnPressed() => Clicked?.Invoke(GameId);
    }
}
