using WaterSortPuzzleGame.BottleCodes;
using WaterSortPuzzleGame.Enum;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WaterSortPuzzleGame.GameController
{
    public class ActionController : MonoBehaviour
    {
        public BottleController FirstBottle;
        public BottleController SecondBottle;

        private Camera _camera;


        private void Start()
        {
            _camera = Camera.main;
        }

        private async void Update()
        {
            if (!InputUtils.IsClickThisFrame()) return;

            if (GameManager.gameState == GameState.Pause) return;

            if (EventSystem.current.IsPointerOverGameObject())
            {

                GameObject selected = EventSystem.current.currentSelectedGameObject;

                // Skip Devtool buttons
                if (selected != null && !selected.CompareTag("IgnoreBlockCancel"))
                {
                    if (FirstBottle != null && SecondBottle == null)
                    {
                        FirstBottle.BottleAnimationController.OnSelectionCanceled();
                        FirstBottle = null;
                    }
                }

            }

            var hit = InputUtils.Raycast2D(_camera);

            if (hit.collider == null) return;

            if (!hit.collider.TryGetComponent(out BottleController bottleController)) return;

            if (FirstBottle == null)
            {
                if (bottleController.IsBottleEmpty()) return;

                if (bottleController.BottleData.NumberOfTopColorLayers == 4) return;

                if (bottleController.BottleAnimationController.BottleIsLocked)
                    await bottleController.BottleAnimationSpeedUp.SpeedUpActions(bottleController.BottleData);

                FirstBottle = bottleController;
                FirstBottle.BottleAnimationController.OnSelected();
            }
            else
            {
                var isClickedSameBottleAgain = FirstBottle == bottleController;

                if (isClickedSameBottleAgain)
                {
                    FirstBottle.BottleAnimationController.OnSelectionCanceled();
                    FirstBottle = null;
                }
                else
                {
                    var maxAmountOfBottleCanTake = 4;
                    var isBottleFull = bottleController.BottleData.NumberOfColorsInBottle >= maxAmountOfBottleCanTake;

                    if (isBottleFull)
                    {
                        FirstBottle.BottleAnimationController.OnSelectionCanceled();
                        FirstBottle = null;
                        SecondBottle = null;

                        print("second bottle full!");
                        return;
                    }
                    var isTopColorsNotMatch = bottleController.BottleData.TopColorIndex != FirstBottle.BottleData.TopColorIndex &&
                              bottleController.BottleData.NumberOfColorsInBottle > 0;

                    if (isTopColorsNotMatch)
                    {
                        FirstBottle.BottleAnimationController.OnSelectionCanceled();
                        FirstBottle = null;
                        SecondBottle = null;

                        print("top colors not matching!");
                        return;
                    }

                    SecondBottle = bottleController;

                    FirstBottle.BottleTransferController.BottleControllerRef = SecondBottle;
                    SecondBottle.BottleTransferController.BottleControllerRef = FirstBottle;
                    SecondBottle.BottleData.ActionBottles.Add(FirstBottle);

                    if (SecondBottle.BottleTransferController.FillBottleCheck(FirstBottle.BottleData.TopColorIndex))
                    {
                        FirstBottle.StartColorTransfer();

                        FirstBottle = null;
                        SecondBottle = null;
                    }
                    else
                    {
                        FirstBottle = null;
                        SecondBottle = null;
                    }
                }
            }
        }

    }
}