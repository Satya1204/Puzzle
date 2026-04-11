using UnityEngine;
#if UNITY_IAP
using UnityEngine.Purchasing;
#endif
namespace WaterSortPuzzleGame
{
    public class ProductData
    {
        public ProductType ProductType { get; }

        public decimal Price { get; }
        public string ISOCurrencyCode { get; }

#if UNITY_IAP
        public Product Product { get; }
#endif
        public ProductData()
        {
            Price = 0.00m;
            ISOCurrencyCode = "USD";
        }
        public ProductData(string defaultPrice, string currency = "USD", ProductType productType = ProductType.Consumable)
        {
            decimal price = 0;
            decimal.TryParse(defaultPrice, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out price);

            Price = price;
            ISOCurrencyCode = currency;
            ProductType = productType;
        }

        public string GetLocalPrice()
        {
            return string.Format("{0} {1}", ISOCurrencyCode, Price);
        }
#if UNITY_IAP
        public ProductData(Product product)
        {
            Product = product;

            ProductType = (ProductType)product.definition.type;

            Price = product.metadata.localizedPrice;
            ISOCurrencyCode = product.metadata.isoCurrencyCode;
        }
#endif
    }
}