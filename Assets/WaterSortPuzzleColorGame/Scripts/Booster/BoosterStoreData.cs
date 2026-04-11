using UnityEngine;
using TMPro;
using UnityEngine.UI;
using WaterSortPuzzleGame.DataClass;

namespace WaterSortPuzzleGame
{
    [System.Serializable]
    public class BoosterStoreData
    {
        [SerializeField] BoosterType boosterType;
        public BoosterType BoosterType => boosterType;

        [SerializeField] int amount;
        public int Amount => amount;

        [Space]
        [SerializeField] Image iconImage;
        public Image IconImage => iconImage;

        [SerializeField] TextMeshProUGUI amountText;
        public TextMeshProUGUI AmountText => amountText;

        [SerializeField] TextMeshProUGUI purchaseFloatingText;
        public TextMeshProUGUI PurchaseFloatingText => purchaseFloatingText;

        [SerializeField] string textFormating = "x{0}";
        public string TextFormating => textFormating;

        private Vector2 floatingTextPosition;
        public Vector2 FloatingTextPosition => floatingTextPosition;

        public void Init()
        {
            if (purchaseFloatingText != null)
            {
                floatingTextPosition = purchaseFloatingText.rectTransform.anchoredPosition;
            }
        }
    }
}