using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaterSortPuzzleGame
{
    public class AppQuitPanel : UIPage
    {
        [SerializeField] private ScaleAnimation animPopup;
        [SerializeField] private Button appQuitBack;
        [SerializeField] private Button appQuitClose;
        [SerializeField] private Button appQuitYes;
        [SerializeField] private Button appQuitNo;
        public override void Init()
        {
            appQuitBack.onClick.AddListener(BackgroundClick);
            appQuitClose.onClick.AddListener(BackClick);
            appQuitYes.onClick.AddListener(QuitApp);
            appQuitNo.onClick.AddListener(BackClick);
        }
        public override void PlayShowAnimation()
        {
            animPopup.Show();
        }
        public override void PlayHideAnimation()
        {
            animPopup.Hide();
        }
        public void QuitApp()
        {
            Application.Quit();
        }
    }
}
