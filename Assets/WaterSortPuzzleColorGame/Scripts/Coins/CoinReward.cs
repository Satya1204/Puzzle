using DG.Tweening;
using TMPro;
using UnityEngine;
namespace WaterSortPuzzleGame
{
    [System.Serializable]
    public class CoinReward : Reward
    {
        [SerializeField] int amount;
        public int Amount => amount;

        [SerializeField] TextMeshProUGUI amountText;
        public TextMeshProUGUI AmountText => amountText;

        [SerializeField] string textFormating = "x{0}";
        public string TextFormating => textFormating;
        [SerializeField] TextMeshProUGUI purchaseFloatingText;
        public TextMeshProUGUI PurchaseFloatingText => purchaseFloatingText;
        private Vector2 floatingTextPosition;
        public Vector2 FloatingTextPosition => floatingTextPosition;
        public override void Init()
        {
            if (amountText != null)
            {
                string numberText = amount.ToString();
                amountText.text = string.Format(textFormating == "" ? "{0}" : textFormating, numberText);
            }
            if (purchaseFloatingText != null)
            {
                floatingTextPosition = purchaseFloatingText.rectTransform.anchoredPosition;
            }
        }

        public override void ApplyReward(int quantity)
        {
            int totalAmount = amount * quantity;
            CoinManager.AddCoins(totalAmount);
        }
        public override void ApplyAnimation(int quantity)
        {
            int totalAmount = amount * quantity;

            TextMeshProUGUI floatingText = purchaseFloatingText;
            if (floatingText != null)
            {
                floatingText.gameObject.SetActive(true);
                floatingText.text = string.Format("+{0}", totalAmount);

                RectTransform textRectTransform = floatingText.rectTransform;
                textRectTransform.anchoredPosition = floatingTextPosition;

                Color newColor = floatingText.color;
                newColor.a = 1.0f;
                floatingText.color = newColor;

                textRectTransform.DOAnchorPos(textRectTransform.anchoredPosition + new Vector2(0, 100), 1.0f)
               .SetEase(Ease.InSine);

                floatingText.DOFade(0.0f, 1.0f).SetEase(Ease.InQuint).OnComplete(() =>
                {
                    textRectTransform.anchoredPosition = floatingTextPosition;

                    floatingText.gameObject.SetActive(false);
                });

            }
        }
    }
}