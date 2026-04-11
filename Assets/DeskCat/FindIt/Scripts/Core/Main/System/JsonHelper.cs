using System.IO;
using UnityEngine;

namespace DeskCat.FindIt.Scripts.Core.Main.System
{
    public static class JsonHelper
    {
        public static void SaveToJson<T>(T objectToSave, string fileName)
        {
            string json = JsonUtility.ToJson(objectToSave, true);
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(filePath, json);
        }

        public static T LoadFromJson<T>(string fileName) where T : new()
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<T>(json);
            }

            return new T();
        }
    }
}