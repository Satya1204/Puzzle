using System.Threading.Tasks;

namespace WaterSortPuzzleGame
{
    public abstract class IAPWrapper
    {
        public abstract Task Init(IAPSettings settings);
       // public abstract void RestorePurchases();
        public abstract void BuyProduct(ProductKeyType productKeyType);
        public abstract ProductData GetProductData(ProductKeyType productKeyType);
    }
}