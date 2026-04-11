using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
namespace WaterSortPuzzleGame
{
    public class UnlockedBoosterPanel : UIPage
    {
        [SerializeField] private ScaleAnimation animPopup;
        [SerializeField] private Image unlockedIcon;
        [SerializeField] private TMP_Text unlockedTitle;
        [SerializeField] private TMP_Text noOfClaim;
        [SerializeField] private Button onClaim;
        private List<BoosterSettings> unlockBoosters;
        private int pageIndex = 0;
        private BoosterSettings settings;

        public override void Init()
        {
            onClaim.onClick.AddListener(ClaimBooster);
        }

        public override void PlayShowAnimation()
        {
            animPopup.Show();
        }
        public override void PlayHideAnimation()
        {
            animPopup.Hide();
        }
        public void SetData(List<BoosterSettings> unlockedBoosters)
        {
            unlockBoosters = unlockedBoosters;
            pageIndex = 0;

            PreparePage(pageIndex);
        }

        private void PreparePage(int index)
        {
            if (!IsInRange(unlockBoosters,index)) return;

            settings = unlockBoosters[index];

            unlockedIcon.sprite = settings.Icon;
            unlockedTitle.text = settings.Title;
            noOfClaim.text = "x " + settings.NoOfClaim.ToString();
        }
        public void ClaimBooster()
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
            BoosterController.UnlockBooster(settings.Type);
            pageIndex++;

            if (pageIndex >= unlockBoosters.Count)
            {
                BackHandler.RemoveRecentlyScreen();

                foreach (BoosterSettings unlockerBooster in unlockBoosters)
                {
                    BoosterController.Instance.ShowBoosterClaimAnimation(unlockerBooster.Type);
                }
            }
            else
            {
                PreparePage(pageIndex);
            }
        }
        private bool IsInRange(List<BoosterSettings> list, int value)
        {
            return (value >= 0 && value < list.Count);
        }
    }
}