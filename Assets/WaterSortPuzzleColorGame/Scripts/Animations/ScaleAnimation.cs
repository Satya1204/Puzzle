using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    [System.Serializable]
    public class ScaleAnimation
    {
        [SerializeField] Transform transform;
        public Transform Transform => transform;

        private Tween scaleTween;
        public ScaleAnimation(Transform transform)
        {
            this.transform = transform;
        }

        public void Show(float duration = 0.15f,
            float uniformtargetScale = 1f,
            float delay = 0f, 
            Ease easeType = Ease.InOutSine,
            bool resetScale = true,
            Action onCompleted = null)
        {
            Vector3 target = uniformtargetScale * Vector3.one;

            if (transform == null)
            {
                Debug.LogError("Transform value cannot be null. Please ensure it is assigned in the Inspector or passed through the constructor.");

                onCompleted?.Invoke();

                return;
            }

            scaleTween.Kill();


            // RESET

            if(resetScale)
            transform.localScale = Vector3.zero;

            scaleTween = transform.DOScale(target, duration).SetEase(easeType).SetDelay(delay).OnComplete(() =>
            {
                onCompleted?.Invoke();
            });
        }

        public void Hide(float duration = 0.15f, float delay = 0f, Action onCompleted = null)
        {
            if (transform == null)
            {
                Debug.LogError("Transform value cannot be null. Please ensure it is assigned in the Inspector or passed through the constructor.");

                onCompleted?.Invoke();

                return;
            }

            scaleTween.Kill();


            scaleTween = transform.DOScale(Vector3.zero, duration).SetEase(Ease.InOutSine).SetDelay(delay).OnComplete(() =>
            {
                onCompleted?.Invoke();
            });
        }
    }
}
