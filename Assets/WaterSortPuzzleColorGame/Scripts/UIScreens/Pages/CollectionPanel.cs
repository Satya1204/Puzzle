using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WaterSortPuzzleGame.DataClass;

namespace WaterSortPuzzleGame
{
    public class CollectionPanel : UIPage
    {
        [SerializeField] private Transform popupTransform;
        private MoveAnimation animPopup;

        [SerializeField]
        private GameObject[] selectedTabView;

        [Header("Buttons")]
        [SerializeField] Button backButton;

        [Header("Toggle")]
        [SerializeField] private Toggle tubeToggle;
        [SerializeField] private Toggle themeToggle;
        [SerializeField] private Toggle palettesToggle;

        [Header("Skin Item Grid")]
        [SerializeField] private TubeSkinItemsGrid tubeSkinItemsGrid;
        [SerializeField] private ThemeSkinItemsGrid themeSkinItemsGrid;
        [SerializeField] private PaletteSkinItemsGrid paletteSkinItemsGrid;
        public override void Init()
        {
            InitTransfrom();
            backButton.onClick.AddListener(BackClick);
            tubeToggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn) OnChangeItem(0);
            });
            themeToggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn) OnChangeItem(1);
            });
            palettesToggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn) OnChangeItem(2);
            });
        }
        public void InitTransfrom()
        {
            animPopup = new MoveAnimation(popupTransform);
        }
        public void OnChangeItem(int index)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
            for (var i = 0; i < selectedTabView.Length; i++)
            {
                selectedTabView[i].SetActive(i == index);
            }
        }
        public override void OnPageResume()
        {
            ResetScrollOfActiveTab();

            // Turn off all toggles first
            tubeToggle.SetIsOnWithoutNotify(false);
            themeToggle.SetIsOnWithoutNotify(false);
            palettesToggle.SetIsOnWithoutNotify(false);

            // Now turn on first toggle
            tubeToggle.SetIsOnWithoutNotify(true);

            OnChangeItem(0);

            SkinManager.Instance.ThemeSkinController.UpdateAutoUnlocks();
            themeSkinItemsGrid.Init();

            SkinManager.Instance.TubesSkinController.UpdateAutoUnlocks();
            tubeSkinItemsGrid.Init();

            SkinManager.Instance.PaletteSkinController.UpdateAutoUnlocks();
            paletteSkinItemsGrid.Init();
        }
        public override void PlayShowAnimation()
        {
            AdsManager.Instance.HideBannerAd();
            animPopup.Show();
        }
        public override void PlayHideAnimation()
        {
            if (GameManager.IsLevelScreen) AdsManager.Instance.ShowBannerAd();

            animPopup.Hide();
        }
        private void ResetScrollOfActiveTab()
        {
            for (int i = 0; i < selectedTabView.Length; i++)
            {
                if (selectedTabView[i] != null)
                {
                    ScrollRect scroll = selectedTabView[i].GetComponent<ScrollRect>();
                    if (scroll != null)
                    {
                        scroll.verticalNormalizedPosition = 1f; // top
                    }
                }
            }
        }
    }
}
