using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WaterSortPuzzleGame.Enum;

namespace WaterSortPuzzleGame.MainMenu
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text coin_text;

        public GameObject Setting, Back, LevelText, CoinBar;

        [Header("Buttons")]
        [SerializeField] Button coinBoxClick;
        [SerializeField] Button backClick;
        [SerializeField] Button settingClick;

        private void Awake()
        {
            coinBoxClick.onClick.AddListener(OnAddCoin);
            backClick.onClick.AddListener(OnBack);
            settingClick.onClick.AddListener(OnSetting);
        }
        public void OnEnable()
        {
            EventManager.TopMenuIconActivation += TopMenuIconActivation;
            CoinManager.UpdateCoin += UpdateCoin;
            UpdateCoin();
        }
        public void OnDisable()
        {
            EventManager.TopMenuIconActivation -= TopMenuIconActivation;
            CoinManager.UpdateCoin -= UpdateCoin;
        }
        private void UpdateCoin()
        {
            coin_text.text = CoinManager.GoldCoin.ToString();
        }
        public void TopMenuIconActivation()
        {
            CoinBar.SetActive(true);
            LevelText.SetActive(false);
            if (BackHandler.RecentScreen.Count > 0)
            {
                var lastItem = BackHandler.RecentScreen[BackHandler.RecentScreen.Count - 1];
                if (lastItem.name == ScreenState.HomePanel)
                {
                    Setting.SetActive(true);
                    Back.SetActive(false);
                }
                else if (lastItem.name == ScreenState.CollectionPanel)
                {
                    Setting.SetActive(true);
                    Back.SetActive(true);
                }
                else if (lastItem.name == ScreenState.MapPanel)
                {
                    Setting.SetActive(true);
                    Back.SetActive(true);
                    LevelText.GetComponentInChildren<TextMeshProUGUI>().text = "Level Map";
                    LevelText.SetActive(true);
                }
                else if (lastItem.name == ScreenState.LevelPanel)
                {
                    Setting.SetActive(true);
                    Back.SetActive(true);
                    LevelText.SetActive(true);
                    LevelText.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
                }
                else if (lastItem.name == ScreenState.UnlockedBoosterPanel 
                    || lastItem.name == ScreenState.LoadingPanel
                    || lastItem.name == ScreenState.SkinUnlockedPanel)
                {
                    Setting.SetActive(false);
                    Back.SetActive(false);
                    CoinBar.SetActive(false);
                    LevelText.SetActive(false);
                }
                else if (lastItem.name == ScreenState.PurchaseBoosterPanel)
                {
                    Setting.SetActive(false);
                    Back.SetActive(true);
                    CoinBar.SetActive(false);
                    LevelText.SetActive(false);
                }
                else if (lastItem.name == ScreenState.LevelCompletePanel)
                {
                    Setting.SetActive(false);
                    Back.SetActive(false);
                    CoinBar.SetActive(false);
                    LevelText.SetActive(true);
                    LevelText.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(147, 137, 137, 255);
                }
                else if (lastItem.name == ScreenState.SuccessErrorPanel)
                {
                    CoinBar.SetActive(false);
                    Setting.SetActive(false);
                    Back.SetActive(true);
                }
                else if (lastItem.name == ScreenState.SettingPanel || lastItem.name == ScreenState.ShopPanel ||
                      lastItem.name == ScreenState.FreeCoinsPanel)
                {
                    Setting.SetActive(false);
                    Back.SetActive(true);
                }
                else if (lastItem.name == ScreenState.AppQuitPanel)
                {
                    Setting.SetActive(false);
                    Back.SetActive(true);
                    CoinBar.SetActive(false);
                }
                else
                {
                    Setting.SetActive(true);
                    Back.SetActive(true);
                }
            }
        }

        public void OnSetting()
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);

            // Hide CollectionPanel if it's displayed
            if (UIController.IsPageDisplayed<CollectionPanel>())
                UIController.HidePage<CollectionPanel>();

            // Show Settings
            UIController.ShowPage<SettingPanel>();
        }
        public void OnBack()
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
            BackHandler.BackEvent();
        }
        public void OnAddCoin()
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);

            if (UIController.IsPageDisplayed<CollectionPanel>())
            {
                UIController.HidePage<CollectionPanel>(); // show ShopPanel after hiding CollectionPanel
            }

            if (UIController.IsPageDisplayed<SettingPanel>())
            {
                UIController.HidePage<SettingPanel>(); // show ShopPanel after hiding SettingPanel
            }

            if (UIController.IsPageDisplayed<FreeCoinPanel>())
            {
                BackHandler.RemoveRecentlyScreen(UIController.GetPage<ShopPanel>()); // show ShopPanel after hiding FreeCoinPanel
            }
            else if (UIController.IsPageDisplayed<ShopPanel>())
            {
                BackHandler.RemoveRecentlyScreen(UIController.GetPage<FreeCoinPanel>()); // show FreeCoinPanel after hiding ShopPanel
            }
            else
            {
                UIController.ShowPage<ShopPanel>();
            }
        }
    }
}
