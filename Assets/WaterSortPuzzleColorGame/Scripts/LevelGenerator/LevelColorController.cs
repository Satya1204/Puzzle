using System.Collections.Generic;
using UnityEngine;

namespace WaterSortPuzzleGame.LevelGenerator
{
    public class LevelColorController : MonoBehaviour
    {
        public int NumberOfColorsToCreate = 2;
        public List<Color> SelectedColors = new List<Color>();


        private List<MyColors> _myColorsList = new List<MyColors>();


        public void SelectColorsToCreate(Data data)
        {
            while (SelectedColors.Count < NumberOfColorsToCreate)
            {
                var selectedColor = GameManager.SelectedPalettes.GetRandomColor(data.GetBottleColorRandomIndex());
                if (!SelectedColors.Contains(selectedColor))
                    SelectedColors.Add(selectedColor);
            }
        }

        public void CreateColorObjects()
        {
            foreach (var color in SelectedColors)
            {
                MyColors colorObj = new MyColors(color);
                _myColorsList.Add(colorObj);
            }
        }
        public void GetRandomColorForBottle(GenerateBottle tempBottle, bool matchState, bool rainbowBottle, Data data)
        {
            for (int j = 0; j < tempBottle.BottleColorsIndex.Length; j++)
            {
                int index = GetColorFromList(matchState, rainbowBottle, tempBottle, j - 1, data);
                tempBottle.BottleColorsIndex[j] = index;
            }

            InitializeBottleNumberedStack(tempBottle);
        }
        private int GetColorFromList(bool matchState, bool rainbowBottle, GenerateBottle tempBottle, int checkIndex,
          Data data)
        {
            if (_myColorsList.Count <= 0) return -1;

            var randomColorIndex = GetRandomColorIndex(data);
            var color = GameManager.GetIndexFromColor(_myColorsList[randomColorIndex].Color);

            if (checkIndex >= 0)
            {
                if (rainbowBottle)
                {
                    var colorMatched = false;

                    var iteration = 0;

                    do
                    {
                        if (iteration > 200) break;

                        iteration++;

                        colorMatched = false;

                        if (_myColorsList.Count < 2) break;

                        for (int i = 0; i <= checkIndex; i++)
                        {
                            if (color.GetHashCode() != tempBottle.GetColorIndexAtPosition(i)) continue;

                            randomColorIndex = GetRandomColorIndex(data);
                            color = GameManager.GetIndexFromColor(_myColorsList[randomColorIndex].Color);

                            colorMatched = true;
                            break;
                        }
                    } while (colorMatched);
                }
                else
                {
                    while (matchState && color.GetHashCode() == tempBottle.GetColorIndexAtPosition(checkIndex))
                    {
                        if (_myColorsList.Count < 2) break;

                        randomColorIndex = GetRandomColorIndex(data);
                        color = GameManager.GetIndexFromColor(_myColorsList[randomColorIndex].Color);
                    }
                }
            }
            _myColorsList[randomColorIndex].Amount++;

            if (_myColorsList[randomColorIndex].MoreThan4())
                _myColorsList.RemoveAt(randomColorIndex);

            return color;
        }
       
        private void InitializeBottleNumberedStack(GenerateBottle comingBottle)
        {
            foreach (var colorHashCode in comingBottle.BottleColorsIndex)
            {
                var emptyColorHashCode = -1;
                if (colorHashCode != emptyColorHashCode)
                    comingBottle.NumberedBottleStack.Push(colorHashCode);
            }

            comingBottle.CheckIsSorted();
            comingBottle.CalculateTopColorAmount();
        }
       
        private int GetRandomColorIndex(Data data)
        {
            var hashString = "GetRandomColor " + data.GetColorPickerRandomIndex().ToString();
            var rand = new Unity.Mathematics.Random((uint)hashString.GetHashCode());

            var randomColorIndex = rand.NextInt(0, _myColorsList.Count);
            
            //Debug.Log(randomColorIndex);
            return randomColorIndex;
        }
    }
}