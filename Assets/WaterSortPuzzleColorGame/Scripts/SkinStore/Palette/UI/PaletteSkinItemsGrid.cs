using UnityEngine;

namespace WaterSortPuzzleGame
{
    public class PaletteSkinItemsGrid : MonoBehaviour
    {
        [SerializeField] private Transform contentParent; // The GridLayoutGroup holder
        [SerializeField] private PaletteSkinItem paletteItemPrefab;

        public void Init()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            // Access PaletteSkinController through SkinManager
            var paletteSkinController = SkinManager.Instance.PaletteSkinController;
            var skins = paletteSkinController.PaletteSkinDatabase.skins;

            foreach (var skin in skins)
            {
                PaletteSkinItem item = Instantiate(paletteItemPrefab, contentParent);
                bool isSelected = paletteSkinController.SelectedSkinPaletteId == skin.id;
                item.Init(skin, isSelected);
            }
        }
    }
}
