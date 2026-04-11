using DG.DOTweenEditor;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    [InitializeOnLoad]
    public static class AutoInitializerLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void LoadMain()
        {
            if (!CoreEditor.AutoLoadInitializer) return;

            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene != null)
            {
                if (currentScene.name != CoreEditor.InitSceneName)
                {
#if UNITY_6000
                    InitSceneLoader initializer = Object.FindFirstObjectByType<InitSceneLoader>();
#else
                    InitSceneLoader initializer = Object.FindObjectOfType<InitSceneLoader>();
#endif

                    if (initializer == null)
                    {
                        GameObject initializerPrefab = GetAsset<GameObject>("InitSceneLoader");
                        if (initializerPrefab != null)
                        {
                            GameObject InitializerObject = Object.Instantiate(initializerPrefab);

                            initializer = InitializerObject.GetComponent<InitSceneLoader>();
                            initializer.Awake();
                        }
                        else
                        {
                            Debug.LogError("[Game]: InitSceneLoader prefab is missing!");
                        }
                    }
                }
            }
        }

        public static T GetAsset<T>(string name = "") where T : Object
        {
            string[] assets = AssetDatabase.FindAssets((string.IsNullOrEmpty(name) ? "" : name + " ") + "t:" + typeof(T).Name);
            if (assets.Length > 0)
            {
                return (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), typeof(T));
            }

            return null;
        }
    }
}