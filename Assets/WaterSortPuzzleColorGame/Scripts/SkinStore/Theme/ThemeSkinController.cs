using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace WaterSortPuzzleGame
{
    public class ThemeSkinController : MonoBehaviour
    {
        private static readonly string saveSelectedId = "SkinThemeId";
        private static readonly string saveUnlockIds = "ThemeUnlocks";

        [Header ("Skin Theme")]
        [SerializeField] ThemeSkinDatabase themeSkinDatabase;
        [SerializeField] private SpriteRenderer backgroundRenderer;
        public ThemeSkinDatabase ThemeSkinDatabase => themeSkinDatabase;

        private ThemeUnlockSave unlockSave = new ThemeUnlockSave();

        public event System.Action<string, string> OnSelectionChanged;

        private string selectedSkinThemeId;
        public string SelectedSkinThemeId => selectedSkinThemeId;

        public event Action OnBackgroundChange;
        
        private void OnEnable()
        {
           // ScreenResizeManager.OnScreenSizeChanged += OnScreenSizeChanged;
        }
        private void OnDisable()
        {
           // ScreenResizeManager.OnScreenSizeChanged -= OnScreenSizeChanged;
        }
        public void Init()
        {
           // Instance = this;

            LoadUnlocks();

            selectedSkinThemeId = PrefManager.GetString(saveSelectedId, "");

            if (string.IsNullOrEmpty(selectedSkinThemeId))
            {
                ThemeSkinData defaultSkin = GetDefaultBackgroundSkin();
                selectedSkinThemeId = defaultSkin.id;

                if (!IsUnlocked(defaultSkin))
                {
                    UnlockTheme(defaultSkin);
                }
            }

            ApplySelectedBackground();
        }
        //private void OnScreenSizeChanged()
        //{
        //    FitBackground();
        //}
        //private void FitBackground()
        //{
        //    Camera cam = Camera.main;

        //    // Get world size of sprite (before scaling)
        //    float spriteWidth = backgroundRenderer.sprite.bounds.size.x;
        //    float spriteHeight = backgroundRenderer.sprite.bounds.size.y;

        //    // Get camera world size
        //    float worldHeight = cam.orthographicSize * 2f;
        //    float worldWidth = worldHeight * cam.aspect;

        //    // Scale needed to fit
        //    float scaleX = worldWidth / spriteWidth;
        //    float scaleY = worldHeight / spriteHeight;

        //    // Pick the larger so it fills screen fully
        //    float finalScale = Mathf.Max(scaleX, scaleY);

        //    backgroundRenderer.transform.localScale = new Vector3(finalScale, finalScale, 1f);
        //}
        private ThemeSkinData GetDefaultBackgroundSkin()
        {
            var skins = themeSkinDatabase.skins;

            // Prefer LevelBased skin that unlocks at Level 1
            var level1Skin = skins.Find(s => s.unlockType == UnlockType.LevelBased && s.unlockValue <= 1);
            if (level1Skin != null)
            {
                return level1Skin;
            }

            // Otherwise, just return the first skin
            return skins[0];
        }
        public void ApplySelectedBackground()
        {
            Sprite sprite = GetSelecedThemeImage();
            if (sprite != null)
            {
                backgroundRenderer.sprite = sprite;
                OnBackgroundChange?.Invoke();
            }
        }
        public Sprite GetSelecedThemeImage()
        {
            var data = themeSkinDatabase.skins.Find(x => x.id == selectedSkinThemeId);
            if (data != null)
            {
                return data.image;
            }
            return null;
        }
        public void SelectTheme(ThemeSkinData data)
        {
            string previousId = selectedSkinThemeId;

            selectedSkinThemeId = data.id;
            PrefManager.SetString(saveSelectedId, selectedSkinThemeId);
            PlayerPrefs.Save();

            ApplySelectedBackground();

            OnSelectionChanged?.Invoke(previousId, selectedSkinThemeId);
        }

        private void LoadUnlocks()
        {
            string json = PlayerPrefs.GetString(saveUnlockIds, "");
            if (!string.IsNullOrEmpty(json))
            {
                unlockSave = JsonUtility.FromJson<ThemeUnlockSave>(json);
            }
        }
        public bool TryUnlockAndSelect(ThemeSkinData data)
        {
            if (IsUnlocked(data))
            {
                SelectTheme(data);
                return true;
            }

            if (CanUnlock(data))
            {
                UnlockTheme(data);
                SelectTheme(data);
                return true;
            }

            // Could not unlock
            return false;
        }

        private bool CanUnlock(ThemeSkinData data)
        {
            if (data.unlockType == UnlockType.LevelBased)
            {
                return (GameManager.LevelIndex + 1) >= data.unlockValue;
            }
            else if (data.unlockType == UnlockType.CoinBased)
            {
                if (CoinManager.IsEnoughCoin(data.unlockValue))
                {
                    CoinManager.SubtractCoins(data.unlockValue);
                    
                    return true;
                }
                else
                {
                    BackHandler.RemoveRecentlyScreen(UIController.GetPage<ShopPanel>());
                    return false;
                }
                
            }
            return false;
        }
        public bool IsUnlocked(ThemeSkinData data)
        {
            return unlockSave.themeUnlockedIds.Contains(data.id);
        }

        public void UnlockTheme(ThemeSkinData data)
        {
            if (!unlockSave.themeUnlockedIds.Contains(data.id))
            {
                unlockSave.themeUnlockedIds.Add(data.id);
                SaveUnlocks();
            }
        }

        private void SaveUnlocks()
        {
            string json = JsonUtility.ToJson(unlockSave);
            PlayerPrefs.SetString(saveUnlockIds, json);
            PlayerPrefs.Save();
        }
        public void UpdateAutoUnlocks()
        {
            int playerLevel = (GameManager.LevelIndex + 1);

            foreach (var skin in themeSkinDatabase.skins)
            {
                if (skin.unlockType == UnlockType.LevelBased && playerLevel >= skin.unlockValue)
                {
                    if (!IsUnlocked(skin))
                    {
                        UnlockTheme(skin);
                    }
                }
            }
        }
        public ThemeSkinData CheckForNewUnlockedSkin()
        {
            if (GameManager.LevelIndex == 0)
            {
                return null;
            }


            var skins = themeSkinDatabase.skins;

            foreach (var skin in skins)
            {
                if (skin.unlockType == UnlockType.LevelBased &&
                    (GameManager.LevelIndex + 1) >= skin.unlockValue &&
                    !IsUnlocked(skin))
                {
                    UnlockTheme(skin);
                    return skin; // Return first newly unlocked skin
                }
            }

            return null; // No new unlocked skin
        }
        public ThemeSkinData GetNextUnlock()
        {
            int currentLevel = GameManager.LevelIndex + 1;

            return themeSkinDatabase.skins
                .Where(t => t.unlockType == UnlockType.LevelBased
                            && t.unlockValue > currentLevel
                            && !IsUnlocked(t))
                .OrderBy(t => t.unlockValue)
                .FirstOrDefault();
        }
    }
}
