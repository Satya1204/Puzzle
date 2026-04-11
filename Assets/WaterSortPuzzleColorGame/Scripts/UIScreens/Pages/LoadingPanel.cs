using DG.Tweening;
using System;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    public class LoadingPanel : UIPage
    {
        [SerializeField] private GameObject loadingIcon;
        private Tween loadingTween;
        public override void Init()
        {

        }
        public override void PlayShowAnimation()
        {
            if (loadingIcon != null && loadingIcon.activeSelf)
                StartLoadingAnimation();
        }
        public override void PlayHideAnimation()
        {
            StopLoadingAnimation();
        }
        private void StartLoadingAnimation()
        {
            loadingTween?.Kill();

            loadingTween = loadingIcon.transform
                .DORotate(new Vector3(0, 0, -360), 3f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
        }
        private void StopLoadingAnimation()
        {
            loadingTween?.Kill();
            loadingTween = null;
        }
    }
}