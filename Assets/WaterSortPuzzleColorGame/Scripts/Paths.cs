using System.IO;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    public static class Paths
    {
        public static string LevelHolderPath =
            Path.Combine(Application.persistentDataPath, PlayerPrefNames.LevelHolder + ".json");

        public static string LevelCreationDataPath =
            Path.Combine(Application.persistentDataPath, PlayerPrefNames.LevelCreationData + ".json");

        public static string CurrentLevel =
           Path.Combine(Application.persistentDataPath, PlayerPrefNames.CurrentLevel + ".json");

        public static string RestartLevel = Path.Combine(Application.persistentDataPath, PlayerPrefNames.RestartLevel + ".json");

        public static string CollectionData = "JsonFiles/CollectionData";
        public static string GetAllLevels = "JsonFiles/AllLevels";
        public static string UndoMoves = Path.Combine(Application.persistentDataPath, "UndoMoves" + ".json");
        public static string AllLevels = Path.Combine(Application.persistentDataPath, "AllLevels" + ".json");
    }
}