using WaterSortPuzzleGame.BottleCodes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaterSortPuzzleGame.LevelScripts
{
    [RequireComponent(typeof(LevelBottlesAligner))]
    public class LevelMakerCreateBottle : MonoBehaviour
    {
        [SerializeField] private float _minXDistanceBetweenHolders;
        [SerializeField] private Camera _camera;

        private GameObject _obj;
        private LevelBottlesAligner _levelBottlesAligner;
        private int rowCount = 1;

        private void Awake()
        {
            _levelBottlesAligner = GetComponent<LevelBottlesAligner>();
        }

        private void OnEnable()
        {
            EventManager.AddExtraEmptyBottle += AddExtraEmptyBottle;
            ScreenResizeManager.OnScreenSizeChanged += RepositionBottles;
        }

        private void OnDisable()
        {
            EventManager.AddExtraEmptyBottle -= AddExtraEmptyBottle;
            ScreenResizeManager.OnScreenSizeChanged -= RepositionBottles;
        }
        private void RepositionBottles()
        {
            if (!GameManager.IsLevelScreen) return;
            var bottleControllerList = GameManager.Instance.bottleControllers;
            var list = PositionsForHolders(bottleControllerList.Count, out var width).ToList();
            AdjustCameraSize(width);

            for (var i = 0; i < bottleControllerList.Count; i++)
            {
                bottleControllerList[i].transform.position = list[i];
                bottleControllerList[i].SetOriginalPositionTo(list[i]);
            }
        }
        private void AddExtraEmptyBottle()
        {
            GameManager.TempEmptyTubes++;
            var gm = GameManager.Instance;
            Bottle extraBottleHelper = new Bottle(-1);
            var extraBottle = InitializeBottle();
            extraBottle.HelperBottle = extraBottleHelper;
            extraBottle.BottleData.NumberOfColorsInBottle = 0;
            extraBottle.transform.SetParent(_levelBottlesAligner.LevelParent.transform);

            // add new bottle to list
            var bottleControllerList = gm.bottleControllers;
            bottleControllerList.Add(extraBottle);


            var list = PositionsForHolders(bottleControllerList.Count, out var width).ToList();
            AdjustCameraSize(width);
            for (var i = 0; i < bottleControllerList.Count; i++)
            {
                bottleControllerList[i].transform.position = list[i];
                bottleControllerList[i].SetOriginalPositionTo(list[i]);
            }

            JsonManager.UpdateCurrentLevelDatatoJson();
        }
        public void CreateBottlesAndAssignPositions(AllBottles AllBottlesInLevel, LevelBottlesAligner levelBottlesAligner)
        {
            var list = PositionsForHolders(AllBottlesInLevel._allBottles.Count, out var width).ToList();
            AdjustCameraSize(width);

            for (int i = 0; i < AllBottlesInLevel._allBottles.Count; i++)
            {
                var newBottle = InitializeBottle();
                newBottle.HelperBottle = AllBottlesInLevel._allBottles[i];
                newBottle.BottleData.NumberOfColorsInBottle = AllBottlesInLevel._allBottles[i].NumberOfColorsInBottle;
                newBottle.BottleData.BottleIndex = i;
                newBottle.transform.position = list[i]; 
                newBottle.BottleData.BottleColorsIndex = AllBottlesInLevel._allBottles[i].BottleColorsIndex;

                newBottle.transform.SetParent(levelBottlesAligner.LevelParent.transform);

                if(i == 0)
                {
                    levelBottlesAligner.CreateAssumeNextPosition(newBottle.gameObject);
                }
            }

            EventManager.TutorialAnimation?.Invoke();
        }

        public IEnumerable<Vector2> PositionsForHolders(int count, out float expectWidth)
        {
            if (IsPortrait())
            {
                return PortraitPositionsForHolders(count, out expectWidth);
            }
            else
            {
                return LandScapePositionsForHolders(count, out expectWidth);
            }
        }
        public IEnumerable<Vector2> LandScapePositionsForHolders(int count, out float expectWidth)
        {
            expectWidth = 8 * _minXDistanceBetweenHolders;
            rowCount = 1;  // Default row count (single row)

            if (count < 10)
            {
                var minPoint = transform.position - ((count - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right -
                               Vector3.up * 0.04f;

                expectWidth = Mathf.Max(count * _minXDistanceBetweenHolders, expectWidth);

                return Enumerable.Range(0, count)
                    .Select(i => (Vector2)minPoint + i * _minXDistanceBetweenHolders * Vector2.right);
            }

            rowCount = 2;  // We force exactly two rows
            var maxCountInRow = Mathf.CeilToInt(count / 2f);

            // Ensure width is sufficient for row alignment
            expectWidth = Mathf.Max((maxCountInRow + 1) * _minXDistanceBetweenHolders, 5 * _minXDistanceBetweenHolders);

            float aspect = (float)Screen.width / Screen.height;
            float height = expectWidth / aspect;

            // **Increase row spacing to prevent overlap**
            float rowSpacing = _minXDistanceBetweenHolders * 2.2f;

            var list = new List<Vector2>();

            // **Top Row Positioning**
            var topRowMinPoint = transform.position + Vector3.up * (rowSpacing / 2f) -
                                 ((maxCountInRow - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right;
            list.AddRange(Enumerable.Range(0, maxCountInRow)
                .Select(i => (Vector2)topRowMinPoint + i * _minXDistanceBetweenHolders * Vector2.right));

            // **Bottom Row Positioning**
            var lowRowMinPoint = transform.position - Vector3.up * (rowSpacing / 2f) -
                                 (((count - maxCountInRow) - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right;
            list.AddRange(Enumerable.Range(0, count - maxCountInRow)
                .Select(i => (Vector2)lowRowMinPoint + i * _minXDistanceBetweenHolders * Vector2.right));

            return list;
        }
        private void AdjustCameraSize(float width)
        {
            if (IsPortrait())
            {
                _camera.orthographicSize = 0.5f * width * Screen.height / Screen.width;
            }
            else
            {
                float bottleHeight = 3.744623f;  // Your bottle collider height
                float padding = 6.0f;            // Extra space at top & bottom
                float totalHeight = (rowCount * bottleHeight) + padding;

                float aspectRatio = (float)Screen.width / Screen.height;
                float widthNeeded = (_minXDistanceBetweenHolders * 9) / aspectRatio + 1f;  // extra space 1f at left right

                _camera.orthographicSize = Mathf.Max(totalHeight * 0.5f, widthNeeded * 0.5f);
            }
            
        }

        public IEnumerable<Vector2> PortraitPositionsForHolders(int count, out float expectWidth)
        {
            float rowSpacing = _minXDistanceBetweenHolders * 2.6f;

            expectWidth = 4 * _minXDistanceBetweenHolders;
            if (count < 6)
            {
                var minPoint = transform.position - ((count - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right -
                               Vector3.up * 0.04f;

                expectWidth = Mathf.Max(count * _minXDistanceBetweenHolders, expectWidth);

                return Enumerable.Range(0, count)
                    .Select(i => (Vector2)minPoint + i * _minXDistanceBetweenHolders * Vector2.right);
            }

            var aspect = (float)Screen.width / Screen.height;

            var maxCountInRow = Mathf.CeilToInt(count / 2f);

            if ((maxCountInRow + 1) * _minXDistanceBetweenHolders > expectWidth)
            {
                expectWidth = (maxCountInRow + 1) * _minXDistanceBetweenHolders;
            }
            else // else added by payal 
            {
                expectWidth = 5 * _minXDistanceBetweenHolders;
            }

            var height = expectWidth / aspect;


            var list = new List<Vector2>();
            var topRowMinPoint = transform.position + Vector3.up * (rowSpacing / 2f) -
                                 ((maxCountInRow - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right - Vector3.up * 0.04f;
            list.AddRange(Enumerable.Range(0, maxCountInRow)
                .Select(i => (Vector2)topRowMinPoint + i * _minXDistanceBetweenHolders * Vector2.right));

            var lowRowMinPoint = transform.position - Vector3.up * (rowSpacing / 2f) -
                                 (((count - maxCountInRow) - 1) / 2f) * _minXDistanceBetweenHolders * Vector3.right -
                                 Vector3.up * 0.04f;
            list.AddRange(Enumerable.Range(0, count - maxCountInRow)
                .Select(i => (Vector2)lowRowMinPoint + i * _minXDistanceBetweenHolders * Vector2.right));

            return list;
        }

        private BottleController InitializeBottle()
        {
            _obj = Instantiate(GameManager.SelectedBottle, Vector3.zero, Quaternion.identity);

            var objBottleControllerScript = _obj.GetComponent<BottleController>();

            objBottleControllerScript.BottleData.BottleSorted = false;

            return objBottleControllerScript;
        }
        bool IsPortrait()
        {
            return Screen.height > Screen.width;
        }
    }
}
