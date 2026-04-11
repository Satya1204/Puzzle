using WaterSortPuzzleGame.BottleCodes;
using UnityEngine;

namespace WaterSortPuzzleGame.LevelScripts
{
    public class LevelParent : MonoBehaviour
    {
        [Space(20)]
        public int NumberOfColor;

        private void Start()
        {
            GameManager.Instance.TotalColorAmount = NumberOfColor;
            var gm = GameManager.Instance;
            gm.bottleControllers.Clear();

            AddControllersToTheControllerList(gm);
        }

        private void AddControllersToTheControllerList(GameManager gm)
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out BottleController controller))
                {
                    gm.bottleControllers.Add(controller);
                }
            }
        }
    }
}