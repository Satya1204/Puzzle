using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    [System.Serializable]
    public class MoveAnimation
    {
        [SerializeField] Transform transform;
        public Transform Transform => transform;

        private Tween moveTween;
        private Vector3 originalPosition;
        public MoveAnimation(Transform transform)
        {
            this.transform = transform;
            originalPosition = transform.localPosition;
        }

        public void Show(float duration = 0.25f,
            float yDirection = 0f,
            float delay = 0f,
            Ease easeType = Ease.OutBack,
            Action onCompleted = null)
        {


            if (transform == null)
            {
                Debug.LogError("Transform value cannot be null. Please ensure it is assigned in the Inspector or passed through the constructor.");

                onCompleted?.Invoke();

                return;
            }

            moveTween.Kill();

            RectTransform popupRect = transform.GetComponent<RectTransform>();
            if (popupRect != null)
            {
                float aboveY = originalPosition.y + popupRect.rect.height + 300f;
                transform.localPosition = new Vector3(originalPosition.x, aboveY, originalPosition.z);
            }


            moveTween = transform.DOLocalMoveY(originalPosition.y, duration).SetEase(easeType).SetDelay(delay).OnComplete(() =>
            {
                onCompleted?.Invoke();
            });
        }

        public void Hide(float duration = 0.2f, float delay = 0f, Action onCompleted = null)
        {
            if (transform == null)
            {
                Debug.LogError("Transform value cannot be null. Please ensure it is assigned in the Inspector or passed through the constructor.");

                onCompleted?.Invoke();

                return;
            }

            moveTween.Kill();

            float aboveY = 0f;
            RectTransform popupRect = transform.GetComponent<RectTransform>();
            if (popupRect != null)
            {
                aboveY = originalPosition.y + popupRect.rect.height + 300f;
            }

            moveTween = transform.DOLocalMoveY(aboveY, duration).SetEase(Ease.InOutSine).SetDelay(delay).OnComplete(() =>
            {
                transform.localPosition = originalPosition;
                onCompleted?.Invoke();
            });
        }
    }
    
}
