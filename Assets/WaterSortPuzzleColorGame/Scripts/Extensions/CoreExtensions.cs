using UnityEngine;

namespace WaterSortPuzzleGame
{
    public static class CoreExtensions
    {
        public static bool CacheComponent<T>(this GameObject gameObject, out T component) where T : Component
        {
            Component unboxedComponent = gameObject.GetComponent(typeof(T));

            if (unboxedComponent != null)
            {
                component = (T)unboxedComponent;

                return true;
            }

            Debug.LogError($"{gameObject.name} doesn't have {typeof(T)} script added to it", gameObject);

            component = null;

            return false;
        }
    }
}