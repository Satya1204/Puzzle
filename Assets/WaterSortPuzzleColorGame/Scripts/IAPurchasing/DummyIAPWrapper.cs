using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    public class DummyIAPWrapper : IAPWrapper
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override async Task Init(IAPSettings settings)
        {
            IAPManager.OnModuleInitialized();
            await Task.CompletedTask;
        }
        public override void BuyProduct(ProductKeyType productKeyType)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.purchaseComplete);
            IAPManager.OnPurchaseCompleted(productKeyType, 1);
        }
        public override ProductData GetProductData(ProductKeyType productKeyType)
        {
            IAPItem item = IAPManager.GetIAPItem(productKeyType);
            if (item != null)
            {
                return new ProductData(item.Price, "USD", item.ProductType); // or item.CurrencyCode if you have that
            }

            return new ProductData(); // fallback zero price if no IAPItem found
        }
    }
}