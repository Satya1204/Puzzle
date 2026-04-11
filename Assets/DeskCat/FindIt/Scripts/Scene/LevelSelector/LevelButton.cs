using DeskCat.FindIt.Scripts.Core.Model;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DeskCat.FindIt.Scripts.Scene.LevelSelector
{
    public class LevelButton : MonoBehaviour
    {
        public string LevelName;
        private Button button;
        public bool isActive = true;

        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => SceneManager.LoadScene(LevelName));
            if (!GlobalSetting.LevelActiveDic.TryAdd(LevelName, isActive))
            {
                isActive = GlobalSetting.LevelActiveDic[LevelName];
            }
            SetLevelActive(isActive);
        }

        public void SetLevelActive(bool value)
        {
            isActive = value;
            button.interactable = isActive;
        }
    }
}