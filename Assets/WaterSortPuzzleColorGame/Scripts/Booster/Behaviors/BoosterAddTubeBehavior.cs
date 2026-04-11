using WaterSortPuzzleGame.DataClass;
using WaterSortPuzzleGame.BottleCodes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace WaterSortPuzzleGame
{
    public class BoosterAddTubeBehavior : BoosterBehavior
    {
        private void OnEnable()
        {
            EventManager.TempEmptyTubesChange += CheckSelectableStatus;
        }

        private void OnDisable()
        {
            EventManager.TempEmptyTubesChange -= CheckSelectableStatus;
        }
        public override void Init()
        {
           // IsSelectable = false;
            CheckSelectableStatus();
        }
        public override bool Activate()
        {
            return true;
        }
        public override bool ApplyToElement()
        {
            if (GameManager.Instance.InActionBottleList.Count == 0)
            {
                EventManager.AddExtraEmptyBottle?.Invoke();
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
        private void CheckSelectableStatus()
        {
            IsSelectable = (GameManager.TempEmptyTubes >= 2) ? false : true;
        }
    }
}