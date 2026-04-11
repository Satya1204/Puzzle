using WaterSortPuzzleGame.Enum;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using WaterSortPuzzleGame.DataClass;
using DG.Tweening;
using System.Collections;

namespace WaterSortPuzzleGame
{
    public class LevelCompletePanel : UIPage
    {
        [Header("Popup")]
        [SerializeField] private ScaleAnimation animPopup;

        [Header("Level Info")]
        [SerializeField] private int freeCoins;
        [SerializeField] private TMP_Text freeCoinsButtonText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private RewardedAdUIController getFreeCoins;

        [Header("Stars")]
        [SerializeField] private GameObject[] starsList = new GameObject[3];

        [Header("Unlock Boxes")]
        [SerializeField] private UnlockBox tubeBox;
        [SerializeField] private UnlockBox themeBox;
        [SerializeField] private Transform freeCoinBox;


        [Header("Next Level Button")]
        [SerializeField] private GameObject nextLevelButton;
        

        [Header("Animations")]
        private ScaleAnimation[] starAnimations = new ScaleAnimation[3];
        private ScaleAnimation[] levelCompleteBoxAnimations = new ScaleAnimation[3];
        private ScaleAnimation nextLevelButtonAnim;
       
        public override void Init()
        {
            nextLevelButton.GetComponent<Button>().onClick.AddListener(NextLevelButtonAction);
            getFreeCoins.Init();
            getFreeCoins.onAdRewarded = OnRewardedAdCompleted;

            InitializeAnimations();
            
            SetFreeCoinsData();
        }
        public override void OnPageResume()
        {
            SetData();
            ResetAllScales();
        }
        public override void PlayShowAnimation()
        {
            
            animPopup.Show(onCompleted : () =>
            {
                StartCoroutine(AnimationLevelComplete());
            });
            
        }
        public override void PlayHideAnimation()
        {
            animPopup.Hide();
        }
        private void SetData()
        {
            levelText.text = "Level " + (GameManager.LevelIndex + 1).ToString();

            var nextTube = SkinManager.Instance.TubesSkinController.GetNextUnlock();
            var nextTheme = SkinManager.Instance.ThemeSkinController.GetNextUnlock();

            tubeBox.SetData(nextTube?.image, nextTube?.unlockValue ?? 0);

            themeBox.SetData(nextTheme?.image, nextTheme?.unlockValue ?? 0);
        }
        public void NextLevelButtonAction()
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
            BackHandler.RemoveRecentlyScreen();

            // delete last level and create new level prototype
            EventManager.CreateNewLevelForJson?.Invoke();
            // update level text
            EventManager.UpdateLevelText?.Invoke();

        }
        private void OnRewardedAdCompleted()
        {
            CoinManager.AddCoins(freeCoins);
        }
        
        private void SetFreeCoinsData()
        {
            freeCoinsButtonText.text = $"Get {freeCoins} Free Coins";
        }
        private void InitializeAnimations()
        {
            // Init Star Animations
            for (int i = 0; i < starsList.Length; i++)
            {
                if (starsList[i] != null)
                {
                    starAnimations[i] = new ScaleAnimation(starsList[i].transform);
                }
            }

            levelCompleteBoxAnimations[0] = new ScaleAnimation(tubeBox.transform);
            levelCompleteBoxAnimations[1] = new ScaleAnimation(themeBox.transform);
            levelCompleteBoxAnimations[2] = new ScaleAnimation(freeCoinBox);
           

            // Init Next Level Button Animation
            if (nextLevelButton != null)
            {
                nextLevelButtonAnim = new ScaleAnimation(nextLevelButton.transform);
            }
        }
        private void ResetAllScales()
        {

            if (starAnimations.Length >= 3)
            {
                RectTransform rect0 = starAnimations[0].Transform.GetComponent<RectTransform>();
                RectTransform rect1 = starAnimations[1].Transform.GetComponent<RectTransform>();
                RectTransform rect2 = starAnimations[2].Transform.GetComponent<RectTransform>();

                if (rect0) rect0.anchoredPosition = new Vector2(141f, -182f);
                if (rect1) rect1.anchoredPosition = new Vector2(0f, -250f);
                if (rect2) rect2.anchoredPosition = new Vector2(-150f, -182f);
            }


            foreach (var star in starAnimations)
            {
                if (star.Transform != null)
                    star.Transform.localScale = Vector3.zero;
            }

            foreach (var box in levelCompleteBoxAnimations)
            {
                if (box.Transform != null)
                    box.Transform.localScale = Vector3.zero;
            }

            if (nextLevelButtonAnim.Transform != null)
                nextLevelButtonAnim.Transform.localScale = Vector3.zero;
        }
        private IEnumerator AnimationLevelComplete()
        {
            Vector2 originalStarPosition = Vector2.zero;

            for (int i = 0; i < starAnimations.Length; i++)
            {
                int index = i;

                // Reset scale to big (if needed)
                //starAnimations[index].Transform.localScale = Vector3.one * 2f;

                // Show animation using your ScaleAnimation script
                starAnimations[index].Show(duration : 0.5f,
                    uniformtargetScale : 2f,
                    easeType : Ease.InOutSine,
                    onCompleted  : () =>
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
                    starAnimations[index].Show(duration: 0.5f, uniformtargetScale: 1f, easeType: Ease.OutBounce, resetScale: false);
                });

                // Slide the star in place
                starsList[index].GetComponent<RectTransform>()
                    .DOAnchorPos(originalStarPosition, 0.8f).SetEase(Ease.InBack);

                yield return new WaitForSeconds(0.25f);
            }

            yield return StartCoroutine(LevelCompleteBoxListAnimation());
        }
        private IEnumerator LevelCompleteBoxListAnimation()
        {
            for (int i = 0; i < levelCompleteBoxAnimations.Length; i++)
            {
                int index = i;

                AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);

                levelCompleteBoxAnimations[index].Show(
                    duration: 1f,
                    uniformtargetScale: 1f,
                    easeType: Ease.OutBounce
                );

                yield return new WaitForSeconds(0.25f);
            }

            // Next Level button animation
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);

            nextLevelButtonAnim.Show(
                duration: 0.5f,
                uniformtargetScale: 1f,
                easeType: Ease.OutBounce,
                onCompleted: () =>
                {
                    AdsManager.Instance.ShowInterstitialAd();
                });
        }
    }
}