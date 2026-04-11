using UnityEngine.UI;
using WaterSortPuzzleGame.Enum;

namespace WaterSortPuzzleGame
{
    public static class NavigationBarUtils
    {
       
        public static void HideAllPanels()
        {

            UIController.HidePage<HomePanel>();
            UIController.HidePage<MapPanel>();
            UIController.HidePage<CollectionPanel>();
        }

        public static void ShowPanel(NavigationType type)
        {
            switch (type)
            {
                case NavigationType.Home:
                    UIController.ShowPage<HomePanel>();
                    break;
                case NavigationType.Collection:
                    UIController.ShowPage<CollectionPanel>();
                    break;
                case NavigationType.Map:
                    UIController.ShowPage<MapPanel>();
                    break;
                default:
                    break;
            }
        }
    }
}