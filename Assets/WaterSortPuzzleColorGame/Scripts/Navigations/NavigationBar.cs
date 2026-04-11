using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterSortPuzzleGame.Enum;
using UnityEngine.UI;
using DG.Tweening;

namespace WaterSortPuzzleGame.Navigations
{
    public class NavigationBar : MonoBehaviour
    {
        public static NavigationBar Instance;
        [SerializeField] private List<NavigationItem> navigationItems;

        private Tween navbarTween;
        private NavigationItem selectedItem;
        private RectTransform navbarRectTransform => GetComponent<RectTransform>();
        private float originalIconY = 30f;
        private float originalTextY = -52f;
        private void Awake()
        {
            if (Instance == null) Instance = this;
        }
        private void OnEnable()
        {
            ShowNavigationAnimation();
            UpdateSizesAndSelection();
        }

        private void OnDisable()
        {
            HideNavigation();
        }

        private void Start()
        {
            if (navigationItems.Count > 0)
            {
                selectedItem = navigationItems.Find(item => item.type == NavigationType.Home);
                if (selectedItem != null)
                {
                    foreach (var item in navigationItems)
                    {
                        item.button.onClick.AddListener(() => OnButtonSelected(item));
                    }

                    UpdateLayout();
                }
            }
        }

        private void ShowNavigationAnimation()
        {
            navbarTween?.Kill();
            navbarRectTransform.anchoredPosition = new Vector2(0, -250f);
            navbarTween = navbarRectTransform.DOAnchorPosY(0f, 0.3f).SetEase(Ease.OutBack);
        }

        private void HideNavigation()
        {
            Vector3 navBarposition = navbarRectTransform.anchoredPosition;
            navBarposition.y = -250f;
            navbarRectTransform.anchoredPosition = navBarposition;

        }
        public void SelectTab(NavigationType type)
        {
            var item = navigationItems.Find(i => i.type == type);
            if (item != null)
            {
                OnButtonSelected(item);
            }
        }
        public void OnButtonSelected(NavigationItem item)
        {
            
            if (selectedItem == item) return;

            if(item.type != NavigationType.Collection) AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);

            selectedItem = item;
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            UpdatePanels();
            UpdateSizesAndSelection();
        }
        private void UpdatePanels()
        {
            NavigationBarUtils.HideAllPanels();
            NavigationBarUtils.ShowPanel(selectedItem.type);
        }
        private void UpdateSizesAndSelection()
        {
            if (selectedItem == null) return;

            UpdateVisualStates();
        }

        private void UpdateVisualStates()
        {
            foreach (var item in navigationItems)
            {
                bool isSelected = item == selectedItem;
                NavigationAnimator.AnimateSelection(item, isSelected,originalIconY, originalTextY);
            }
        }
        private void OnDestroy()
        {
            navbarTween?.Kill();
        }

    }
   
}