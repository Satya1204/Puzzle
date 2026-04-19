using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PuzzleApp.MatchObjects
{
    public class MatchObjectsDraggableItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] Image _image;
        [SerializeField] CanvasGroup _canvasGroup;

        string _pairId;
        RectTransform _rectTransform;
        Canvas _rootCanvas;
        Transform _originalParent;
        Vector2 _originalPosition;
        MatchObjectsDropZone[] _dropZones;
        bool _isDragging;
        bool _isLocked;
        Coroutine _returnCoroutine;

        public string PairId => _pairId;
        public RectTransform RectTransform => _rectTransform;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Bind(string pairId, Sprite sprite, MatchObjectsDropZone[] dropZones, Canvas rootCanvas)
        {
            _pairId = pairId;
            _image.sprite = sprite;
            _dropZones = dropZones;
            _rootCanvas = rootCanvas;
        }

        public void Lock()
        {
            _isLocked = true;
            if (_canvasGroup != null)
                _canvasGroup.blocksRaycasts = false;
        }

        void OnDestroy()
        {
            if (_returnCoroutine != null)
                StopCoroutine(_returnCoroutine);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isLocked || _isDragging)
                return;

            _isDragging = true;
            _originalParent = _rectTransform.parent;
            _originalPosition = _rectTransform.anchoredPosition;

            if (_returnCoroutine != null)
            {
                StopCoroutine(_returnCoroutine);
                _returnCoroutine = null;
            }

            _rectTransform.SetParent(_rootCanvas.transform, false);
            if (_canvasGroup != null)
                _canvasGroup.alpha = 0.7f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging || _isLocked)
                return;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rootCanvas.transform as RectTransform,
                eventData.position,
                _rootCanvas.worldCamera,
                out var localPoint))
            {
                _rectTransform.anchoredPosition = localPoint;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isDragging || _isLocked)
                return;

            _isDragging = false;

            MatchObjectsDropZone targetZone = null;
            foreach (var zone in _dropZones)
            {
                if (zone.ContainsScreenPoint(eventData.position))
                {
                    targetZone = zone;
                    break;
                }
            }

            if (targetZone != null && targetZone.TryAccept(this))
            {
                if (_canvasGroup != null)
                    _canvasGroup.alpha = 1f;
                return;
            }

            _returnCoroutine = StartCoroutine(ReturnToOrigin());
        }

        IEnumerator ReturnToOrigin()
        {
            float duration = 0.3f;
            float elapsed = 0f;
            Vector2 startPos = _rectTransform.anchoredPosition;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                _rectTransform.anchoredPosition = Vector2.Lerp(startPos, _originalPosition, t);
                yield return null;
            }

            _rectTransform.anchoredPosition = _originalPosition;
            _rectTransform.SetParent(_originalParent, false);

            if (_canvasGroup != null)
                _canvasGroup.alpha = 1f;

            _returnCoroutine = null;
        }
    }
}
