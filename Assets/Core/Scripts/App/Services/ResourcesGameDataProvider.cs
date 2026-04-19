using System;
using UnityEngine;

namespace PuzzleApp.App.Services
{
    public sealed class ResourcesGameDataProvider : IGameDataProvider
    {
        public T LoadJson<T>(string resourcePath) where T : class
        {
            var textAsset = Resources.Load<TextAsset>(resourcePath);
            if (textAsset == null)
            {
                Debug.LogError($"ResourcesGameDataProvider: '{resourcePath}' not found in Resources.");
                return null;
            }

            try
            {
                return JsonUtility.FromJson<T>(textAsset.text);
            }
            catch (Exception e)
            {
                Debug.LogError($"ResourcesGameDataProvider: JSON parse error for '{resourcePath}': {e.Message}");
                return null;
            }
        }
    }
}
