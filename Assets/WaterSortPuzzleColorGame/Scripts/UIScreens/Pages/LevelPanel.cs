using UnityEngine;
namespace WaterSortPuzzleGame
{
    public class LevelPanel : UIPage
    {
        [SerializeField] private BoosterUIController boosterUIController;
        public BoosterUIController BoosterUIController => boosterUIController;

        public override void Init()
        {
            boosterUIController.Init();
        }
        public override void PlayShowAnimation()
        {
            boosterUIController.Show();
        }
        public override void PlayHideAnimation()
        {
            boosterUIController.Hide();
        }
    }
}