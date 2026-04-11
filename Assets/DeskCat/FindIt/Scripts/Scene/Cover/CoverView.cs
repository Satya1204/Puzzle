using DeskCat.FindIt.Scripts.Core.Model;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DeskCat.FindIt.Scripts.Scene.Cover
{
    public class CoverView : MonoBehaviour
    {
        public Button PlayBtn;
        public Button SettingBtn;
        public GameObject SettingPanel;

        public string LevelSelectorSceneName = "LevelSelector";

        private void Start()
        {
            PlayBtn.onClick.AddListener(PlayBtnFunction);
            SettingBtn.onClick.AddListener(SettingBtnFunction);
        }

        private void PlayBtnFunction()
        {
            SceneManager.LoadScene(LevelSelectorSceneName);
        }

        private void SettingBtnFunction()
        {
            SettingPanel.SetActive(true);
        }
    }
}