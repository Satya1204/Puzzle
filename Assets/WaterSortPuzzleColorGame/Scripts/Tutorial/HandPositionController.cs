using UnityEngine;

namespace WaterSortPuzzleGame.Tutorial
{
    public class HandPositionController : MonoBehaviour
    {
        [SerializeField] public Vector3 transferPosition;
        [SerializeField] public ClickController leftBottleClicker;
        [SerializeField] public ClickController rightBottleClicker;
        
        private void OnEnable()
        {
            leftBottleClicker.OnClicked += MoveRight;

            leftBottleClicker.OnClicked += leftBottleClicker.CallCloseCollider;

            leftBottleClicker.OnClicked += rightBottleClicker.CallOpenCollider;

            rightBottleClicker.OnClicked += DestroyHand;

        }

        private void OnDisable()
        {
            leftBottleClicker.OnClicked -= MoveRight;
            
            leftBottleClicker.OnClicked -= leftBottleClicker.CallCloseCollider;
            
            leftBottleClicker.OnClicked -= rightBottleClicker.CallOpenCollider;
            
            rightBottleClicker.OnClicked -= DestroyHand;
        }

        private void MoveRight()
        {
            gameObject.transform.position = transferPosition;
        }

        private void DestroyHand()
        {
            gameObject.SetActive(false);
        }
    }
}
