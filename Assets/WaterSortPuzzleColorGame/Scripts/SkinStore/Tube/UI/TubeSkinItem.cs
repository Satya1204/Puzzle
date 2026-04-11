using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WaterSortPuzzleGame
{
    public class TubeSkinItem : MonoBehaviour
    {
        [SerializeField] private ScaleAnimation anim;
        [SerializeField] private Image squareMask;
        [SerializeField] private Image image;
        [SerializeField] private Image lockIcon;
        [SerializeField] private TMP_Text coinText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private GameObject selectionIcon;
        [SerializeField] private GameObject unCheckBox;
        [SerializeField] private GameObject coinBox;

        [Header("Button")]
        [SerializeField] private Button itemClick;

        [Header("Unlocked Colors")]
        [SerializeField] private Color squareMaskUnlocked;
        [SerializeField] private Color imageUnlocked;

        [Header("Locked Colors")]
        [SerializeField] private Color squareMaskLocked;
        [SerializeField] private Color imageLocked;

        private TubeSkinData skinData;

        private void OnEnable()
        {
            SkinManager.Instance.TubesSkinController.OnSelectionChanged += HandleSelectionChanged;
        }

        private void OnDisable()
        {
            SkinManager.Instance.TubesSkinController.OnSelectionChanged -= HandleSelectionChanged;
        }

        private void HandleSelectionChanged(string oldId, string newId)
        {
            if (skinData.id == oldId || skinData.id == newId)
            {
                UpdateVisuals(SkinManager.Instance.TubesSkinController.SelectedSkinTubeId == skinData.id);
            }
        }
        public void Init(TubeSkinData data, bool isSelected)
        {
            skinData = data;

            itemClick.onClick.AddListener(ButtonClick);

            image.sprite = skinData.image;

            UpdateVisuals(isSelected);
        }
        private void UpdateVisuals(bool isSelected)
        {
            bool unlocked = SkinManager.Instance.TubesSkinController.IsUnlocked(skinData);
           
            lockIcon.gameObject.SetActive(!unlocked);
            squareMask.color = unlocked ? squareMaskUnlocked : squareMaskLocked;
            image.color = unlocked ? imageUnlocked : imageLocked;
            unCheckBox.gameObject.SetActive(unlocked || isSelected);
            selectionIcon.SetActive(isSelected);

            if (skinData.unlockType == UnlockType.CoinBased)
            {
                coinText.text = skinData.unlockValue.ToString();
                coinBox.SetActive(!unlocked);
                levelText.gameObject.SetActive(false);
            }
            else if (skinData.unlockType == UnlockType.LevelBased)
            {
                levelText.text = $"Level {skinData.unlockValue}";
                levelText.gameObject.SetActive(!unlocked);
                coinBox.SetActive(false);
            }

            
        }

        public void ButtonClick()
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
            transform.localScale = 1.02f * Vector3.one;
            anim.Show(resetScale: false);

            if (SkinManager.Instance.TubesSkinController.SelectedSkinTubeId == skinData.id)
            {
                // Already selected  optionally bounce or feedback
                return;
            }

            bool success = SkinManager.Instance.TubesSkinController.TryUnlockAndSelect(skinData);

            if (success)
            {
                // Selection visuals will update via event
            }
            else
            {
                // You may show locked message / prompt if needed
            }
        }
    }
}