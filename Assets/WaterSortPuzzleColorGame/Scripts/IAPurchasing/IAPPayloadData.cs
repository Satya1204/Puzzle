using System;

namespace WaterSortPuzzleGame
{
    [Serializable]
    public class IAPPayloadData
    {
        public string orderId;
        public string packageName;
        public string productId;
        public long purchaseTime;
        public int purchaseState;
        public string purchaseToken;
        public int quantity;
        public bool acknowledged;
    }
}