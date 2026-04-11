using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WaterSortPuzzleGame
{
    public class InitSceneLoader : MonoBehaviour
    {
        [SerializeField] private string[] scenesToLoad = { "Menu", "LevelGenerator" };

        private static bool loaded = false;

        public void Awake()
        {
            // Only run once
            if (loaded) return;
            loaded = true;

            DontDestroyOnLoad(gameObject);

            Scene currentScene = SceneManager.GetActiveScene();

            foreach (string sceneName in scenesToLoad)
            {
                // Skip if already loaded or is the active scene
                if (sceneName == currentScene.name) continue;

                if (!SceneManager.GetSceneByName(sceneName).isLoaded)
                {
                    SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                }
            }
        }
    }
}
