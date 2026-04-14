#undef UNITY_IAP
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using WaterSortPuzzleGame.Enum;

#if UNITY_IAP
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine.Purchasing;

namespace WaterSortPuzzleGame
{
    public class UnityIAPWrapper : IAPWrapper
    {

        public static StoreController storeController { get; private set; }

    /// <summary>
    /// Initializes the IAP system with the provided settings.
    /// </summary>
    /// <param name="settings">The IAP settings to use for initialization.</param>
        public override async Task Init(IAPSettings settings)
        {
            try
            {
              //  Debug.Log($"[UnityIAPWrapper]: Start Initialization");
                var options = new InitializationOptions().SetEnvironmentName("production");

                await UnityServices.InitializeAsync(options);


                // v5: get a controller instance
                storeController = UnityIAPServices.StoreController();

                // Attach handlers BEFORE connecting

                storeController.OnStoreDisconnected += OnStoreDisconnected;
                storeController.OnProductsFetched += OnProductsFetched;
                storeController.OnProductsFetchFailed += OnProductsFetchFailed;
                storeController.OnPurchasesFetched += OnPurchasesFetched;
                storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
                storeController.OnPurchasePending += OnPurchasePending;
                storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
                storeController.OnPurchaseFailed += OnPurchaseFailed;
                storeController.OnPurchaseDeferred += OnPurchaseDeferred;

                RegisterEntitlementCallback();
                // Connect to the current store
                await storeController.Connect();

                // Initialize products
                var initialProductsToFetch = BuildProductDefinitions(settings);
                storeController.FetchProducts(initialProductsToFetch);

            }
            catch (System.Exception exception)
            {
                Debug.Log($"[UnityIAPWrapper]: Initialization failed with exception: {exception}");
            }
        }

        private void RegisterEntitlementCallback()
        {
            storeController.OnCheckEntitlement += (result) =>
            {
                Product product = result.Product;     // the Product being checked
                var status = result.Status;

                Debug.Log($"[UNITYIAP]Product:{product}, Entitile Status: {status}");
                bool isEntitled = status == EntitlementStatus.FullyEntitled;

                //Only Fore Non-Consumable Product (Ads)
                if (isEntitled)
                {
                    IAPItem item = IAPManager.GetIAPItem(product.definition.id);
                    IAPManager.OnPurchaseCompleted(item.ProductKeyType, 0);

                }
            };
        }
        public void CheckEntitlementForAllProducts()
        {
            foreach (var product in storeController.GetProducts())
            {
                storeController.CheckEntitlement(product); // triggers the callback above
            }
        }
        
