using UnityEngine;
using TMPro;
namespace WaterSortPuzzleGame
{
    public static class DevPanelEnabler
    {
        public static bool IsDevPanelDisplayed()
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
        public static void RegisterPanel(GameObject panel)
        {
            panel.SetActive(IsDevPanelDisplayed());
        }
    }
}
