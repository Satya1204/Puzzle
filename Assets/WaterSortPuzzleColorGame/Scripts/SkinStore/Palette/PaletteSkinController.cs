using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace WaterSortPuzzleGame
{
    public class PaletteSkinController : MonoBehaviour
    {
        private static readonly string saveSelectedId = "SkinPaletteId";
        private static readonly string saveUnlockIds = "PaletteUnlocks";

        [Header ("Skin Palette")]
        [SerializeField] PaletteSkinDatabase paletteSkinDatabase;
        public PaletteSkinDatabase PaletteSkinDatabase => paletteSkinDatabase;

        private PaletteUnlockSave unlockSave = new PaletteUnlockSave();

        public event System.Action<string, string> OnSelectionChanged;

        private string selectedSkinPaletteId;
        public string SelectedSkinPaletteId => selectedSkinPaletteId;
        
        public void Init()
        {

            LoadUnlocks();

            selectedSkinPaletteId = PrefManager.GetString(saveSelectedId, "");

            if (string.IsNullOrEmpty(selectedSkinPaletteId))
            {
                PaletteSkinData defaultSkin = GetDefaultPaletteSkin();
                selectedSkinPaletteId = defaultSkin.id;

                if (!IsUnlocked(defaultSkin))
                {
                    UnlockPalette(defaultSkin);
                }
            }

            ApplySelectedPalette();
        }
        private PaletteSkinData GetDefaultPaletteSkin()
        {
            var skins = paletteSkinDatabase.skins;

            // Prefer LevelBased skin that unlocks at Level 1
            var level1Skin = skins.Find(s => s.unlockType == UnlockType.LevelBased && s.unlockValue <= 1);
            if (level1Skin != null)
            {
                return level1Skin;
            }

            // Otherwise, just return the first skin
            return skins[0];
        }
        public void ApplySelectedPalette()
        {
            Colors colorDatabase = GetSelecedPalette();
            if (colorDatabase != null)
            {
                GameManager.SetSelectedPalette(colorDatabase);
            }
        }
        public Colors GetSelecedPalette()
        {
            var data = paletteSkinDatabase.skins.Find(x => x.id == selectedSkinPaletteId);
            if (data != null)
            {
                return data.colorPalette;
            }
            return null;
        }
        public void SelectPalette(PaletteSkinData data)
        {
            string previousId = selectedSkinPaletteId;

            selectedSkinPaletteId = data.id;
            PrefManager.SetString(saveSelectedId, selectedSkinPaletteId);
            PlayerPrefs.Save();

            ApplySelectedPalette();

            OnSelectionChanged?.Invoke(previousId, selectedSkinPaletteId);
        }

        private void LoadUnlocks()
        {
            string json = PlayerPrefs.GetString(saveUnlockIds, "");
            if (!string.IsNullOrEmpty(json))
            {
                unlockSave = JsonUtility.FromJson<PaletteUnlockSave>(json);
            }
        }
        public bool TryUnlockAndSelect(PaletteSkinData data)
        {
            if (IsUnlocked(data))
            {
                SelectPalette(data);
                return true;
            }

            if (CanUnlock(data))
            {
                UnlockPalette(data);
                SelectPalette(data);
                return true;
            }

            // Could not unlock
            return false;
        }

        private bool CanUnlock(PaletteSkinData data)
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
        public bool IsUnlocked(PaletteSkinData data)
        {
            return unlockSave.paletteUnlockedIds.Contains(data.id);
        }

        private void UnlockPalette(PaletteSkinData data)
        {
            if (!unlockSave.paletteUnlockedIds.Contains(data.id))
            {
                unlockSave.paletteUnlockedIds.Add(data.id);
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

            foreach (var skin in paletteSkinDatabase.skins)
            {
                if (skin.unlockType == UnlockType.LevelBased && playerLevel >= skin.unlockValue)
                {
                    if (!IsUnlocked(skin))
                    {
                        UnlockPalette(skin);
                    }
                }
            }
        }
        public PaletteSkinData CheckForNewUnlockedSkin()
        {
            if (GameManager.LevelIndex == 0)
            {
                return null;
            }


            var skins = paletteSkinDatabase.skins;

            foreach (var skin in skins)
            {
                if (skin.unlockType == UnlockType.LevelBased &&
                    (GameManager.LevelIndex + 1) >= skin.unlockValue &&
                    !IsUnlocked(skin))
                {
                    UnlockPalette(skin);
                    return skin; // Return first newly unlocked skin
                }
            }

            return null; // No new unlocked skin
        }
    }
}
