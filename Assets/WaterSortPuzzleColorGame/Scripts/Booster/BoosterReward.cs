using DG.Tweening;
using TMPro;
using UnityEngine;
namespace WaterSortPuzzleGame
{
    [System.Serializable]
    public class BoosterReward : Reward
    {
        [SerializeField] BoosterStoreData[] boostersData;

        public override void Init()
        {
            foreach (BoosterStoreData boosterData in boostersData)
            {
                boosterData.Init();

                if (boosterData.IconImage != null)
                {
                    BoosterBehavior boosterBehavior = BoosterController.GetBoosterBehavior(boosterData.BoosterType);
                    if (boosterBehavior != null)
                    {
                        boosterData.IconImage.sprite = boosterBehavior.Settings.Icon;
                    }
                }

                if (boosterData.AmountText != null)
                {
                    boosterData.AmountText.text = string.Format(string.IsNullOrEmpty(boosterData.TextFormating) ? boosterData.Amount.ToString() : string.Format(boosterData.TextFormating, boosterData.Amount));
                }
            }
        }

        public override void ApplyReward(int quantity)
        {
            foreach (BoosterStoreData boosterData in boostersData)
            {
                int totalAmount = boosterData.Amount * quantity;
                BoosterController.IncrementBooster(boosterData.BoosterType, totalAmount);
            }
            BoosterController.BoosterUIController.RedrawPanels();
        }
        public override void ApplyAnimation(int quantity)
        {
            foreach (BoosterStoreData boosterData in boostersData)
            {
                int totalAmount = boosterData.Amount * quantity;

                TextMeshProUGUI floatingText = boosterData.PurchaseFloatingText;
                if (floatingText != null)
                {
                    floatingText.gameObject.SetActive(true);
                    floatingText.text = string.Format("+{0}", totalAmount);

                    RectTransform textRectTransform = floatingText.rectTransform;
                    textRectTransform.anchoredPosition = boosterData.FloatingTextPosition;

                    Color newColor = floatingText.color;
                    newColor.a = 1.0f;
                    floatingText.color = newColor;

                    textRectTransform.DOAnchorPos(textRectTransform.anchoredPosition + new Vector2(0, 100), 1.0f)
                   .SetEase(Ease.InSine);

                    floatingText.DOFade(0.0f, 1.0f).SetEase(Ease.InQuint).OnComplete(() =>
                    {
                        textRectTransform.anchoredPosition = boosterData.FloatingTextPosition;

                        floatingText.gameObject.SetActive(false);
                    });

                }
            }
        }
    }
}