using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace WaterSortPuzzleGame
{
    public class SkinUnlockedPanel : UIPage
    {
        [SerializeField] private ScaleAnimation animPopup;

        [Header("Description")]
        [SerializeField] private TMP_Text description;

        [Header("Images")]
        [SerializeField] private Image tubeImage;
        [SerializeField] private Image themeImage;
        [SerializeField] private Image paletteImage;

        [Header("Buttons")]
        [SerializeField] private Button onApply;
        [SerializeField] private Button onBack;
        [SerializeField] private Button onClose;

        private List<(SkinType, SkinData)> unlockedSkins;
        private int pageIndex = 0;

        public Action OnPopupClosed;

        public override void Init()
        {
            onApply.onClick.AddListener(ApplySkin);
            onBack.onClick.AddListener(OnBackgroundClick);
            onClose.onClick.AddListener(OnCloseClick);
        }

        public override void PlayShowAnimation()
        {
            animPopup.Show();
        }

        public override void PlayHideAnimation()
        {
            animPopup.Hide();
            OnPopupClosed?.Invoke();
        }

        public void SetData(List<(SkinType, SkinData)> skins)
        {
            unlockedSkins = skins;
            pageIndex = 0;
            PreparePage(pageIndex);
        }

        private void PreparePage(int index)
        {
            if (index < 0 || index >= unlockedSkins.Count) return;

            HideAllImages();

            var (skinType, data) = unlockedSkins[index];

            description.text = $"Congratulations! You Unlocked a New {skinType}";
            if (skinType == SkinType.Tube)
            {
                tubeImage.gameObject.SetActive(true);
                tubeImage.sprite = data.image;
            }
            else if (skinType == SkinType.Theme)
            {
                themeImage.gameObject.SetActive(true);
                themeImage.sprite = data.image;
            }
            else if (skinType == SkinType.Palette)
            {
                paletteImage.gameObject.SetActive(true);
                paletteImage.sprite = data.image;
            }
        }

        private void HideAllImages()
        {
            tubeImage.gameObject.SetActive(false);
            themeImage.gameObject.SetActive(false);
            paletteImage.gameObject.SetActive(false);
        }

        private void ApplySkin()
        {
           
            // Apply based on current page
            var (skinType, data) = unlockedSkins[pageIndex];

            if (skinType == SkinType.Tube)
                SkinManager.Instance.TubesSkinController.SelectTube((TubeSkinData)data);
            else if (skinType == SkinType.Theme)
                SkinManager.Instance.ThemeSkinController.SelectTheme((ThemeSkinData)data);
            else if (skinType == SkinType.Palette)
                SkinManager.Instance.PaletteSkinController.SelectPalette((PaletteSkinData)data);

            // Next page or close
            GoNextPage();
        }
        private void GoNextPage()
        {
            pageIndex++;
            if (pageIndex >= unlockedSkins.Count)
            {
                BackHandler.RemoveRecentlyScreen();
            }
            else
            {
                PreparePage(pageIndex);
            }
        }
        private void OnBackgroundClick()
        {
            GoNextPage();
        }
        private void OnCloseClick()
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
            GoNextPage();
        }
    }
}