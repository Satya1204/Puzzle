using WaterSortPuzzleGame.BottleCodes;
using UnityEngine;

namespace WaterSortPuzzleGame.UndoLastMove
{
    [System.Serializable]
    public class Move
    {
        public MovesBottle _firstBottle;
        public MovesBottle _secondBottle;
        public int _transferColorAmount;
        public int _color;
        private BottleController _firstBottleController;
        private BottleController _secondBottleController;

        public Move(BottleController first, BottleController second, int colorAmountAmount, int color)
        {

            MovesBottle movesBottle = new MovesBottle();

            movesBottle.BottleIndex = first.BottleData.BottleIndex;
            movesBottle.NumberOfColorsInBottle = first.BottleData.NumberOfColorsInBottle;
            movesBottle.BottleColorsIndex = first.BottleData.BottleColorsIndex;
            _firstBottle = movesBottle;

            movesBottle = new MovesBottle();
            movesBottle.BottleIndex = second.BottleData.BottleIndex;
            movesBottle.NumberOfColorsInBottle = second.BottleData.NumberOfColorsInBottle;
            movesBottle.BottleColorsIndex = second.BottleData.BottleColorsIndex;
            _secondBottle = movesBottle;

            _transferColorAmount = colorAmountAmount;
            _color = color;
        }

        public void UndoNewMove()
        {
            GameObject _currentLevel = GameObject.Find("LevelParent");
            for (int i = 0; i < _currentLevel.transform.childCount; i++)
            {
                if (_currentLevel.transform.GetChild(i).TryGetComponent(out BottleController controller))
                {
                    if (controller.BottleData.BottleIndex == _firstBottle.BottleIndex)
                    {
                        _firstBottleController = controller;

                    }
                    else if (controller.BottleData.BottleIndex == _secondBottle.BottleIndex)
                    {
                        _secondBottleController = controller;
                    }
                }
            }

            _firstBottleController.BottleData.NumberOfColorsInBottle += _transferColorAmount;
            _secondBottleController.BottleData.NumberOfColorsInBottle -= _transferColorAmount;

            var firstStartIndex = _firstBottleController.BottleData.NumberOfColorsInBottle - _transferColorAmount;
            firstStartIndex = (int)Mathf.Clamp(firstStartIndex, 0, Mathf.Infinity);

            for (var i = firstStartIndex; i < _firstBottleController.BottleData.NumberOfColorsInBottle; i++)
            {
                _firstBottleController.BottleData.BottleColorsIndex[i] = _color;
            }

            _firstBottleController.UpdateAfterUndo();
            _secondBottleController.UpdateAfterUndo();

            _firstBottleController.BottleColorController.CheckIsBottleSorted(_firstBottleController.BottleData);
            _secondBottleController.BottleColorController.CheckIsBottleSorted(_secondBottleController.BottleData);
            JsonManager.UpdateCurrentLevelDatatoJson();
        }
    }

    [System.Serializable]
    public class MovesBottle
    {
        public int ParentNum;
        public int NumberOfColorsInBottle = 0;
        public int[] BottleColorsIndex = new int[4];
        public int BottleIndex = -1;
    }
}
