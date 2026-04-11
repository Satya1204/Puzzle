using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    public class ScreenResizeManager : MonoBehaviour
    {
        public static Action OnScreenSizeChanged;

        private int lastScreenWidth, lastScreenHeight;

        void Start()
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            OnScreenSizeChanged?.Invoke();
        }

        void Update()
        {
            if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            {
                lastScreenWidth = Screen.width;
                lastScreenHeight = Screen.height;

                OnScreenSizeChanged?.Invoke(); 
            }
        }
    }
}
