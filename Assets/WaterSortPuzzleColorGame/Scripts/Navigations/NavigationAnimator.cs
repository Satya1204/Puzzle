using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace WaterSortPuzzleGame.Navigations
{
    public static class NavigationAnimator
    {
        public static void AnimateSelection(NavigationItem item, bool isSelected, float originalIconY,float originalTextY)
        {
            if (item == null || item.icon == null || item.text == null)
            {
                Debug.LogWarning("NavigationItem is missing icon or text references.");
                return;
            }

            // Highlight toggle
            var highlight = item.button.GetComponent<Image>();
            if (highlight != null)
                highlight.enabled = isSelected;


            float iconY = isSelected ? originalIconY + 70f : originalIconY;
            float textY = isSelected ? originalTextY + 20f : originalTextY;
            float iconScale = isSelected ? 1.5f : 1f;
            float textScale = isSelected ? 1.2f : 1f;

            item.icon.DOScale(iconScale, 0.3f).SetEase(Ease.OutBack);
            item.icon.DOLocalMoveY(iconY, 0.3f).SetEase(Ease.OutBack);
            item.text.DOLocalMoveY(textY, 0.3f).SetEase(Ease.OutBack);
            item.text.DOScale(textScale, 0.3f).SetEase(Ease.OutBack);

        }
    }
}
