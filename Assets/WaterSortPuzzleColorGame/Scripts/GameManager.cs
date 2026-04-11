using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using WaterSortPuzzleGame.BottleCodes;
using WaterSortPuzzleGame.Enum;

namespace WaterSortPuzzleGame
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance => instance;
        private void OnEnable()
        {
            EventManager.CheckIsLevelCompleted += CheckLevelIsCompleted;
            EventManager.RestartLevel += ClearInActionBottleList;
            EventManager.RestartLevel += ResetAllLineRenderers;
            EventManager.LoadNextLevel += ResetAllLineRenderers;
            EventManager.ChangeScreen += LoadScreen;
        }
        private void OnDisable()
        {
            EventManager.CheckIsLevelCompleted -= CheckLevelIsCompleted;
            EventManager.RestartLevel -= ClearInActionBottleList;
            EventManager.RestartLevel -= ResetAllLineRenderers;
            EventManager.LoadNextLevel -= ResetAllLineRenderers;
            EventManager.ChangeScreen -= LoadScreen;
        }
        [Header("UI Controller")]
        [SerializeField] UIController uiController;

        [Header("Game Settings")]
        [SerializeField] private GameSettings gameSettings;
        public GameSettings GameSettings => gameSettings;
        [Header("Camera")]
        [SerializeField] private Camera cameraUI;

        [Header("Tubes")] public List<BottleController> bottleControllers;

        [Header("Particles")][SerializeField] private GameObject confettiParticle;

        [Header("Material")][SerializeField] private Material mat;

        [Header("Line Renderer")]
        [SerializeField]
        private LineRenderer lineRenderer;

        [Header("Integers")] public int TotalColorAmount;

        [Header("Color Palettes List")] public List<Colors> listOfPalettes;
        [Header("Bottles")] public List<GameObject> listOfBottles;

        [Header("In Action")]
        [SerializeField]
        public List<BottleController> InActionBottleList = new List<BottleController>();

        [Header("Lines")] public Transform line1;
        public Transform line2;

        private ObjectPool<LineRenderer> _pool;
        private List<LineRenderer> _gettedLineRenderers = new List<LineRenderer>();


        [SerializeField] GameObject blockRayCast;
        public static GameState gameState = GameState.Pause;
        public static bool replayLevel = true;
        public GameObject ConfettiParticle => confettiParticle;
        public Material Mat => mat;
        [Header("Developer Panel")]
        [SerializeField] GameObject devPanel;

        private GameLoader gameLoader;
        private AdsManager adsManager;
        private BoosterController boosterController;
        private IAPInitModule iapInitModule;
        private SkinManager skinManager;
        public static int LevelIndex
        {
            get
            {
                return PrefManager.GetInt(PlayerPrefNames.LevelIndex, Instance.gameSettings.LevelIndex);
            }
            set
            {
                replayLevel = true;

                PrefManager.SetInt(PlayerPrefNames.LevelIndex, value);
                if (TotalCompletedLevelIndex != (LevelIndex - 1) && TotalCompletedLevelIndex < (LevelIndex - 1))
                {
                    TotalCompletedLevelIndex = LevelIndex - 1;
                    replayLevel = false;
                }
            }
        }
        public static int TotalCompletedLevelIndex
        {
            get
            {
                return PrefManager.GetInt(PlayerPrefNames.TotalCompletedLevelIndex, Instance.gameSettings.TotalCompletedLevelIndex);
            }
            set
            {
                PrefManager.SetInt(PlayerPrefNames.TotalCompletedLevelIndex, value);
            }
        }
       
        public static bool IsSoundEnable
        {
            get
            {
                return PrefManager.GetBool(PlayerPrefNames.SoundEnable, Instance.gameSettings.IsSoundEnable);
            }
            set
            {
                PrefManager.SetBool(PlayerPrefNames.SoundEnable, value);
                EventManager.ChangeSoundSetting?.Invoke();
            }

        }
        public static bool IsMusicEnable
        {
            get
            {
                return PrefManager.GetBool(PlayerPrefNames.MusicEnable, Instance.gameSettings.IsMusicEnable);
            }
            set
            {
                PrefManager.SetBool(PlayerPrefNames.MusicEnable, value);
                EventManager.ChangeMusicSetting?.Invoke();
            }

        }
       
        public static int TempEmptyTubes
        {
            get
            {
                return PrefManager.GetInt(PlayerPrefNames.TempEmptyTubes);
            }
            set
            {
                PrefManager.SetInt(PlayerPrefNames.TempEmptyTubes, value);
                EventManager.TempEmptyTubesChange?.Invoke();
            }
        }
        public static Colors SelectedPalettes { get; private set; }
        public static GameObject SelectedBottle { get; private set; }
        public static bool IsLevelScreen
        {
            get
            {
                return PrefManager.GetBool(PlayerPrefNames.IsLevelScreen);
            }
            set
            {
                PrefManager.SetBool(PlayerPrefNames.IsLevelScreen, value);
                Debug.Log("IsLevelScreen:" + IsLevelScreen.ToString());
                EventManager.ChangeScreen?.Invoke();
            }
        }
        
        public void Awake()
        {
            instance = this;

            gameObject.CacheComponent(out gameLoader);
            gameObject.CacheComponent(out adsManager);
            gameObject.CacheComponent(out boosterController);
            gameObject.CacheComponent(out iapInitModule);
            gameObject.CacheComponent(out skinManager);

            
            gameLoader.Init();
            skinManager.Init();
            iapInitModule.Init();
            boosterController.Init();
            adsManager.Init();
            uiController.Init();
            DevPanelEnabler.RegisterPanel(devPanel);
        }
        
        public void Start()
        {
            SetRaycastBlocker(false);
            DefineLineRendererPool();
            EventManager.ChangeScreen?.Invoke();
        }
        void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame) //if (Input.GetKeyDown(KeyCode.Escape))
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
                BackHandler.BackEvent();
            }
        }
        public void SetRaycastBlocker(bool active)
        {
            if (blockRayCast != null)
                blockRayCast.SetActive(active);
        }
        public static Color GetColorFromIndex(int index)
        {
            return (index < 0) ? Color.black : SelectedPalettes.colors[index].color;
        }
        public static int GetIndexFromColor(Color color)
        {
            return SelectedPalettes.colors.FindIndex(i => i.color == color);
        }

        private void DefineLineRendererPool()
        {
            _pool = new ObjectPool<LineRenderer>(() => { return Instantiate(lineRenderer); },
                lr => { lr.gameObject.SetActive(true); }, lr =>
                {
                    lr.gameObject.SetActive(false);
                    lr.enabled = false;
                },
                lr => { Destroy(lr.gameObject); }, true, 10);
        }

        private void CheckLevelIsCompleted()
        {
            var completedColorAmount = 0;

            foreach (var bottle in bottleControllers)
            {
                if (bottle.BottleData.BottleSorted)
                    completedColorAmount++;
            }

            if (completedColorAmount == TotalColorAmount)
            {
                if (gameState != GameState.Win)
                {
                    Instance.SetRaycastBlocker(true);
                    gameState = GameState.Win;
                    BackHandler.RemoveAllAboveLevelPanel();

                    LevelIndex++;
                    TempEmptyTubes = 0;
                    if (!replayLevel)
                    {
                        CoinManager.AddCoins(5);
                    }
                    StartCoroutine(AnimLevelCompleted());
                }
            }
        }
        IEnumerator AnimLevelCompleted()
        {
            DisableBottlesCollider();
            ClearInActionBottleList();

            EventManager.LevelCompleted?.Invoke();

            yield return new WaitForSeconds(0.5f);
            EventManager.WinEffect?.Invoke();
            yield return new WaitForSeconds(0.5f);

            
            UIController.ShowPage<LevelCompletePanel>();
        }
        // Call this from PaletteController
        public static void SetSelectedPalette(Colors palette)
        {
            SelectedPalettes = palette;

            if (IsLevelScreen) EventManager.CreatePrototypeOrLevel?.Invoke();
        }
        // Call this from TubeController
        public static void SetSelectedBottle(GameObject bottle)
        {
            SelectedBottle = bottle;

            if (IsLevelScreen) EventManager.CreatePrototypeOrLevel?.Invoke();
        }
        private void ClearInActionBottleList()
        {
            InActionBottleList.Clear();
        }

        private void DisableBottlesCollider()
        {
            foreach (var bottleController in bottleControllers)
            {
                bottleController.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        private void ResetAllLineRenderers()
        {
            if (_gettedLineRenderers.Count <= 0) return;

            foreach (var lineRenderer in _gettedLineRenderers)
            {
                lineRenderer.enabled = false;
                _pool.Release(lineRenderer);
            }

            _gettedLineRenderers.Clear();
        }

        public LineRenderer GetLineRenderer()
        {
            var lineRenderer = _pool.Get();
            _gettedLineRenderers.Add(lineRenderer);
            return lineRenderer;
        }

        public void ReleaseLineRenderer(LineRenderer lr)
        {
            if (!_gettedLineRenderers.Contains(lr)) return;

            _pool.Release(lr);
            _gettedLineRenderers.Remove(lr);
        }

        public static void LoadScreen()
        {
            if (IsLevelScreen)
            {
                gameState = GameState.Play;

                if (BackHandler.RecentScreen.Count == 0)
                {
                    UIController.ShowPage<HomePanel>(() =>
                    {
                        UIController.HidePage<HomePanel>(() =>
                        {
                            UIController.ShowPage<LevelPanel>();
                        });
                    });
                }
            }
            else
            {
                gameState = GameState.Pause;

                if (BackHandler.RecentScreen.Count > 0)
                {
                    GameObject lastToSecondItem = BackHandler.RecentScreen[^1].gameObject;
                    if (lastToSecondItem.gameObject.TryGetComponent<UIPage>(out var page))
                    {
                        Type type = UIController.GetPageTypeFromGameObject(lastToSecondItem.gameObject);

                        if (type != null)
                        {
                            UIController.ShowPageByType(type); 
                        }
                    }
                }

                Instance.ResetAllLineRenderers();
                EventManager.BackToHomeScreen?.Invoke();
            }

            EventManager.UpdateLevelText?.Invoke();
        }

        public static bool IsInternetConnection()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool IsWideScreen()
        {
#if UNITY_IOS
            bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
            if (deviceIsIpad)
                return true;

            return false;
#else
            return cameraUI.aspect > (9f / 16f);
#endif
        }
    }
}