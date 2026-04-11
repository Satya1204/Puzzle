using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WaterSortPuzzleGame.DataClass;
using WaterSortPuzzleGame.Enum;

namespace WaterSortPuzzleGame
{
    public class BoosterUIBehavior : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] GameObject defaultElementsObjects;
        [SerializeField] GameObject lockStateObject;
        [SerializeField] private GameObject boosterIcon;
        [SerializeField] private GameObject lockSprite;
        [SerializeField] private GameObject plusIcon;
        [SerializeField] private GameObject totalCount;
        [SerializeField] private TextMeshProUGUI unlockLevelText;


        protected BoosterBehavior behavior;
        public BoosterBehavior Behavior => behavior;

        protected BoosterSettings settings;
        public BoosterSettings Settings => settings;

        private bool isActive = false;
        public bool IsActive => isActive;

        private bool isLocked = false;
        public bool IsLocked => isLocked;

        private BoosterData boosterData;
        public BoosterData BoosterData => boosterData;
        private Button button;

        private Image boosterIconImage;
        private TextMeshProUGUI boosterText;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => OnButtonClicked());
        }
        private void GetComponents()
        {
            boosterIconImage = boosterIcon.GetComponent<Image>();
            boosterText = totalCount.GetComponentInChildren<TextMeshProUGUI>();
        }
        public void Initialise(BoosterBehavior boosterBehavior)
        {
            GetComponents();

            behavior = boosterBehavior;
            settings = boosterBehavior.Settings;
            isActive = false;
        }
        public void Activate()
        {
            isActive = true;
            gameObject.SetActive(true);
            unlockLevelText.text = $"Level {settings.UnlockLevel}";
            Redraw();
        }
        public void Disable()
        {
            isActive = false;

            gameObject.SetActive(false);
        }
        
        public void Redraw()
        {
            SetBlockState();
            int amount = settings.Save;
            if (amount > 0)
            {
                plusIcon.SetActive(false);
                totalCount.SetActive(true);
                boosterText.text = amount.ToString();
            }
            else
            {
                plusIcon.SetActive(true);
                totalCount.SetActive(false);
            }

            boosterIconImage.sprite = isLocked || !behavior.IsSelectable ? settings.InActiveIcon : settings.Icon;

            behavior.OnRedrawn();
        }
        public void SetBlockState()
        {
            isLocked = !settings.IsUnlocked;

            lockStateObject.SetActive(isLocked);

            defaultElementsObjects.SetActive(!isLocked);
        }

        public void OnButtonClicked()
        {
            

            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);

            if (!settings.IsUnlocked)
            {
                SuccessErrorPanel gameUI = UIController.GetPage<SuccessErrorPanel>();
                gameUI.SetData(ToasterState.LockFeature, $"This feature will unlock in Level {settings.UnlockLevel}");

                UIController.ShowPage<SuccessErrorPanel>();
                return;
            }


            if (settings.Save > 0)
            {
                if (behavior.IsSelectable)
                {
                    BoosterController.SelectBooster(settings.Type);
                }
            }
            else
            {
                PurchaseBoosterPanel gameUI = UIController.GetPage<PurchaseBoosterPanel>();
                gameUI.SetData(settings);

                UIController.ShowPage<PurchaseBoosterPanel>();
            }
        }
    }
}
