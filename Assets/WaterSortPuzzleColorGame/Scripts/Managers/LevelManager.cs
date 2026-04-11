using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WaterSortPuzzleGame.BottleCodes;
using UnityEngine;
using System.Collections;

namespace WaterSortPuzzleGame.Managers
{
    public class LevelManager : MonoBehaviour
    {
        private GameObject _currentLevel;
        private void OnEnable()
        {
            EventManager.LoadNextLevel += LoadNextLevel;
            EventManager.RestartLevel += RestartLevel;
            EventManager.GetLevelParent += GetLevelParent;
            EventManager.CreateNewLevelForJson += CreateNewLevelForJson;
            EventManager.CreatePrototypeOrLevel += CreateLevel;
            EventManager.BackToHomeScreen += BackToHomeScreen;
        }

        private void OnDisable()
        {
            EventManager.LoadNextLevel -= LoadNextLevel;
            EventManager.RestartLevel -= RestartLevel;
            EventManager.GetLevelParent -= GetLevelParent;
            EventManager.CreateNewLevelForJson -= CreateNewLevelForJson;
            EventManager.CreatePrototypeOrLevel -= CreateLevel;
            EventManager.BackToHomeScreen -= BackToHomeScreen;
        }
        private void Start()
        {
            UndoLastMoveManager.UpdateMovesList();

            if (GameManager.IsLevelScreen)
            {
                CreateLevel();
            }
        }
        private void BackToHomeScreen()
        {
            if (_currentLevel != null) Destroy(_currentLevel);

            if (GameObject.Find("BottlePreRotationPosition"))
            {
                Destroy(GameObject.Find("BottlePreRotationPosition"));
            }
        }
        public void CreateLevel()
        {

            if (_currentLevel != null) Destroy(_currentLevel);

            var path = Paths.CurrentLevel;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                AllBottles levelPrototype = JsonUtility.FromJson<AllBottles>(json);
                if (GameManager.LevelIndex == levelPrototype.LevelIndex)
                {
                    EventManager.CreatePrototype?.Invoke(levelPrototype);
                    BoosterController.BoosterUIController.OnLevelStarted();
                    StartCoroutine(CheckUnlockedSkins());
                    
                }
                else
                {
                    GameManager.TempEmptyTubes = 0;
                    EventManager.CreateLevel?.Invoke();
                }

            }
            else
            {
                GameManager.TempEmptyTubes = 0;
                EventManager.CreateLevel?.Invoke();
            }
        }
        private void RestartLevel()
        {
            UndoLastMoveManager.ResetUndoActions();


            Destroy(_currentLevel);

            // If Current Level is available in AllLevels.Json.
            if (JsonManager.IsAvailableInAllLevels())
            {
                JsonManager.FromAllLevelsToCurrentLevel();
                return;
            }

            // This will use for random levels.
            var path = Paths.RestartLevel;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                AllBottles levelPrototype = JsonUtility.FromJson<AllBottles>(json);
                if (GameManager.LevelIndex == levelPrototype.LevelIndex)
                {
                    File.WriteAllText(Paths.CurrentLevel, json);
                    CreateLevel();
                    return;
                }
            }
            JsonManager.FromAllLevelsToCurrentLevel();

        }

        private void GetLevelParent(GameObject levelParent)
        {
            _currentLevel = levelParent;
        }

        private void LoadNextLevel()
        {
            UndoLastMoveManager.ResetUndoActions();
            CreateLevel();
        }

        private void CreateNewLevelForJson()
        {
            EventManager.CreateLevel?.Invoke();
        }
        private IEnumerator CheckUnlockedSkins()
        {
            var unlockedList = new List<(SkinType, SkinData)>();

            ThemeSkinData unlockedTheme = SkinManager.Instance.ThemeSkinController.CheckForNewUnlockedSkin();
            if (unlockedTheme != null) unlockedList.Add((SkinType.Theme, unlockedTheme));

            TubeSkinData unlockedTube = SkinManager.Instance.TubesSkinController.CheckForNewUnlockedSkin();
            if (unlockedTube != null) unlockedList.Add((SkinType.Tube, unlockedTube));

            PaletteSkinData unlockedPalette = SkinManager.Instance.PaletteSkinController.CheckForNewUnlockedSkin();
            if (unlockedPalette != null) unlockedList.Add((SkinType.Palette, unlockedPalette));

            if (unlockedList.Count > 0)
            {
                bool popupDone = false;

                SkinUnlockedPanel gameUI = UIController.GetPage<SkinUnlockedPanel>();
                gameUI.SetData(unlockedList);

                UIController.ShowPage<SkinUnlockedPanel>();

                gameUI.OnPopupClosed = () => popupDone = true;

                yield return new WaitUntil(() => popupDone);
            }

            BoosterController.BoosterUIController.UnlockBoosters();
        }
        

        #region Development

        public void ReloadDev()
        {
            RestartLevel();
        }
        public void PrevLevelDev()
        {
            if (GameManager.LevelIndex == 0) return;
            GameManager.LevelIndex--;
            EventManager.UpdateLevelText?.Invoke();
            CreateLevel();
        }
        public void NextLevelDev()
        {
            GameManager.LevelIndex++;
            EventManager.UpdateLevelText?.Invoke();
            CreateLevel();
        }

        #endregion
    }
}