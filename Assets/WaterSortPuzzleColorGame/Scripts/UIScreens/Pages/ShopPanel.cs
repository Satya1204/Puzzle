using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WaterSortPuzzleGame.DataClass;

namespace WaterSortPuzzleGame
{
    public class ShopPanel : UIPage
    {
        [SerializeField] private Transform popupTransform;
        [SerializeField] private Button shopBack;
        [SerializeField] private Transform shopContent;

        private MoveAnimation animPopup;
        public override void Init()
        {
            shopBack.onClick.AddListener(BackgroundClick);
            InitTransfrom();
            InstantiateShopItems();
        }
        public void InitTransfrom()
        {
            animPopup = new MoveAnimation(popupTransform);
        }
        public override void OnPageResume()
        {
            if (shopContent.childCount == 0)
            {
                InstantiateShopItems();
            }
        }
        public override void PlayShowAnimation()
        {
            AdsManager.Instance.HideBannerAd();
            animPopup.Show();
        }
        public override void PlayHideAnimation()
        {
            if(GameManager.IsLevelScreen) AdsManager.Instance.ShowBannerAd();

            animPopup.Hide();
        }
        private void InstantiateShopItems()
        {
            foreach (Transform child in shopContent)
            {
                Destroy(child.gameObject); // clear old items
            }

            foreach (var iapItem in IAPManager.GetAllProducts())
            {
                if (iapItem.ShopItem == null)
                {
                    Debug.LogWarning($"Shop item prefab missing for {iapItem}");
                    continue;
                }
                // Create UI prefab
                var shopGO = Instantiate(iapItem.ShopItem, shopContent);
            }
        }

    }
}
