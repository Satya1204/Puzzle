using UnityEngine;
namespace WaterSortPuzzleGame
{
    public abstract class Reward : MonoBehaviour
    {
        public virtual void Init() { }

        public abstract void ApplyReward(int quantity);
        public virtual void ApplyAnimation(int quantity) { }

    }
}
