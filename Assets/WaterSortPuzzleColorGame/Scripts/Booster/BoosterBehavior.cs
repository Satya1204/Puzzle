using UnityEngine;
using System;
using WaterSortPuzzleGame.BottleCodes;

namespace WaterSortPuzzleGame
{
    public abstract class BoosterBehavior : MonoBehaviour
    {
        protected BoosterSettings settings;
        public BoosterSettings Settings => settings;

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            protected set
            {
                isBusy = value;
            }
        }
        private bool isSelectable = true;

        public bool IsSelectable
        {
            get => isSelectable;
            set { isSelectable = value; isDirty = true; }
        }


        private bool isSelected;
        public virtual bool IsSelected
        {
            get => isSelected;
            protected set
            {
                isSelected = value;

                SelectStateChanged?.Invoke(isSelected);
            }
        }

        protected bool isDirty = true;
        public bool IsDirty => isDirty;

        public event Action<bool> SelectStateChanged;

        public void InitialiseSettings(BoosterSettings settings)
        {
            this.settings = settings;
        }

        public abstract void Init();
        public abstract bool Activate();
        public abstract bool ApplyToElement();
        public virtual void OnSelected()
        {
            isSelected = true;
            isDirty = true;
            
            SelectStateChanged?.Invoke(true);
        }

        public virtual void OnDeselected(bool skipStackRemoval = false)
        {
            isSelected = false;
            isDirty = true;

            SelectStateChanged?.Invoke(false);
            
        }

        public virtual bool IsActive() => true;

        public void OnRedrawn()
        {
            isDirty = false;
        }

    }

}