using WaterSortPuzzleGame.BottleCodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    public class BoosterController : MonoBehaviour
    {
        public static BoosterController Instance;

        [SerializeField] BoosterDatabase database;
        [SerializeField] LevelPanel levelPanel;
        public static BoosterUIController BoosterUIController { get; private set; }

        public static BoosterBehavior[] ActiveBoosters { get; private set; }
        
        public static BoosterBehavior SelectedBooster { get; private set; }

        private static Dictionary<BoosterType, BoosterBehavior> boostersLink;

        public delegate void BoosterCallback(BoosterType boosterType);

        private Transform behaviorsContainer;

        public void Init()
        {
            Instance = this;

            behaviorsContainer = new GameObject("[BOOSTER]").transform;
            behaviorsContainer.gameObject.isStatic = true;

            BoosterSettings[] boosterSettings = database.Boosters;


            // RunBoosterMigration  start

            // --- Booster Migration (v1.0.5 or earlier) ---
            // This function migrates old booster data (from v1.0.5 or earlier) 
            // to the new ScriptableObject system introduced in v1.0.4. 
            // Keep this line only if updating an existing live version from v1.0.5 or earlier. 
            // For fresh releases or versions >= v1.0.6, it has no effect and can be removed.
            RunBoosterMigration(boosterSettings);

            // RunBoosterMigration end

            ActiveBoosters = new BoosterBehavior[boosterSettings.Length];
            boostersLink = new Dictionary<BoosterType, BoosterBehavior>();

            for (int i = 0; i < ActiveBoosters.Length; i++)
            {
                // Initialise boosters
                boosterSettings[i].Init();

                // Spawn behavior object 
                GameObject boosterBehaviorObject = Instantiate(boosterSettings[i].BehaviorPrefab, behaviorsContainer);

                BoosterBehavior boosterBehavior = boosterBehaviorObject.GetComponent<BoosterBehavior>();
                boosterBehavior.InitialiseSettings(boosterSettings[i]);
                boosterBehavior.Init();

                ActiveBoosters[i] = boosterBehavior;

                // Add booster to dictionary
                boostersLink.Add(ActiveBoosters[i].Settings.Type, ActiveBoosters[i]);
            }

            BoosterUIController = levelPanel.BoosterUIController;
            BoosterUIController.Initialise();
        }
        public void RunBoosterMigration(BoosterSettings[] allBoosters)
        {
            string MigrationKey = "BoosterMigrationDone";

            if (PrefManager.GetBool(MigrationKey, false)) return; // already migrated


            // Map old keys to new BoosterType
            Dictionary<string, BoosterType> oldKeys = new Dictionary<string, BoosterType>
            {
                { "RemainingUndo", BoosterType.Undo },
                { "EmptyTubes", BoosterType.AddTube }
            };
            foreach (var kvp in oldKeys)
            {
                string oldKey = kvp.Key;
                BoosterType type = kvp.Value;

                if (PrefManager.HasKey(oldKey))
                {
                    int value = PrefManager.GetInt(oldKey, 0);
                    PrefManager.SetInt($"Booster_{type}", value);
                }
              
            }
            foreach (var booster in allBoosters)
            {
                if (booster.UnlockLevel != 0 && GameManager.LevelIndex + 1 >= booster.UnlockLevel)
                {
                    booster.IsUnlocked = true;
                }
            }

            // Mark migration complete
            PrefManager.SetBool(MigrationKey, true);
        }
        public static bool SelectBooster(BoosterType boosterType)
        {
            if (boostersLink.ContainsKey(boosterType))
            {
                BoosterBehavior boosterBehavior = boostersLink[boosterType];

                if (SelectedBooster == boosterBehavior)
                {
                    SelectedBooster.OnDeselected();
                    SelectedBooster = null;

                    return false;
                }

                if (!boosterBehavior.IsBusy)
                {
                    if (SelectedBooster != null)
                    {
                        SelectedBooster.OnDeselected();
                        SelectedBooster = null;
                    }

                    SelectedBooster = boosterBehavior;
                    boosterBehavior.OnSelected();

                    return true;
                }
            }
            else
            {
                Debug.LogWarning(string.Format("Booster with type {0} isn't registered.", boosterType));
            }

            return false;
        }
        public static void DeselectBooster()
        {
            if (SelectedBooster != null)
            {
                SelectedBooster.OnDeselected(skipStackRemoval: true);
                SelectedBooster = null;
            }
        }
        public static void ApplyToElement()
        {
            if (SelectedBooster != null)
            {
                if (SelectedBooster.ApplyToElement())
                {
                    if (!SelectedBooster.IsBusy)
                    {
                        BoosterSettings settings = SelectedBooster.Settings;

                        settings.Save--;

                        SelectedBooster.OnDeselected();
                        SelectedBooster = null;
                    }
                }
            }
        }
        public static void UnlockBooster(BoosterType boosterType)
        {
            if (boostersLink.ContainsKey(boosterType))
            {
                BoosterBehavior boosterBehavior = boostersLink[boosterType];
                BoosterSettings settings = boosterBehavior.Settings;

                if (!settings.IsUnlocked)
                {
                    settings.IsUnlocked = true;
                }
            }
        }
        public void ShowBoosterClaimAnimation(BoosterType boosterType)
        {
            if (boostersLink.ContainsKey(boosterType))
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.purchaseComplete);

                BoosterBehavior boosterBehavior = boostersLink[boosterType];
                BoosterSettings settings = boosterBehavior.Settings;

                settings.Save += settings.NoOfClaim;
                BoosterUIController.RedrawPanels();
            }
        }
        public static BoosterBehavior GetBoosterBehavior(BoosterType boosterType)
        {
            if (boostersLink.ContainsKey(boosterType))
            {
                BoosterBehavior boosterBehavior = boostersLink[boosterType];
                return boosterBehavior;
            }
            return null;

        }
        public static void IncrementBooster(BoosterType boosterType, int amount)
        {
            if (boostersLink.ContainsKey(boosterType))
            {
                BoosterBehavior boosterBehavior = boostersLink[boosterType];
                BoosterSettings settings = boosterBehavior.Settings;
                settings.Save += amount;
                if (!settings.IsUnlocked)
                {
                    settings.IsUnlocked = true;
                }
            }
        }
       
        public bool PurchaseCoinBaseBooster(BoosterType boosterType)
        {
            if (boostersLink.ContainsKey(boosterType))
            {
                BoosterBehavior boosterBehavior = boostersLink[boosterType];
                BoosterSettings settings = boosterBehavior.Settings;
                if (settings.HasEnoughCurrency())
                {
                    CoinManager.SubtractCoins(settings.Coins);

                    settings.Save += settings.CoinReward;
                    BoosterUIController.RedrawPanels();
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.purchaseComplete);
                    return true;
                }
                else
                {
                    UIController.HidePage<PurchaseBoosterPanel>(() =>
                    {
                        UIController.ShowPage<ShopPanel>();
                    });
                    return false;
                }
            }
            else
            {
                Debug.LogWarning(string.Format("[Booster]: Booster with type {0} isn't registered.", boosterType));
            }

            return false;
        }
        public void PurchaseAdsBaseBooster(BoosterType boosterType)
        {
            if (boostersLink.ContainsKey(boosterType))
            {
                BoosterBehavior boosterBehavior = boostersLink[boosterType];
                BoosterSettings settings = boosterBehavior.Settings;
                settings.Save += settings.AdsReward;
                BoosterUIController.RedrawPanels();
                AudioManager.Instance.PlaySFX(AudioManager.Instance.purchaseComplete);
            }
            else
            {
                Debug.LogWarning(string.Format("[Booster]: Booster with type {0} isn't registered.", boosterType));
            }

        }
        
    }
}
