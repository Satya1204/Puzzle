using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WaterSortPuzzleGame.Enum;

namespace WaterSortPuzzleGame.Navigations
{
    [System.Serializable]
    public class NavigationItem
    {
        public NavigationType type;
        public Button button;         // Button reference
        public RectTransform icon;    // Icon reference
        public RectTransform text;    // Text reference
    }

}
