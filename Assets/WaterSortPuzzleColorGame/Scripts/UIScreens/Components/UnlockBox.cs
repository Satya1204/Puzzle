using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace WaterSortPuzzleGame
{
    
    [RequireComponent(typeof(RectTransform))]
    public class UnlockBox : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI progressText;

        /// <summary>
        /// Sets the unlock box data and handles visibility.
        /// </summary>
        /// <param name="spriteName">Sprite path (Resources folder)</param>
        /// <param name="unlockLevel">Level at which this item unlocks</param>
        public void SetData(Sprite sprite, int unlockLevel)
        {
            if (sprite == null || unlockLevel <= 0)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            iconImage.sprite = sprite;
            progressText.text = $"{GameManager.LevelIndex + 1}/{unlockLevel}";
        }
    }
}
