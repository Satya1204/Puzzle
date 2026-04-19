using System;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleApp.MatchObjects
{
    public class MatchObjectsDropZone : MonoBehaviour
    {
        [SerializeField] Image _background;
        [SerializeField] Image _correctBadge;

        string _expectedPairId;
        bool _isOccupied;
        MatchObjectsDraggableItem _occupyingItem;
        Action _onCorrectDrop;

        public string ExpectedPairId => _expectedPairId;
        public bool IsOccupied => _isOccupied;

        public void Setup(string expectedPairId, Action onCorrectDrop)
        {
            _expectedPairId = expectedPairId;
            _onCorrectDrop = onCorrectDrop;
            _isOccupied = false;
            if (_correctBadge != null)
                _correctBadge.gameObject.SetActive(false);
        }

        public bool ContainsScreenPoint(Vector2 screenPos)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(
                (RectTransform)transform,
                screenPos,
                null);
        }

        public bool TryAccept(MatchObjectsDraggableItem item)
        {
            if (_isOccupied)
                return false;

            if (item.PairId != _expectedPairId)
                return false;

            Accept(item);
            return true;
        }

        void Accept(MatchObjectsDraggableItem item)
        {
            _isOccupied = true;
            _occupyingItem = item;

            item.RectTransform.SetParent(transform, false);
            item.RectTransform.anchoredPosition = Vector2.zero;
            item.Lock();

            if (_correctBadge != null)
                _correctBadge.gameObject.SetActive(true);

            _onCorrectDrop?.Invoke();
        }
    }
}
