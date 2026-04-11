using UnityEngine;
namespace WaterSortPuzzleGame
{
    [CreateAssetMenu(fileName = "IAP Setting", menuName = "Content/IAP Setting")]
    public class IAPSettings : ScriptableObject
    {
       
        [Header("IAP")]
        [SerializeField] IAPItem[] iapItems;
        public IAPItem[] IAPItems => iapItems;
    }
}