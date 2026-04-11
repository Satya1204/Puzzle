using UnityEngine;

namespace WaterSortPuzzleGame
{
    public class TubeSkinItemsGrid : MonoBehaviour
    {
        [SerializeField] private Transform contentParent; // The GridLayoutGroup holder
        [SerializeField] private TubeSkinItem tubeItemPrefab;

        public void Init()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            // Access TubesSkinController through SkinManager
            var tubeController = SkinManager.Instance.TubesSkinController;
            var skins = tubeController.TubeSkinDatabase.skins;

            foreach (var skin in skins)
            {
                TubeSkinItem item = Instantiate(tubeItemPrefab, contentParent);
                bool isSelected = tubeController.SelectedSkinTubeId == skin.id;
                item.Init(skin, isSelected);
            }
        }
    }
}
