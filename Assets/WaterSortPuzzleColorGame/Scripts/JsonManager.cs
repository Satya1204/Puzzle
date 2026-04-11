using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using WaterSortPuzzleGame.BottleCodes;
using WaterSortPuzzleGame.DataClass;
using WaterSortPuzzleGame.UndoLastMove;
using WaterSortPuzzleGame.LevelGenerator;

namespace WaterSortPuzzleGame
{
    public static class JsonManager
    {

        public static List<AllBottles> _allBottles = new List<AllBottles>();
        public static List<GenerateAllBottles> _allGenerateBottles = new List<GenerateAllBottles>();
        

        public static void TryGetUndoMovesFromJson(ref List<Move> data)
        {
            if (File.Exists(Paths.UndoMoves))
            {
                var json = File.ReadAllText(Paths.UndoMoves);
                data = JsonHelper.FromJson<Move>(json).ToList();
            }
        }
        public static void TrySaveUndoMovesToJson(List<Move> tempMoves)
        {
            var json = JsonHelper.ToJson(tempMoves.ToArray(), true);
            File.WriteAllText(Paths.UndoMoves, json);
        }
       
        public static void FromAllLevelsToCurrentLevel()
        {
            TextAsset textAsset = Resources.Load<TextAsset>(Paths.GetAllLevels);
            if (textAsset != null)
            {
                string json;
                var data = JsonHelper.FromJson<AllBottles>(textAsset.text);
                var currentData = data.First(i => i.LevelIndex == GameManager.LevelIndex);
                json = JsonUtility.ToJson(currentData);
                File.WriteAllText(Paths.CurrentLevel, json);
                File.WriteAllText(Paths.RestartLevel, json);

                EventManager.LoadNextLevel?.Invoke();
            }

        }
        public static bool IsAvailableInAllLevels()
        {
            TextAsset textAsset = Resources.Load<TextAsset>(Paths.GetAllLevels);
            if (textAsset != null)
            {
                var data = JsonHelper.FromJson<AllBottles>(textAsset.text);
                bool isAvailable = data.Any(i => i.LevelIndex == GameManager.LevelIndex);
                if (isAvailable) return true;
            }
            return false;
        }
        public static void UpdateCurrentLevelDatatoJson()
        {
            Bottle tempBottle = new Bottle(-1);
            List<Bottle> tempBottles = new List<Bottle>();


            foreach (var bottle in GameManager.Instance.bottleControllers)
            {
                tempBottle = bottle.HelperBottle;
                tempBottle.NumberOfColorsInBottle = bottle.BottleData.NumberOfColorsInBottle;
                tempBottle.BottleColorsIndex = bottle.BottleData.BottleColorsIndex;                                                                  //tempBottle.BottleColorsHashCodes = ColorToHashCode(bottle.BottleData.BottleColorsIndex);
                tempBottles.Add(tempBottle);
            }
            AllBottles tempAllBottles = new AllBottles(tempBottles);
            var path = Paths.CurrentLevel;
            if (File.Exists(path))
            {
                tempAllBottles.LevelIndex = GameManager.LevelIndex;
                tempAllBottles.NumberOfColorInLevel = GameManager.Instance.TotalColorAmount;
                var json = JsonUtility.ToJson(tempAllBottles);
                File.WriteAllText(path, json);
            }
        }

        public static void SaveAllGenerateLevelsToJson(List<GenerateAllBottles> data)
        {
            var json = JsonHelper.ToJson(data.ToArray(), false);
            var path = Paths.AllLevels;
            File.WriteAllText(path, json);

        }
        public static void SaveLevelCreateDataToJson(ref Data data)
        {
            var json = JsonUtility.ToJson(data);
            var path = Paths.LevelCreationDataPath;
            File.WriteAllText(path, json);
        }
        public static void TryGetLevelCreateDataFromJson(ref Data data)
        {
            var path = Paths.LevelCreationDataPath;

            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                data = JsonUtility.FromJson<Data>(json);
            }
            else
            {
                Debug.Log("Data not exists. Creating new one");
                var json = JsonUtility.ToJson(data);
                File.WriteAllText(path, json);
            }
        }
        public static void SaveToJson(GenerateAllBottles allBottles, bool _firstTimeGenerate)
        {
            if (_firstTimeGenerate) // this code uses when first time 1000 levels generate
            {
                allBottles.LevelIndex = _allBottles.Count;
                _allGenerateBottles.Add(allBottles);

                SaveAllGenerateLevelsToJson(_allGenerateBottles);
            }
            else
            {
                allBottles.LevelIndex = GameManager.LevelIndex;
                var json = JsonUtility.ToJson(allBottles);

                File.WriteAllText(Paths.CurrentLevel, json);
                File.WriteAllText(Paths.RestartLevel, json);
                EventManager.LoadNextLevel?.Invoke();
            }

        }
    }


    public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }
    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
}