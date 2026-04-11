using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterSortPuzzleGame.DataClass;
using WaterSortPuzzleGame.Enum;

namespace WaterSortPuzzleGame
{
    public static class BackHandler
    {
        public static List<RecentlyAddedScreen> RecentScreen = new List<RecentlyAddedScreen>();

        public static void BackEvent()
        {
            if (GameManager.gameState == GameState.Win) return;

            if (UIController.IsPageDisplayed<SuccessErrorPanel>()) // Prioritize this
            {
                RemoveRecentlyScreen();
            }
            else if (UIController.IsPageDisplayed<LevelCompletePanel>()
                || UIController.IsPageDisplayed<UnlockedBoosterPanel>()
                || UIController.IsPageDisplayed<LoadingPanel>()) // Then check this
            {
                return;
            }
            else if (UIController.IsPageDisplayed<ShopPanel>())
            {
                RemoveRecentlyScreen(UIController.GetPage<FreeCoinPanel>());
            }
            else
            {
                RemoveRecentlyScreen();
            }
            
        }

        public static void AddRecentlyScreen(string name, GameObject gameObject)
        {

            var data = new RecentlyAddedScreen { name = name, gameObject = gameObject };

            if (RecentScreen.Count > 0)
            {
                if (RecentScreen.Find(x => x.name == ScreenState.SettingPanel || x.name == ScreenState.CollectionPanel) != null
                    && name == ScreenState.ShopPanel)
                {
                    RecentScreen.RemoveAll(x => x.name == ScreenState.CollectionPanel || x.name == ScreenState.SettingPanel);
                }
            }

            var isExist = RecentScreen.Find(x => x.name == name);
            if (isExist != null)
            {
                if (isExist.name == ScreenState.HomePanel)
                    RecentScreen.Clear();
                else
                    RecentScreen.Remove(isExist);
            }

            RecentScreen.Add(data);
            EventManager.TopMenuIconActivation?.Invoke();

        }
        public static void RemoveRecentlyScreen(UIPage pageToShowAfterHide = null)
        {
            if (RecentScreen.Count > 1)
            {
                var lastToSecondItem = RecentScreen[RecentScreen.Count - 2];
                var lastItem = RecentScreen[RecentScreen.Count - 1];
                RecentScreen.RemoveAt(RecentScreen.Count - 1);

                if (lastItem.name == ScreenState.LevelPanel)
                {
                    UIController.HidePage<LevelPanel>();
                    GameManager.IsLevelScreen = false;
                    EventManager.TopMenuIconActivation?.Invoke();

                    if (lastToSecondItem.name == ScreenState.MapPanel)
                        EventManager.LoadMapLevels?.Invoke();

                    return;

                }
                if (lastItem.gameObject.TryGetComponent<UIPage>(out var lastPage))
                {

                    Type type = UIController.GetPageTypeFromGameObject(lastItem.gameObject);

                    if (type != null)
                    {
                        UIController.HidePageByType(type, () =>
                        {
                            if (pageToShowAfterHide != null)
                            {
                                var type = UIController.GetPageTypeFromGameObject(pageToShowAfterHide.gameObject);
                                if (type != null)
                                    UIController.ShowPageByType(type);
                            }
                        });
                    }
                }
                else
                {
                    lastItem.gameObject.SetActive(false);
                }

                if (pageToShowAfterHide == null)
                    ShowLastToSecond(lastToSecondItem);
                

            }
            else if (RecentScreen.Count == 1 && RecentScreen[0].name == ScreenState.HomePanel)
            {
                UIController.ShowPage<AppQuitPanel>();
            }
            EventManager.TopMenuIconActivation?.Invoke();
        }
        private static void ShowLastToSecond(RecentlyAddedScreen lastToSecondItem)
        {
            if (lastToSecondItem.gameObject != null)
            {
                if (lastToSecondItem.gameObject.TryGetComponent<UIPage>(out var page))
                {
                    Type type = UIController.GetPageTypeFromGameObject(lastToSecondItem.gameObject);

                    if (type != null)
                    {
                        UIController.ShowPageByType(type); // Registered in PopupUtils
                    }
                }
                else
                {
                    lastToSecondItem.gameObject.SetActive(true); // Fallback
                }
            }
        }
        public static void RemoveAllAboveLevelPanel(Action onAllClosed = null)
        {
            // Start from the last shown page (top of stack)
            for (int i = RecentScreen.Count - 1; i >= 0; i--)
            {
                var item = RecentScreen[i];

                // Stop when we hit LevelPanel
                if (item.name == ScreenState.LevelPanel)
                    break;

                // Remove from stack
                RecentScreen.RemoveAt(i);

                // Hide the page properly
                if (item.gameObject.TryGetComponent<UIPage>(out var page))
                {
                    Type type = UIController.GetPageTypeFromGameObject(item.gameObject);
                    if (type != null)
                        UIController.HidePageByType(type);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }

            BoosterController.DeselectBooster();
            onAllClosed?.Invoke();
        }
    }
}
