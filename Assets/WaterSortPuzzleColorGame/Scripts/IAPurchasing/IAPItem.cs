using UnityEngine;

namespace WaterSortPuzzleGame
{
    [System.Serializable]
    public class IAPItem
    {
        [SerializeField] string androidID;
        [SerializeField] ProductKeyType productKeyType;
        [SerializeField] ProductType productType;
        [Tooltip("Default Price In USD. Example: 5")]
        [SerializeField] string price;
        [SerializeField] GameObject shopItem;
        [SerializeField] ShopUIType shopUIType;
        
        public string Price => price;

        public string ID
        {
            get
            {
#if UNITY_ANDROID
                return androidID;
#else
                return string.Format("unknown_platform_{0}", productKeyType);
#endif
            }
        }
        public ProductKeyType ProductKeyType { get => productKeyType; set => productKeyType = value; }
        public ProductType ProductType { get => productType; set => productType = value; }
        public GameObject ShopItem => shopItem;
        public ShopUIType ShopUIType => shopUIType;

    }
}
