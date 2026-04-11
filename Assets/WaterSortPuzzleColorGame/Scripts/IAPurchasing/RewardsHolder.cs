using UnityEngine;
using UnityEngine.Events;

namespace WaterSortPuzzleGame
{
    public abstract class RewardsHolder : MonoBehaviour
    {
        [SerializeReference] public Reward[] rewards;

        protected void InitializeComponents()
        {
            // Initialize rewards
            for (int i = 0; i < rewards.Length; i++)
            {
                rewards[i].Init();
            }
        }

        public void ApplyRewards(int quantity)
        {
            for (int i = 0; i < rewards.Length; i++)
            {
                rewards[i].ApplyReward(quantity);
            }
        }
        public void ApplyAnimation(int quantity)
        {
            for (int i = 0; i < rewards.Length; i++)
            {
                rewards[i].ApplyAnimation(quantity);
            }
        }
        
    }
}
