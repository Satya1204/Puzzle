using UnityEngine;
namespace WaterSortPuzzleGame
{
    public sealed class IAPItemHolder : RewardsHolder
    {
        [Header("IAP Settings")]
        [SerializeField] ProductKeyType productKey;

        [Space]
        [Header("IAP Button")]
        [SerializeField] IAPButton purchaseButton;

       
        private ProductData product;
        private void OnEnable()
        {
            if (IAPManager.IsInitialized)
            {
                OnIAPManagerLoaded(); // Already initialized — call immediately
            }
            else
            {
                IAPManager.Initialized += OnIAPManagerLoaded; // Wait for initialization
            }

            // Subscribe to purchase callback
            IAPManager.PurchaseCompleted += OnPurchaseComplete;
        }

        private void OnDisable()
        {
            // Unsubscribe from purchase callback
            IAPManager.Initialized -= OnIAPManagerLoaded;
            IAPManager.PurchaseCompleted -= OnPurchaseComplete;
        }

        private void Awake()
        {
            InitializeComponents();
            // Link the product type to purchase button
            purchaseButton.Init(productKey);
        }
       
        private void OnIAPManagerLoaded()
        {
           
            // Get product data
            product = IAPManager.GetProductData(productKey);

            // Update button state
            purchaseButton.UpdateState(product);

            if (product.ProductType == ProductType.NonConsumable && IAPManager.IsNoAdsPurchased)
            {
                purchaseButton.PurchaseState();
                return;
                
            }
        }

        private void OnPurchaseComplete(ProductKeyType key, int quantity)
        {
            // Check if the purchased product type is equal to items's product type
            if (productKey == key)
            {
                ApplyAnimation(quantity);
                // Disable holder if it's an one time purchase (non-consumable) product 
                if (product.ProductType == ProductType.NonConsumable)
                {
                    IAPManager.IsNoAdsPurchased = true;
                    purchaseButton.PurchaseState();
                }
                
            }
        }
       
    }
}