using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterSortPuzzleGame.MainMenu;

namespace WaterSortPuzzleGame
{
    public abstract class UIPage : MonoBehaviour
    {
        protected bool isPageDisplayed;
        public bool IsPageDisplayed { get => isPageDisplayed; set => isPageDisplayed = value; }

        private string defaultName;

        public void SetName()
        {
            defaultName = name;
        }
        public abstract void Init();

        public void ActivePage()
        {
            gameObject.SetActive(true);
            isPageDisplayed = true;
        }

        public void DeActivePage()
        {
            gameObject.SetActive(false);
            isPageDisplayed = false;
        }

        public abstract void PlayShowAnimation();
        public abstract void PlayHideAnimation();

        public virtual void OnPageResume() { }
        protected virtual void BackClick()
        {
            //MenuPanel.Instance.OnBack();

            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
            BackHandler.BackEvent();
        }
        protected virtual void BackgroundClick()
        {
            BackHandler.BackEvent();
           // MenuPanel.Instance.OnBackGroundClick();
        }
    }
}
