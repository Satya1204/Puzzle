using System.Threading;
using WaterSortPuzzleGame.BottleCodes;
using WaterSortPuzzleGame.Dispatchers;
using UnityEngine;
using WaterSortPuzzleGame.LevelGenerator;

namespace WaterSortPuzzleGame.LevelScripts
{
    [RequireComponent(typeof(LevelBottlesAligner))]
    public class LevelMakerMainThreadActions : MonoBehaviour
    {
        private LevelBottlesAligner _bottlesAligner;
        private LevelMakerCreateBottle _createBottle;
        private LevelMaker _levelMaker;

        private void Awake()
        {
            _bottlesAligner = GetComponent<LevelBottlesAligner>();
            _createBottle = GetComponent<LevelMakerCreateBottle>();
            _levelMaker = GetComponent<LevelMaker>();
        }
        
        private void Update()
        {
            Dispatcher.Instance.InvokePending();
        }
        
        public void MainThread_GetLevelParent()
        {
            Dispatcher.Instance.Invoke(() => { EventManager.GetLevelParent?.Invoke(_bottlesAligner.LastCreatedParent); });
        }
        
        public void MainThread_CreateLevelParentAndLineObjects(int numberOfColorInLevel)
        {
            Thread.Sleep(50);
            Dispatcher.Instance.Invoke(() => _bottlesAligner.CreateLevelParentAndLineObjects(numberOfColorInLevel));
        }
        
        public void MainThread_CreateBottlesAndAssignPositions(AllBottles allBottles)
        {
            Dispatcher.Instance.Invoke(() =>_createBottle.CreateBottlesAndAssignPositions(allBottles,_bottlesAligner));
        }
        public void MainThread_SaveToJson(GenerateAllBottles allBottles, bool _firstTimeGenerate)
        {
            Dispatcher.Instance.Invoke(() => JsonManager.SaveToJson(allBottles, _firstTimeGenerate));
        }

        public void MainThread_SaveLevelCreateDataToJson(Data data)
        {
            Dispatcher.Instance.Invoke(() => JsonManager.SaveLevelCreateDataToJson(ref data));
        }

    }
}
