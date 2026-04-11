using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using WaterSortPuzzleGame.Enum;

namespace WaterSortPuzzleGame
{
    public class SuccessErrorPanel : UIPage
    {
        [SerializeField] private ScaleAnimation animPopup;

        [SerializeField] private Button successBack;
        [SerializeField] Button closeButton;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private Image image;
        [SerializeField] private SuccessErrorDatabase successErrorDatabase;
        public override void Init()
        {
            successBack.onClick.AddListener(BackgroundClick);
            closeButton.onClick.AddListener(BackClick);
        }

        public override void PlayShowAnimation()
        {
            animPopup.Show();
        }
        public override void PlayHideAnimation()
        {
            animPopup.Hide();
        }
        public void SetData(ToasterState type, string errorMessge = null)
        {
            var result = successErrorDatabase.SuccessErrors.Find(item => item.type == type);

            if (result == null) return;

            if (errorMessge != null)
            {
                result.message = errorMessge;
            }
            title.text = result.title;
            description.text = result.message;
            image.sprite = result.sprite;

        }
    }
}
