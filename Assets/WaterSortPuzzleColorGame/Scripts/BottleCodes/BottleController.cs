using WaterSortPuzzleGame.BottleCodes.Animation;
using UnityEngine;

namespace WaterSortPuzzleGame.BottleCodes
{
    [RequireComponent(typeof(BottleData))]
    [RequireComponent(typeof(BottleColorController))]
    [RequireComponent(typeof(BottleAnimationController))]
    [RequireComponent(typeof(BottleTransferController))]
    [RequireComponent(typeof(BottleAnimationSpeedUp))]
    [RequireComponent(typeof(BottleSpriteRendererOrderController))]
    [RequireComponent(typeof(BottleFindRotationPointAndDirection))]

    [System.Serializable]
    public class BottleController : MonoBehaviour
    {
        public BottleData BottleData { get; private set; }
        public BottleColorController BottleColorController { get; private set; }
        public BottleAnimationController BottleAnimationController { get; private set; }
        public BottleTransferController BottleTransferController { get; private set; }
        public BottleAnimationSpeedUp BottleAnimationSpeedUp { get; private set; }
        public FillAndRotationValues FillAndRotationValues { get; private set; }
        public BottleSpriteRendererOrderController BottleSpriteRendererOrderController { get; private set; }
        public SpriteRenderer BottleSpriteRenderer { get; private set; }
        public BottleFindRotationPointAndDirection BottleFindRotationPointAndDirection { get; private set; }


        [Header("Bottle Sprite Renderer")] public SpriteRenderer BottleMaskSR;


        // Game manager
        private GameManager _gm;


        [Header("Bottle Helper")] [SerializeField]
        public Bottle HelperBottle;


        public float _preRotateAmount = 0f;
        private void Awake()
        {
            BottleData = GetComponent<BottleData>();
            BottleColorController = GetComponent<BottleColorController>();
            BottleAnimationController = GetComponent<BottleAnimationController>();
            BottleTransferController = GetComponent<BottleTransferController>();
            BottleAnimationSpeedUp = GetComponent<BottleAnimationSpeedUp>();
            BottleSpriteRendererOrderController = GetComponent<BottleSpriteRendererOrderController>();
            BottleFindRotationPointAndDirection = GetComponent<BottleFindRotationPointAndDirection>();
            BottleSpriteRenderer = GetComponent<SpriteRenderer>();

            FillAndRotationValues = FillAndRotationValues.Instance;
        }

        private void Start()
        {
            _gm = GameManager.Instance;
            BottleMaskSR.material = _gm.Mat;

            BottleColorController.SetFillAmount(FillAndRotationValues.GetFillCurrentAmount(BottleData));
            BottleColorController.UpdateColorsOnShader(BottleData.BottleColorsIndex);
            BottleColorController.UpdateTopColorValues(BottleData);
            BottleColorController.CheckIsBottleSorted(BottleData);
        }

        public void UpdateAfterUndo()
        {
            BottleColorController.SetFillAmount(FillAndRotationValues.GetFillCurrentAmount(BottleData));
            BottleColorController.UpdateColorsOnShader(BottleData.BottleColorsIndex);
            BottleData.TopColorIndex = BottleData.PreviousTopColorIndex;
            BottleColorController.UpdateTopColorValues(BottleData);
        }

        public bool IsBottleEmpty()
        {
            return BottleData.NumberOfColorsInBottle <= 0;
        }

        public void StartColorTransfer()
        {
            AddActionBottleToActionBottleList();
            PreRotaionAmount();
            // chose rotation point and direction
            BottleFindRotationPointAndDirection.ChoseRotationPointAndDirection(BottleTransferController
                .BottleControllerRef);

            var bottleControllerRef = BottleTransferController.BottleControllerRef;
            var bottleRefData = bottleControllerRef.BottleData;

            // get how many water color will pour
            BottleTransferController.NumberOfColorsToTransfer = Mathf.Min(BottleData.NumberOfTopColorLayers,
                4 - bottleRefData.NumberOfColorsInBottle);

            // setting array color values to pouring water color
            for (var i = 0; i < BottleTransferController.NumberOfColorsToTransfer; i++)
            {
                bottleRefData.BottleColorsIndex[bottleRefData.NumberOfColorsInBottle + i] = BottleData.TopColorIndex;
            }

            // updating colors on shader
            bottleControllerRef.BottleColorController.UpdateColorsOnShader(bottleRefData.BottleColorsIndex);

            // setting render order
            BottleSpriteRendererOrderController.SetSortingOrder(BottleSpriteRenderer, BottleMaskSR);

            // call move bottle
            StartAnimationChain();

            // call pre rotate bottle
            PreRotateBottle();
            
        }

        private void AddActionBottleToActionBottleList()
        {
            _gm.InActionBottleList.Add(this);
        }

        private void StartAnimationChain()
        {
            BottleAnimationController.DisableCollider();
            BottleFindRotationPointAndDirection.ChoseMovePosition(BottleTransferController, _preRotateAmount);

            BottleAnimationController.StartAnimationChain();
        }

        private float PreRotaionAmount()
        {
            if (BottleData.NumberOfColorsInBottle == 4)
            {
                _preRotateAmount = 40.0f;//28.0f;
            }
            else if (BottleData.NumberOfColorsInBottle == 3)
            {
                _preRotateAmount = 60.0f;//45.0f;
            }
            else if (BottleData.NumberOfColorsInBottle == 2)
            {
                _preRotateAmount = 70.0f;
            }
            else
            {
                _preRotateAmount = 80.0f;//45.0f;
            }
            return _preRotateAmount;
        }
        private void PreRotateBottle()
        {
            BottleAnimationController.PlayPreRotateTween();
        }

        public void SetOriginalPositionTo(Vector3 position)
        {
            BottleAnimationController.SetOriginalPosition(position);
        }
    }
}