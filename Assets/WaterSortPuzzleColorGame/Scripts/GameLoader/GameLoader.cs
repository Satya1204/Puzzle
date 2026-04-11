using System.IO;
using UnityEngine;
using WaterSortPuzzleGame.DataClass;

namespace WaterSortPuzzleGame
{
    public class GameLoader : MonoBehaviour//Singleton<GameLoader>
    {
        [SerializeField] public bool isDeletePrefab;
        private void CheckPlayerPrefs()
        {

            if (!PrefManager.HasKey(PlayerPrefNames.FirstTime))// || isDeletePrefab)
            {
                DeleteAllData();
                PrefManager.SetInt(PlayerPrefNames.FirstTime, 10);
            }
        }
        private void DeleteAllData()
        {
            if (File.Exists(Paths.UndoMoves))
            {
                File.Delete(Paths.UndoMoves);
            }
            if (File.Exists(Paths.CurrentLevel))
            {
                File.Delete(Paths.CurrentLevel);
            }
            EventManager.UpdateLevelText?.Invoke();

        }

        public void Init()
        {
            Application.targetFrameRate = 60;
            CheckPlayerPrefs();
        }
    }
}