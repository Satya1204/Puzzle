using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterSortPuzzleGame;
using WaterSortPuzzleGame.LevelScripts;

namespace WaterSortPuzzleGame.LevelGenerator
{
    [RequireComponent(typeof(LevelColorController))]
    [RequireComponent(typeof(CreateBottlesForLevel))]
    [RequireComponent(typeof(BottleCreateBottleState))]
    public class LevelGenerate : MonoBehaviour
    {
        [SerializeField] public int NumberOfLevelsToGenerate;
        public Data Data;
        public BottleCreateBottleState BottleCreateBottleState { get; private set; }
        public LevelColorController LevelColorController { get; private set; }

        private LevelColorController _colorController;
        private CreateBottlesForLevel _createBottlesForLevel;
        private BottleCreateBottleState _bottleCreateBottleState;
        private LevelMakerMainThreadActions _levelMakerMainThreadActions;
        private bool _firstTimeGenerate = false;
        private int _createdBottles;
        private int _numberOfBottlesCreate;
        private int _totalWaterCount;
        private void Awake()
        {
            _levelMakerMainThreadActions = FindAnyObjectByType<LevelMakerMainThreadActions>();
            _colorController = GetComponent<LevelColorController>();
            _createBottlesForLevel = GetComponent<CreateBottlesForLevel>();
            _bottleCreateBottleState = GetComponent<BottleCreateBottleState>();

            BottleCreateBottleState = _bottleCreateBottleState;
            LevelColorController = _colorController;

        }
        //1000 levels generated using this function.
        public void CreateAllLevels()
        {
            List<GenerateAllBottles> ListOfAllBottles = new List<GenerateAllBottles>();
            for (int i = 0; i < NumberOfLevelsToGenerate; i++)
            {
                EventManager.AllLevelsGenerate?.Invoke(i);
                var allBottle = CreateAllLevelPrototype();
                allBottle.LevelIndex = i;
                ListOfAllBottles.Add(allBottle);

            }
            JsonManager.SaveAllGenerateLevelsToJson(ListOfAllBottles);
            JsonManager.SaveLevelCreateDataToJson(ref Data);
        }

        public void CreateLevelProtoTypeFromMaker()
        {
            JsonManager.TryGetLevelCreateDataFromJson(ref Data);
            CreateLevelPrototype();
        }

        public void CreateLevelPrototype()
        {
            Data.CreatedBottles.Clear();
            _createdBottles = 0;
            _colorController.SelectedColors.Clear();

            _colorController.SelectColorsToCreate(Data);

            _colorController.CreateColorObjects();

            _totalWaterCount = _colorController.SelectedColors.Count * 4;

            _numberOfBottlesCreate = LevelMakerHelper.RandomizeNumberOfBottle(Data, _colorController);

            _createBottlesForLevel.CreateBottles(_numberOfBottlesCreate, _bottleCreateBottleState.NoMatches,
                _bottleCreateBottleState.RainbowBottle, ref _totalWaterCount,
                _colorController, Data, _createdBottles);

            var _allBottles = new GenerateAllBottles(Data.CreatedBottles);
            ColorNumerator.NumerateColors(_colorController.SelectedColors);

            if (_allBottles.IsSolvable())
            {

                _allBottles.NumberOfColorInLevel = _colorController.NumberOfColorsToCreate;
                _levelMakerMainThreadActions.MainThread_SaveToJson(_allBottles, _firstTimeGenerate);

                _levelMakerMainThreadActions.MainThread_SaveLevelCreateDataToJson(Data);

            }
            else
            {
                CreateLevelPrototype();
            }
        }
        private GenerateAllBottles CreateAllLevelPrototype()
        {
            Data.CreatedBottles.Clear();
            _createdBottles = 0;
            _colorController.SelectedColors.Clear();

            _colorController.SelectColorsToCreate(Data);

            _colorController.CreateColorObjects();

            _totalWaterCount = _colorController.SelectedColors.Count * 4;

            _numberOfBottlesCreate = LevelMakerHelper.RandomizeNumberOfBottle(Data, _colorController);

            _createBottlesForLevel.CreateBottles(_numberOfBottlesCreate, _bottleCreateBottleState.NoMatches,
                _bottleCreateBottleState.RainbowBottle, ref _totalWaterCount,
                _colorController, Data, _createdBottles);

            var _allBottles = new GenerateAllBottles(Data.CreatedBottles);
            ColorNumerator.NumerateColors(_colorController.SelectedColors);
            _allBottles.NumberOfColorInLevel = _colorController.NumberOfColorsToCreate;

            if (_allBottles.IsSolvable())
            {
                Debug.Log("Solved");
            }
            else
            {
                CreateAllLevelPrototype();

            }
            return _allBottles;
        }
    }
}
