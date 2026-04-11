using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace WaterSortPuzzleGame.UI
{
    public class CanvasController : MonoBehaviour
    {
        [SerializeField]
        private GameObject navigationPanel;
        [SerializeField]
        private Image canvasImage;

        private CanvasScaler canvasScaler;
        private void OnEnable()
        {
            EventManager.ChangeScreen += ChangeScreen;
            ScreenResizeManager.OnScreenSizeChanged += UpdateCanvasMatch;
        }

        private void OnDisable()
        {
            EventManager.ChangeScreen -= ChangeScreen;
            ScreenResizeManager.OnScreenSizeChanged -= UpdateCanvasMatch;
        }
        private void Start()
        {
            if (SkinManager.Instance?.ThemeSkinController != null)
            {
                SkinManager.Instance.ThemeSkinController.OnBackgroundChange += OnBackgroundChange;
                OnBackgroundChange();
            }
        }
        private void ChangeScreen()
        {
            if (GameManager.IsLevelScreen)
            {
                navigationPanel.SetActive(false);
                if (canvasImage != null)
                    canvasImage.enabled = false;
            }
            else
            {
                navigationPanel.SetActive(true);
                if (canvasImage != null)
                    canvasImage.enabled = true;
            }
        }
        private void UpdateCanvasMatch()
        {
            if (canvasScaler == null) canvasScaler = GetComponent<CanvasScaler>();

            bool isTablet = GameManager.Instance.IsWideScreen();
            canvasScaler.matchWidthOrHeight = isTablet ? 1 : 0;

            StartCoroutine(DelayedInit());
        }
        private IEnumerator DelayedInit()
        {
            yield return new WaitForEndOfFrame(); // Waits 1 frame so layout is updated

            ShopPanel shopUI = UIController.GetPage<ShopPanel>();
            shopUI?.InitTransfrom();

            CollectionPanel collectonUI = UIController.GetPage<CollectionPanel>();
            collectonUI?.InitTransfrom();
        }
        private void OnBackgroundChange()
        {
            Sprite sprite = SkinManager.Instance.ThemeSkinController.GetSelecedThemeImage();
            if (sprite != null)
            {
                canvasImage.sprite = sprite;
            }
        }
    }
}