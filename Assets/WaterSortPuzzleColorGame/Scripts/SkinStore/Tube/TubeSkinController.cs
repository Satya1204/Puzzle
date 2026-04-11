using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace WaterSortPuzzleGame
{
    public class TubeSkinController : MonoBehaviour
    {
        private static readonly string saveSelectedId = "SkinTubeId";
        private static readonly string saveUnlockIds = "TubeUnlocks";

        [Header ("Skin Tube")]
        [SerializeField] TubeSkinDatabase tubeSkinDatabase;
        public TubeSkinDatabase TubeSkinDatabase => tubeSkinDatabase;

        private TubeUnlockSave unlockSave = new TubeUnlockSave();

        public event System.Action<string, string> OnSelectionChanged;

        private string selectedSkinTubeId;
        public string SelectedSkinTubeId => selectedSkinTubeId;
        public void Init()
        {
            LoadUnlocks();

            selectedSkinTubeId = PrefManager.GetString(saveSelectedId, "");

            if (string.IsNullOrEmpty(selectedSkinTubeId))
            {
                TubeSkinData defaultSkin = GetDefaultTubeSkin();
                selectedSkinTubeId = defaultSkin.id;

                if (!IsUnlocked(defaultSkin))
                {
                    UnlockTube(defaultSkin);
                }
            }

            ApplySelectedTube();
        }
        private TubeSkinData GetDefaultTubeSkin()
        {
            var skins = tubeSkinDatabase.skins;

            // Prefer LevelBased skin that unlocks at Level 1
            var level1Skin = skins.Find(s => s.unlockType == UnlockType.LevelBased && s.unlockValue <= 1);
            if (level1Skin != null)
            {
                return level1Skin;
            }

            // Otherwise, just return the first skin
            return skins[0];
        }
        public void ApplySelectedTube()
        {
            GameObject bottlePref = GetSelecedTube();
            if (bottlePref != null)
            {
                GameManager.SetSelectedBottle(bottlePref);
            }
        }
        public GameObject GetSelecedTube()
        {
            var data = tubeSkinDatabase.skins.Find(x => x.id == selectedSkinTubeId);
            if (data != null)
            {
                return data.bottlePrefab;
            }
            return null;
        }
        public void SelectTube(TubeSkinData data)
        {
            string previousId = selectedSkinTubeId;

            selectedSkinTubeId = data.id;
            PrefManager.SetString(saveSelectedId, selectedSkinTubeId);
            PlayerPrefs.Save();

            ApplySelectedTube();

            OnSelectionChanged?.Invoke(previousId, selectedSkinTubeId);
        }

        private void LoadUnlocks()
        {
            string json = PlayerPrefs.GetString(saveUnlockIds, "");
            if (!string.IsNullOrEmpty(json))
            {
                unlockSave = JsonUtility.FromJson<TubeUnlockSave>(json);
            }
        }
        public bool TryUnlockAndSelect(TubeSkinData data)
        {
            if (IsUnlocked(data))
            {
                SelectTube(data);
                return true;
            }

            if (CanUnlock(data))
            {
                UnlockTube(data);
                SelectTube(data);
                return true;
            }

            // Could not unlock
            return false;
        }

        private bool CanUnlock(TubeSkinData data)
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
        public bool IsUnlocked(TubeSkinData data)
        {
            return unlockSave.tubeUnlockedIds.Contains(data.id);
        }

        public void UnlockTube(TubeSkinData data)
        {
            if (!unlockSave.tubeUnlockedIds.Contains(data.id))
            {
                unlockSave.tubeUnlockedIds.Add(data.id);
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
            int playerLevel = GameManager.LevelIndex + 1;

            foreach (var skin in tubeSkinDatabase.skins)
            {
                if (skin.unlockType == UnlockType.LevelBased && playerLevel >= skin.unlockValue)
                {
                    if (!IsUnlocked(skin))
                    {
                        UnlockTube(skin);
                    }
                }
            }
        }
        public TubeSkinData CheckForNewUnlockedSkin()
        {
            if (GameManager.LevelIndex == 0)
            {
                return null;
            }


            var skins = tubeSkinDatabase.skins;

            foreach (var skin in skins)
            {
                if (skin.unlockType == UnlockType.LevelBased &&
                    (GameManager.LevelIndex + 1) >= skin.unlockValue &&
                    !IsUnlocked(skin))
                {
                    UnlockTube(skin);
                    return skin; // Return first newly unlocked skin
                }
            }

            return null; // No new unlocked skin
        }
        public TubeSkinData GetNextUnlock()
        {
            int currentLevel = GameManager.LevelIndex + 1;

            return tubeSkinDatabase.skins
                .Where(s => s.unlockType == UnlockType.LevelBased
                            && s.unlockValue > currentLevel
                            && !IsUnlocked(s))
                .OrderBy(s => s.unlockValue)
                .FirstOrDefault();
        }
    }
}
