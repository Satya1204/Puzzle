using WaterSortPuzzleGame.BottleCodes;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    public class BoosterUndoBehavior : BoosterBehavior
    {
        private void OnEnable()
        {
            UndoLastMoveManager.OnMoveChange += OnMoveChange;
        }

        private void OnDisable()
        {
            UndoLastMoveManager.OnMoveChange -= OnMoveChange;
        }
        public override void Init()
        {
            IsSelectable = false;
        }
        private void OnMoveChange()
        {
            IsSelectable = UndoLastMoveManager._moves.Count > 0;
        }
        public override bool Activate()
        {
            return true;
        }
        public override bool ApplyToElement()
        {
            if (GameManager.Instance.InActionBottleList.Count == 0)
            {
                UndoLastMoveManager.UndoLastMove();
                return true;
            }
            return false;
        }
        public override void OnSelected()
        {
            base.OnSelected();
            BoosterController.ApplyToElement();
        }
        public override void OnDeselected(bool skipStackRemoval = false)
        {
            base.OnDeselected();
        }
    }
}