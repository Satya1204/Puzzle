
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterSortPuzzleGame.Enum;

namespace WaterSortPuzzleGame
{
    public class UIController : MonoBehaviour
    {
        private static UIController instance;

        private static List<UIPage> pages;
        private static Dictionary<Type, UIPage> pagesLink = new Dictionary<Type, UIPage>();

        public void Init()
        {
            instance = this;

            pages = new List<UIPage>();
            pagesLink = new Dictionary<Type, UIPage>();
            for (int i = 0; i < transform.childCount; i++)
            {
                UIPage uiPage = transform.GetChild(i).GetComponent<UIPage>();
                if (uiPage != null)
                {
                    uiPage.SetName();

                    if (pagesLink.ContainsKey(uiPage.GetType()))
                    {
                        Debug.LogError($"[UI Controller] Page {uiPage.GetType()} is already added to the Popup Utils. Please remove the duplicate object to resolve this issue.", uiPage);

                        continue;
                    }

                    pagesLink.Add(uiPage.GetType(), uiPage);

                    pages.Add(uiPage);
                    uiPage.Init();
                }
            }
        }

        public static void ShowPage<T>(Action onPageOpened = null) where T : UIPage
        {
            Type pageType = typeof(T);
            UIPage page = pagesLink[pageType];
            GameManager.gameState = ((page is LevelPanel) ? GameState.Play : GameState.Pause);
            GameManager.Instance.SetRaycastBlocker(false);
            if (!page.IsPageDisplayed)
            {
                page.OnPageResume();
                page.ActivePage();
                page.PlayShowAnimation();
                BackHandler.AddRecentlyScreen(page.name, page.gameObject);
                onPageOpened?.Invoke();
            }

        }

        public static void HidePage<T>(Action onPageClosed = null)
        {
            GameManager.gameState = GameState.Play;
            Type pageType = typeof(T);
            UIPage page = pagesLink[pageType];
            if (page.IsPageDisplayed)
            {
                page.DeActivePage();
                page.PlayHideAnimation();
                onPageClosed?.Invoke();

            }
        }
        public static void ShowPageByType(Type pageType)
        {
            GameManager.Instance.SetRaycastBlocker(false);
            if (pagesLink.TryGetValue(pageType, out var page) && !page.IsPageDisplayed)
            {
                GameManager.gameState = ((page is LevelPanel) ? GameState.Play : GameState.Pause);
                page.OnPageResume();
                page.ActivePage();
                page.PlayShowAnimation();
                BackHandler.AddRecentlyScreen(page.name, page.gameObject);
            }
        }
        public static void HidePageByType(Type pageType, Action onPageClosed = null)
        {
            GameManager.gameState = GameState.Play;
            if (pagesLink.TryGetValue(pageType, out var page) && page.IsPageDisplayed)
            {
                page.DeActivePage();
                page.PlayHideAnimation();
                onPageClosed?.Invoke();
            }
        }
        public static T GetPage<T>() where T : UIPage
        {
            UIPage page;

            if (pagesLink.TryGetValue(typeof(T), out page))
                return (T)page;

            return null;
        }


        public static bool IsPageDisplayed<T>() where T : UIPage
        {
            return TryGetPage<T>(out var page) && page.IsPageDisplayed;
        }

        public static bool TryGetPage<T>(out T page) where T : UIPage
        {
            page = null;
            if (pagesLink.TryGetValue(typeof(T), out var uiPage))
            {
                page = uiPage as T;
                return true;
            }
            return false;
        }
        public static Type GetPageTypeFromGameObject(GameObject go)
        {
            foreach (var kvp in pagesLink)
            {
                if (kvp.Value.gameObject == go)
                {
                    return kvp.Key;
                }
            }
            return null;
        }

    }
}
