using System;
using UnityEngine;
using PuzzleApp.Features.MatchingPair;

namespace PuzzleApp.UI
{
    public class MatchingPairCardItem : CardItemBase<MatchingPairDefinition>
    {
        [SerializeField] int _pieceCount;

        public event Action<int> Clicked;

        public int PieceCount { get => _pieceCount; set => _pieceCount = value; }

        protected override void ApplyBinding(MatchingPairDefinition data)
        {
            _pieceCount = data.pieceCount;

            if (_iconImage != null)
            {
                _iconImage.sprite = data.icon;
                _iconImage.enabled = data.icon != null;
            }
        }

        protected override void OnClicked(MatchingPairDefinition data) => Clicked?.Invoke(_pieceCount);
    }
}
