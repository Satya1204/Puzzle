using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleApp.UI
{
    public class MatchObjectsLevelItem : MonoBehaviour
    {
        [SerializeField] Button _button;
        [SerializeField] GameObject _completedImage;
        [SerializeField] GameObject _incompleteImage;
        [SerializeField] TextMeshProUGUI _label;

        public event Action<int> Clicked;

        int _levelIndex = -1;

        void Awake()
        {
            if (_button == null)
                _button = GetComponent<Button>();
        }

        void OnEnable()
        {
            if (_button != null)
                _button.onClick.AddListener(OnPressed);
        }

        void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnPressed);
        }

        public void Bind(int levelIndex, bool isCompleted)
        {
            _levelIndex = levelIndex;

            if (_label != null)
                _label.text = (levelIndex + 1).ToString();

            SetCompleted(isCompleted);
        }

        public void SetCompleted(bool isCompleted)
        {
            if (_completedImage != null)
                _completedImage.SetActive(isCompleted);

            if (_incompleteImage != null)
                _incompleteImage.SetActive(!isCompleted);
        }

        void OnPressed() => Clicked?.Invoke(_levelIndex);
    }
}
