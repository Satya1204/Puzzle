using UnityEditor;

namespace WaterSortPuzzleGame
{
    [InitializeOnLoad]
    public static class CoreEditor
    {
        public static bool AutoLoadInitializer { get; private set; } = true;
        public static string InitSceneName { get; private set; } = "Loading";
    }
}