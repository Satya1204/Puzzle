using UnityEngine;
namespace WaterSortPuzzleGame
{
    public class IAPInitModule : MonoBehaviour
    {
        [SerializeField] IAPSettings settings;

        public void Init()
        {
            if (settings == null)
            {
                Debug.LogError("IAP Settings is not assigned in Init Settings", this);

                return;
            }

            IAPManager.Init(settings);
        }
    }
}
