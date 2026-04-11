using System;

namespace WaterSortPuzzleGame
{
    [Serializable]
    public class IAPPayload
    {
        public string json;
        public string signature;
        public IAPPayloadData payloadData;
    }
}
