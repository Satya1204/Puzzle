using WaterSortPuzzleGame.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WaterSortPuzzleGame.Enum;

namespace WaterSortPuzzleGame
{
    public static class IAPManager
    {
        public static bool IsInitialized { get; private set; } = false;
        public static event Action Initialized;
        public static event ProductCallback PurchaseCompleted;
        public static bool IsNoAdsPurchased
        {
            get => PlayerPrefs.GetInt("IAPProduct_NoAds", 0) == 1;
            set  {
                Debug.Log($"IAPProduct_NoAds: {value}");
                PlayerPrefs.SetInt("IAPProduct_NoAds", value ? 1 : 0); 
            }
                
        }
        private static IAPSettings settings;
        private static IAPWrapper wrapper;
        private static Dictionary<ProductKeyType, IAPItem> productsTypeToProductLink;
        public static void Init(IAPSettings setting)
        {
            if (IsInitialized)
            {
                return;
            }

            settings = setting;
            if (settings == null)
            {
                Debug.Log("[IAP Manager]: IAPSettings is null!");
                return;
            }

            productsTypeToProductLink = new Dictionary<ProductKeyType, IAPItem>();

            IAPItem[] items = settings.IAPItems;
            if (items != null)
            {
                foreach (IAPItem item in items)
                {
                    if (!productsTypeToProductLink.ContainsKey(item.ProductKeyType))
                    {
                        productsTypeToProductLink.Add(item.ProductKeyType, item);
                    }
                    else
                    {
                        Debug.LogWarning($"[IAP Manager]: Product with the type {item.ProductKeyType} has duplicates in the list!", settings);
                    }
                }
            }

#if UNITY_IAP
    wrapper = new UnityIAPWrapper();
#else
            wrapper = new DummyIAPWrapper();
            Debug.LogWarning("[IAP Manager]: Unity IAP not installed, using DummyIAPWrapper!");
#endif

            //wrapper = new UnityIAPWrapper();
            wrapper.Init(settings);
        }

        public static IAPItem GetIAPItem(string productID)
        {
            if (string.IsNullOrEmpty(productID)) return null;

            foreach (IAPItem item in productsTypeToProductLink.Values)
            {
                if (item.ID == productID)
                    return item;
            }

            return null;
        }

        public static IAPItem GetIAPItem(ProductKeyType productKeyType)
        {
            productsTypeToProductLink.TryGetValue(productKeyType, out IAPItem item);

            return item;
        }

        public static void BuyProduct(ProductKeyType productKeyType)
        {

            wrapper.BuyProduct(productKeyType);
        }

        public static ProductData GetProductData(ProductKeyType productKeyType)
        {
            var product = wrapper.GetProductData(productKeyType);

            if (product == null)
            {
                Debug.LogWarning($"[IAP Manager]: Product of type '{productKeyType}' was not found in Monetization Settings. Please ensure it is added to the products list.", settings);
            }

            return product;
        }

        public static void OnModuleInitialized()
        {
            IsInitialized = true;

            Initialized?.Invoke();

        }

        public static void OnPurchaseCompleted(ProductKeyType productKey, int quantity = 1)
        {
            if (UIController.GetPage<LoadingPanel>().IsPageDisplayed)
            {
                BackHandler.RemoveRecentlyScreen();
            }
            ApplyCosumableReward(productKey, quantity);
            PurchaseCompleted?.Invoke(productKey, quantity);
        }
        private static void ApplyCosumableReward(ProductKeyType productKey, int quantity = 1)
        {
            IAPItem item = GetIAPItem(productKey);
            if(item.ShopItem != null)
            {
                item.ShopItem.GetComponent<IAPItemHolder>().ApplyRewards(quantity);
            }
            else
            {
                Debug.Log("ShopItem is null in IAPSettings.");
            }
            
        }
        public static void OnPurchaseFailed()
        {
            if (UIController.GetPage<LoadingPanel>().IsPageDisplayed)
            {
                SuccessErrorPanel gameUI = UIController.GetPage<SuccessErrorPanel>();
                gameUI.SetData(ToasterState.PaymentFailed);

                BackHandler.RemoveRecentlyScreen(UIController.GetPage<SuccessErrorPanel>());
            }
            else
            {
                SuccessErrorPanel gameUI = UIController.GetPage<SuccessErrorPanel>();
                gameUI.SetData(ToasterState.PaymentFailed);

                UIController.ShowPage<SuccessErrorPanel>();
            }
        }
        public static IEnumerable<IAPItem> GetAllProducts()
        {
            if (productsTypeToProductLink == null) return Enumerable.Empty<IAPItem>();
            return productsTypeToProductLink.Values;
        }

        public delegate void ProductCallback(ProductKeyType productKeyType,int quantity);
        //public delegate void ProductFailCallback(ProductKeyType productKeyType, PurchaseFailureReason failureReason);
    }
}