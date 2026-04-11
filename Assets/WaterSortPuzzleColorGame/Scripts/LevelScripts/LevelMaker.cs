using WaterSortPuzzleGame.BottleCodes;
using UnityEngine;
using WaterSortPuzzleGame.LevelGenerator;
using System.Threading;

namespace WaterSortPuzzleGame.LevelScripts
{
    [RequireComponent(typeof(LevelMakerMainThreadActions))]
    public class LevelMaker : MonoBehaviour
    {
        [SerializeField]public int NumberOfLevelsToGenerate;
        private LevelMakerMainThreadActions _levelMakerMainThreadActions;
        private Thread _myThread;
        private void Awake()
        {
            _levelMakerMainThreadActions = GetComponent<LevelMakerMainThreadActions>();
        }
        private void OnEnable()
        {
            EventManager.CreateLevel += CreateNewLevel_GUIButton;
            EventManager.CreatePrototype += CreateLevelFromPrototype;
        }

        private void OnDisable()
        {
            EventManager.CreateLevel -= CreateNewLevel_GUIButton;
            EventManager.CreatePrototype -= CreateLevelFromPrototype;
        }
        public void CreateNewLevel_GUIButton()
        {
            if (GameManager.LevelIndex < NumberOfLevelsToGenerate)
            {
                JsonManager.FromAllLevelsToCurrentLevel();
            }
            else
            {

                LevelGenerate levelGenerate = FindAnyObjectByType<LevelGenerate>();

                if (_myThread == null || !_myThread.IsAlive)
                {
                    if (levelGenerate != null)
                    {

                        _myThread = new Thread(levelGenerate.CreateLevelProtoTypeFromMaker);
                    }
                }

                _myThread.Start();
            }

        }

        private void CreateLevelFromPrototype(AllBottles prototypeLevel)
        {
            _levelMakerMainThreadActions.MainThread_CreateLevelParentAndLineObjects(prototypeLevel.NumberOfColorInLevel);
            _levelMakerMainThreadActions.MainThread_CreateBottlesAndAssignPositions(prototypeLevel);
            _levelMakerMainThreadActions.MainThread_GetLevelParent();
        }

    }
}