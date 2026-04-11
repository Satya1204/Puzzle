using System;
using UnityEngine;

namespace WaterSortPuzzleGame.Tutorial
{
    [RequireComponent(typeof(ColliderController))]
    public class ClickController : MonoBehaviour
    {

        public Action OnClicked;

        private ColliderController _colliderController;

        private void Awake()
        {
            _colliderController = GetComponent<ColliderController>();
        }
        private void Update()
        {
            if (!InputUtils.IsClickThisFrame()) return;

            var hit = InputUtils.Raycast2D(Camera.main);
            if (hit.collider == null) return;

            if (hit.collider.gameObject == gameObject)
                OnClicked?.Invoke();
        }

        // Old Input System
        //private void OnMouseDown()
        //{
        //    OnClicked?.Invoke();
        //}

        public void CallCloseCollider()
        {
            _colliderController.CloseColliderAfter(.1f);
        }

        public void CallOpenCollider()
        {
            _colliderController.OpenColliderAfter(.1f);
        }
    }
}
