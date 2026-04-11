using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WaterSortPuzzleGame.Enum;
using WaterSortPuzzleGame.MainMenu;

namespace WaterSortPuzzleGame
{
    public class SettingPanel : UIPage
    {
        [SerializeField] private ScaleAnimation animPopup;

        [SerializeField] private Button settingBack;
        [SerializeField] private Button settingClose;

        [SerializeField] private Button _soundButton;
        [SerializeField] private Button _musicButton;
        [SerializeField] private Sprite[] _soundEnableAndDisableSprites;
        [SerializeField] private Sprite[] _musicEnableAndDisableSprites;
        [SerializeField] private Button collectionButton;
        [SerializeField] private Button shareAppButton;
        [SerializeField] private Button rateAppButton;
        [SerializeField] private Button moreAppButton;
        [SerializeField] private Button privacyButton;
        [SerializeField] private Button privacySetting;

        public void OnEnable()
        {
            EventManager.ChangeSoundSetting += ChangeSoundSetting;
            EventManager.ChangeMusicSetting += ChangeMusicSetting;
        }
        public void OnDisable()
        {
            EventManager.ChangeSoundSetting -= ChangeSoundSetting;
            EventManager.ChangeMusicSetting -= ChangeMusicSetting;
        }
        public override void Init()
        {
            settingBack.onClick.AddListener(BackgroundClick);
            settingClose.onClick.AddListener(BackClick);
            _soundButton.onClick.AddListener(OnClickSound);
            _musicButton.onClick.AddListener(OnClickMusic);
            collectionButton.onClick.AddListener(OnClickCollection);
            shareAppButton.onClick.AddListener(OnClickShareApp);
            rateAppButton.onClick.AddListener(OnClickRateThisApp);
            moreAppButton.onClick.AddListener(OnClickMoreApp);
            privacyButton.onClick.AddListener(OnClickPrivacy);
            privacySetting.onClick.AddListener(OnClickPrivacySetting);

            ChangeSoundSetting();
            ChangeMusicSetting();
        }
        public override void PlayShowAnimation()
        {
            CheckPrivacyStatus();
            animPopup.Show();
        }
        public override void PlayHideAnimation()
        {
            animPopup.Hide();
        }

        public void ChangeSoundSetting()
        {
            _soundButton.image.sprite = _soundEnableAndDisableSprites[GameManager.IsSoundEnable ? 0 : 1];
        }
        public void ChangeMusicSetting()
        {
            _musicButton.image.sprite = _musicEnableAndDisableSprites[GameManager.IsMusicEnable ? 0 : 1];
        }
        public void OnClickSound()
        {
            GameManager.IsSoundEnable = !GameManager.IsSoundEnable;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
        }
        public void OnClickMusic()
        {
            GameManager.IsMusicEnable = !GameManager.IsMusicEnable;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);
        }
        public void OnClickCollection()
        {
            UIController.HidePage<SettingPanel>(() =>
            {
                UIController.ShowPage<CollectionPanel>();
            });
        }
        public void OnClickShareApp()
        {
            StartCoroutine(ShareTextInAnroid());
        }

        public void OnClickRateThisApp()
        {
            string url = "http://play.google.com/store/apps/details?id=" + GameManager.Instance.GameSettings.AppPackageName;
            Application.OpenURL(url);
        }
        public void OnClickMoreApp()
        {
            Application.OpenURL("http://play.google.com/store/apps/developer?id=" + GameManager.Instance.GameSettings.DeveloperId);
        }
        public void OnClickPrivacy()
        {
            Application.OpenURL(GameManager.Instance.GameSettings.PrivacyPolicy);
        }
        public IEnumerator ShareTextInAnroid()
        {
            var shareSubject = "Play Water Sort Game on your phone"; //Subject text
            var shareMessage = "Download Water Sort Game from this link: \n" +//Message text
                               "https://play.google.com/store/apps/details?id=" + GameManager.Instance.GameSettings.AppPackageName; //Your link


            if (!Application.isEditor)
            {
                AndroidJavaClass intentClass =
                    new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject =
                    new AndroidJavaObject("android.content.Intent");
                intentObject.Call<AndroidJavaObject>
                    ("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
                //put text and subject extra
                intentObject.Call<AndroidJavaObject>("setType", "text/plain");

                intentObject.Call<AndroidJavaObject>
                    ("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
                intentObject.Call<AndroidJavaObject>
                    ("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage);

                //call createChooser method of activity class
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

                AndroidJavaObject currentActivity =
                    unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject chooser =
                    intentClass.CallStatic<AndroidJavaObject>
                    ("createChooser", intentObject, "Share Via");
                currentActivity.Call("startActivity", chooser);

            }

            yield return null;
        }
        private void CheckPrivacyStatus()
        {
            if (AdsManager.Instance.PrivacyStatus())
            {
                privacySetting.gameObject.SetActive(true);
            }
            else
            {
                privacySetting.gameObject.SetActive(false);
            }
        }
        public void OnClickPrivacySetting()
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.touch);

            if (!GameManager.IsInternetConnection())
            {
                SuccessErrorPanel gameUI = UIController.GetPage<SuccessErrorPanel>();
                gameUI.SetData(ToasterState.InternetError);

                UIController.ShowPage<SuccessErrorPanel>();
                return;
            }

#if ADMOB
            AdsManager.Instance.ShowPrivacyOptionsForm((string error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    
                    SuccessErrorPanel gameUI = UIController.GetPage<SuccessErrorPanel>();
                    gameUI.SetData(ToasterState.NetworkError);
                    UIController.ShowPage<SuccessErrorPanel>();

                    Debug.LogWarning("Failed to show consent privacy form: " + error);
                }
                else
                {
                    Debug.Log("Privacy form opened successfully.");
                }
            });
#endif
        }
    }
}