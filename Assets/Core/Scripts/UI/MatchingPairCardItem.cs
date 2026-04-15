using System;
using UnityEngine;
using UnityEngine.UI;
using PuzzleApp.Features.MatchingPair;

namespace PuzzleApp.UI
{
    /// <summary>
    /// View for a single piece card inside the MatchingPair lobby grid.
    /// Works for both inspector-placed cards (set <c>_pieceCount</c> in inspector)
    /// and dynamically spawned cards (call <see cref="Bind"/>).
    /// </summary>
    public class MatchingPairCardItem : MonoBehaviour
    {
        [SerializeField] Button _button;
        [SerializeField] Image _iconImage;
        [SerializeField] int _pieceCount;

        public event Action<int> Clicked;

        public int PieceCount { get => _pieceCount; set => _pieceCount = value; }

        void Awake()
        {
            if (_button == null)
                _button = GetComponent<Button>();

            if (_button != null)
            {
                _button.onClick.RemoveListener(OnPressed);
                _button.onClick.AddListener(OnPressed);
            }
        }

        void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnPressed);
        }

        public void Bind(MatchingPairDefinition definition)
        {
            _pieceCount = definition.pieceCount;

            if (_button == null)
                _button = GetComponent<Button>();

            if (_button == null)
                return;

            _button.onClick.RemoveListener(OnPressed);
            _button.onClick.AddListener(OnPressed);

            if (_iconImage != null)
            {
                _iconImage.sprite = definition.icon;
                _iconImage.enabled = definition.icon != null;
            }
        }

        void OnPressed() => Clicked?.Invoke(_pieceCount);
    }
}
