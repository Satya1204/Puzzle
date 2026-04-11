using UnityEngine;
using UnityEngine.UI;
using WaterSortPuzzleGame.Enum;
using WaterSortPuzzleGame.Navigations;

namespace WaterSortPuzzleGame
{
    public class MapPanel : UIPage
    {
        [SerializeField] private GameObject _levelTileUIPrefab;
        [SerializeField] private RectTransform _content;
        [SerializeField] private int _totalLevel;
        private GameObject newData;
        private int totalLevel;
        private int startLevel = 0;
        private float _lastContentY;
        public void OnEnable()
        {
            EventManager.LoadMapLevels += LoadMapLevels;
        }
        public void OnDisable()
        {
            EventManager.LoadMapLevels -= LoadMapLevels;
        }
        private void Start()
        {
            totalLevel = _totalLevel; // initialize chunk size
            _lastContentY = float.MinValue;
        }

        public override void Init()
        {
        }
        public override void PlayShowAnimation()
        {
        }
        public override void PlayHideAnimation()
        {
        }
        public void ClearLevels()
        {
            startLevel = 0;
            totalLevel = _totalLevel;
            _content.anchoredPosition = new Vector2(0, 0);
            while (_content.childCount > 0)
            {
                DestroyImmediate(_content.GetChild(0).gameObject);
            }
        }
        private void LoadMapLevels()
        {
            ClearLevels();
            GenerateLevel();

        }
        private void GenerateLevel()
        {
            int columns = GetColumnCount(); 
            int rowsToGenerate = GetVisibleRows() + 3;
            int itemsToGenerate = columns * rowsToGenerate; 

            for (int i = startLevel; i < startLevel + itemsToGenerate; i++)
            {
                if (i == 999)
                {
                    _content.GetComponent<GridLayoutGroup>().padding.bottom += 260;
                }
                else
                {
                    _content.GetComponent<GridLayoutGroup>().padding.bottom = 10;
                }

                newData = Instantiate(_levelTileUIPrefab, _content);
                if (newData.TryGetComponent<LevelMapPrefab>(out var item))
                {
                    item.SetData(i);
                }
            }

            startLevel += itemsToGenerate; 

        }
        private void OnScroll()
        {
            int currentTileCount = _content.childCount;
            int columns = GetColumnCount();

            var grid = _content.GetComponent<GridLayoutGroup>();
            float tileHeight = grid.cellSize.y + grid.spacing.y;
            float totalContentHeight = (currentTileCount / (float)columns) * tileHeight;
            float viewHeight = _content.parent.GetComponent<RectTransform>().rect.height;
            float scrollY = _content.anchoredPosition.y;

            
            if (scrollY + (tileHeight * 10) > totalContentHeight - viewHeight)
            {
                GenerateLevel();
            }
        }


        private int GetColumnCount()
        {
            var grid = _content.GetComponent<GridLayoutGroup>();
            float totalWidth = _content.rect.width + grid.spacing.x;
            float tileWidth = grid.cellSize.x + grid.spacing.x;
            int columns = Mathf.FloorToInt(totalWidth / tileWidth);
            return Mathf.Max(columns, 1); 
        }

        private int GetVisibleRows()
        {
            var grid = _content.GetComponent<GridLayoutGroup>();
            float tileHeight = grid.cellSize.y + grid.spacing.y;
            float viewHeight = _content.parent.GetComponent<RectTransform>().rect.height;
            return Mathf.CeilToInt(viewHeight / tileHeight); 
        }

        void Update()
        {
            float currentY = _content.anchoredPosition.y;
            if (!Mathf.Approximately(currentY, _lastContentY))
            {
                _lastContentY = currentY;
                OnScroll(); 
            }
        }
        public override void OnPageResume()
        {
            NavigationBar.Instance.SelectTab(NavigationType.Map);
        }
    }
}