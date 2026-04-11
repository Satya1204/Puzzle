using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace WaterSortPuzzleGame
{
    public class RewardedAdUIController : MonoBehaviour
    {
        [Header("Required")]
        [SerializeField] private Button adButton;

        [Header("Optional UI")]
        [SerializeField] private GameObject loadingIcon;
        [SerializeField] private GameObject adsIcon;
        [SerializeField] private Image buttonImage;
        [SerializeField] private Sprite enabledSprite;
        [SerializeField] private Sprite disabledSprite;


        public Action onAdRewarded;

        private Coroutine adCheckRoutine;
        private Tween loadingTween;
        public void Init()
        {
            adButton.onClick.AddListener(OnClickButton);
        }

        private void OnEnable()
        {
            UpdateButtonState();
            adCheckRoutine = StartCoroutine(CheckAdAvailability());

            if (loadingIcon != null && loadingIcon.activeSelf)
                StartLoadingAnimation();
        }

        private void OnDisable()
        {
            if (adCheckRoutine != null)
                StopCoroutine(adCheckRoutine);

            StopLoadingAnimation();
        }
        private void StopLoadingAnimation()
        {
            loadingTween?.Kill();
            loadingTween = null;
        }
        private void StartLoadingAnimation()
        {

            loadingTween = loadingIcon.transform
                .DORotate(new Vector3(0, 0, -360), 3f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
        }
        private void UpdateButtonState()
        {
            bool isReady = AdsManager.Instance.IsRewardedAdReady();

            if (loadingIcon != null)
            {
                loadingIcon.SetActive(!isReady);
                if (!isReady && loadingTween == null)
                    StartLoadingAnimation();
                else
                    StopLoadingAnimation();
            }


            if (adsIcon != null)
                adsIcon.SetActive(isReady);

            if (buttonImage != null)
                buttonImage.sprite = isReady ? enabledSprite : disabledSprite;
        }

        private IEnumerator CheckAdAvailability()
        {
            while (!AdsManager.Instance.IsRewardedAdReady())
            {
                yield return new WaitForSeconds(0.5f);
            }

            UpdateButtonState(); // Once ad is ready, enable button
        }

        private void OnClickButton()
        {
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.touch);

            AdsManager.Instance.ShowRewardedAd(() =>
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.purchaseComplete);
                onAdRewarded?.Invoke();
            });
        }
    }
}