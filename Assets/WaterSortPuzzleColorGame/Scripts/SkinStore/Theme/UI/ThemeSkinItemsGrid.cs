using UnityEngine;

namespace WaterSortPuzzleGame
{
    public class ThemeSkinItemsGrid : MonoBehaviour
    {
        [SerializeField] private Transform contentParent; // The GridLayoutGroup holder
        [SerializeField] private ThemeSkinItem themeItemPrefab;

        public void Init()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            // Access ThemeSkinController through SkinManager
            var themeController = SkinManager.Instance.ThemeSkinController;
            var skins = themeController.ThemeSkinDatabase.skins;

            foreach (var skin in skins)
            {
                ThemeSkinItem item = Instantiate(themeItemPrefab, contentParent);
                bool isSelected = themeController.SelectedSkinThemeId == skin.id;
                item.Init(skin, isSelected);
            }
        }
    }
}