        private List<ProductDefinition> BuildProductDefinitions(IAPSettings settings)
        {
            var initialProductsToFetch = new List<ProductDefinition>();
            IAPItem[] items = settings.IAPItems;
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.ID))
                    initialProductsToFetch.Add(new ProductDefinition(item.ID, (UnityEngine.Purchasing.ProductType)item.ProductType));
                else
                    Debug.LogWarning($"[UnityIAPWrapper] {item.ProductType} has no ID configured.");
            }
            return initialProductsToFetch;
            
        }
        private void OnProductsFetched(List<Product> products)
        {
            
            // Products are ready; now fetch purchases
            storeController.FetchPurchases();
        }
        private void OnPurchasesFetched(Orders orders)
        {
            
            CheckEntitlementForAllProducts();
            IAPManager.OnModuleInitialized();
        }
        
        private void OnProductsFetchFailed(ProductFetchFailed reason)
        {
            Debug.Log($"[UnityIAPWrapper] Products fetch failed: {reason}");
        }
        private void OnPurchaseDeferred(DeferredOrder deferredOrder)
        {
            Debug.Log($"[UnityIAPWrapper] Purchase deferred for product: {deferredOrder?.Info}");
            // Optional: Show UI like "Purchase pending approval"
        }
        private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription reason)
        {
            Debug.Log($"[UnityIAPWrapper] Purchases fetch failed: {reason}");
        }
        private void OnPurchaseConfirmed(Order order)
        {
            Debug.Log($"[UnityIAPWrapper] Purchases confirmed: {order}");

            if (order?.Info?.PurchasedProductInfo != null && order.Info.PurchasedProductInfo.Count > 0)
            {
                IAPItem item = IAPManager.GetIAPItem(order.Info.PurchasedProductInfo[0].productId);
                int quantity = 1;

                string receipt = order.Info.Receipt;
                if (!string.IsNullOrEmpty(receipt))
                {
                    var payData = JsonUtility.FromJson<IAPPayData>(receipt);
                    if (payData.Store != "fake")
                    {
                        var payload = JsonUtility.FromJson<IAPPayload>(payData.Payload);
                        var payloadData = JsonUtility.FromJson<IAPPayloadData>(payload.json);
                        quantity = payloadData.quantity;
                    }
                }

                if (item != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.purchaseComplete);
                    IAPManager.OnPurchaseCompleted(item.ProductKeyType, quantity);
                }
                return;
            }
            else
            {
                Debug.Log("[IAPManager] No product info available.");
            }
            
        }
        private void OnStoreDisconnected(StoreConnectionFailureDescription desc)
        {
            Debug.Log($"[IAPManager]: Initialization/connection failed Message: {desc.Message}");
        }
        private void OnPurchaseFailed(FailedOrder failedOrder)
        {
            if (failedOrder?.Info?.PurchasedProductInfo == null || failedOrder.Info.PurchasedProductInfo.Count == 0)
            {
                Debug.Log("[IAPManager] Purchase failed but no product info available.");
                IAPManager.OnPurchaseFailed();
                return;
            }

            var productId = failedOrder.Info.PurchasedProductInfo[0].productId;
            var reason = failedOrder.FailureReason;     // enum: PurchaseFailureReason
            var message = failedOrder.Details;    // string message from store (extra info)


            IAPManager.OnPurchaseFailed();

            Debug.Log($"[IAPManager] Purchase failed. Product: {productId}, Reason: {reason}, Message: {message}");
        }
        private void OnPurchasePending(PendingOrder order)
        {
            
            Debug.Log($"[UnityIAPWrapper] Purchases Pending: {order}");
            storeController.ConfirmPurchase(order);
        }
       
        /// <summary>
        /// Initiates the purchase of a product.
        /// </summary>
        /// <param name="productKeyType">The key type of the product to purchase.</param>
        public override void BuyProduct(ProductKeyType productKeyType)
        {
            if (!IAPManager.IsInitialized)
            {
                Debug.LogWarning("[UnityIAPWrapper]: The IAP module is not initialized!");
                SuccessErrorPanel gameUI = UIController.GetPage<SuccessErrorPanel>();
                gameUI.SetData(ToasterState.NetworkError);

                UIController.ShowPage<SuccessErrorPanel>();
                return;
            }
            UIController.ShowPage<LoadingPanel>();

            IAPItem item = IAPManager.GetIAPItem(productKeyType);
            if (item != null)
            {
                storeController.PurchaseProduct(item.ID);
            }
        }

        /// <summary>
        /// Gets the product data for a specified product key type.
        /// </summary>
        /// <param name="productKeyType">The key type of the product.</param>
        /// <returns>The product data.</returns>
        public override ProductData GetProductData(ProductKeyType productKeyType)
        {
            if (!IAPManager.IsInitialized)
            {
                IAPItem fallbackItem = IAPManager.GetIAPItem(productKeyType);
                if (fallbackItem != null)
                {
                    return new ProductData(fallbackItem.Price, "USD"); // or item.CurrencyCode if you have that
                }

                return new ProductData(); // fallback zero price if no IAPItem found
            }


            IAPItem item = IAPManager.GetIAPItem(productKeyType);

            Product product = storeController.GetProducts().FirstOrDefault(p => p.definition.id == item.ID);

            if (product != null)
            {
                return new ProductData(product);
            }

            return null;
        }
        public void RestoreAllPurchases()
        {
            storeController.RestoreTransactions((success, error) =>
            {
                if (success)
                {
                    Debug.Log("[IAP Manager] All previous purchases restored.");
                    // OnCheckEntitlement is automatically called for each owned product
                }
                else
                {
                    Debug.LogWarning("[IAP Manager] Restore failed: " + error);
                }
            });
        }

    }
}

#endif