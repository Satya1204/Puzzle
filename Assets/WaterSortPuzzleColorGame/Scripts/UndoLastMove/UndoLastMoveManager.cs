using System.Collections.Generic;
using System.Linq;
using WaterSortPuzzleGame.BottleCodes;
using UnityEngine;
using System;
using WaterSortPuzzleGame.UndoLastMove;

namespace WaterSortPuzzleGame
{
    public class UndoLastMoveManager
    {
        public static List<Move> _moves = new();
        public static event Action OnMoveChange;

        public static void UpdateMovesList() 
        {
            JsonManager.TryGetUndoMovesFromJson(ref _moves);
            OnMoveChange?.Invoke();
        }
        public static void AddMoveToList(BottleController first, BottleController second, int numberOfTopColorLayer, int color)
        {
            var move = new Move(first, second, numberOfTopColorLayer, color);
            _moves.Add(move);
            MoveToJson();
            OnMoveChange?.Invoke();
        }

        public static void UndoLastMove()
        {
            if (_moves.Count == 0) return ;

            var lastMove = _moves.Last();
            _moves.Remove(lastMove);

            lastMove.UndoNewMove();
            MoveToJson();
            if (_moves.Count == 0)
            {
                OnMoveChange?.Invoke();
            }
        }

        public static void ResetUndoActions()
        {
            _moves.Clear();
            MoveToJson();
            OnMoveChange?.Invoke();
        }
        private static void MoveToJson()
        {
            JsonManager.TrySaveUndoMovesToJson(_moves);
        }
    }
}

