using System;
using UnityEngine;
using TMPro;
using PuzzleApp.Features.MatchObjects;

namespace PuzzleApp.UI
{
    public class MatchObjectsCardItem : CardItemBase<MatchObjectsDefinition>
    {
        [SerializeField] TextMeshProUGUI _titleText;

        public event Action<MatchObjectsDefinition> Clicked;

        protected override void ApplyBinding(MatchObjectsDefinition data)
        {
            if (_titleText != null)
                _titleText.text = data.title;

            if (_iconImage != null && data.icon != null)
                _iconImage.sprite = data.icon;
        }

        protected override void OnClicked(MatchObjectsDefinition data) => Clicked?.Invoke(data);
    }
}
