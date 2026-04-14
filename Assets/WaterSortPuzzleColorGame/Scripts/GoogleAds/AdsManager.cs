#undef ADMOB
#if ADMOB
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
#endif
using System;
using System.Collections.Generic;
using WaterSortPuzzleGame.DataClass;
using UnityEngine;
using WaterSortPuzzleGame.Enum;
using System.Collections;

namespace WaterSortPuzzleGame
{
    public class AdsManager : MonoBehaviour
    {
        [SerializeField] AdsSettings settings;
        public static AdsManager Instance { get; private set; }


#if ADMOB
        private InterstitialAd _interstitialAd;
        private RewardedAd _rewardedAd;
        private BannerView _bannerView;
        
#endif

        private static bool? _isInitialized;

        //Keep only if you are testing consent form
//#if ADMOB
//        internal static List<string> TestDeviceIds = new List<string>()
//                {
//                    AdRequest.TestDeviceSimulator,
//#if UNITY_IPHONE
//                    "96e23e80653bb28980d3f40beb58915c",
//#elif UNITY_ANDROID
//                    "8EA6DD8F1F71A59BECF411EDC8975B9F"
//#endif
//                };
//#endif

        // Start is called before the first frame update
        public void Init()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

#if ADMOB

            //Keep only if you are testing consent form

            //AdsConsentController.ResetConsent();
            //MobileAds.SetRequestConfiguration(new RequestConfiguration
            //{
            //    TestDeviceIds = TestDeviceIds
            //});

            if (AdsConsentController.CanRequestAds)
            {
                InitializeGoogleMobileAds();
            }
            InitializeGoogleMobileAdsConsent();
#endif

        }
        public void Start()
        {
            ShowBannerAd();
        }
        private void OnEnable()
        {
            ScreenResizeManager.OnScreenSizeChanged += OnScreenSizeChanged;
        }

        private void OnDisable()
        {
            ScreenResizeManager.OnScreenSizeChanged -= OnScreenSizeChanged;
        }
        private void OnScreenSizeChanged()
        {
            DisableForcedAd();

            if (UIController.IsPageDisplayed<ShopPanel>() ||
                UIController.IsPageDisplayed<CollectionPanel>() ||
                UIController.IsPageDisplayed<HomePanel>())
                HideBannerAd();
            else
                ShowBannerAd();
        }
        public bool PrivacyStatus()
        {
#if ADMOB
            return AdsConsentController.PrivacyStatus();
#else
            return false;
#endif
        }
        public bool IsRewardedAdReady()
        {
#if ADMOB
            return _rewardedAd != null && _rewardedAd.CanShowAd();
#else
            Debug.Log("ADMOB SDK not found. Dummy RewardedAd is ready");
            return true;
#endif
        }
        public void ShowInterstitialAd()
        {
            if (IAPManager.IsNoAdsPurchased) return;

#if ADMOB

            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {

                _interstitialAd.Show();
            }
            else
            {
                LoadInterstitialAd();

            }
#else
            Debug.Log("ADMOB SDK not found. Ads skipped.");
#endif

        }

        public void ShowRewardedAd(Action onComplete)
        {
#if ADMOB
           if (!GameManager.IsInternetConnection())
            {
                SuccessErrorPanel gameUI = UIController.GetPage<SuccessErrorPanel>();
                gameUI.SetData(ToasterState.InternetError);

                UIController.ShowPage<SuccessErrorPanel>();

                return;
            }


            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                _rewardedAd.Show((reward) =>
                {
                    StartCoroutine(InvokeOnMainThread(onComplete)); //onComplete?.Invoke();
                });
            }
            else
            {
                if (_rewardedAd == null)
                {
                    Debug.Log("null reward ads");
                }

                LoadRewardedAd();
            }
#else
            Debug.Log("ADMOB SDK not found. Dummy Reward.");
            onComplete?.Invoke(); // fallback (optional)
#endif
        }
        private IEnumerator InvokeOnMainThread(Action onComplete)
        {
            yield return null;
            onComplete?.Invoke();
        }
        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowBannerAd()
        {
            if (IAPManager.IsNoAdsPurchased || GameManager.Instance.IsWideScreen()) return;
#if ADMOB
            if (_bannerView != null)
            {
                Debug.Log("Showing banner view.");
                _bannerView.Show();
            }
            else
            {
                if (_bannerView == null)
                {
                    Debug.Log("null banner ads");
                }

                LoadBannerAd();
            }
#else
            Debug.Log("ADMOB SDK not found. Dummy Banner Show.");
#endif
        }
        public void DisableForcedAd()
        {
#if ADMOB
            DestroyBannerAd();
#else
            Debug.Log("ADMOB SDK not found. Disable Banner Ads.");
#endif
        }

        /// <summary>
        /// Hides the ad.
        /// </summary>
        public void HideBannerAd()
        {
#if ADMOB
            if (_bannerView != null)
            {
                Debug.Log("Hiding banner view.");
                _bannerView.Hide();
            }
#else
            Debug.Log("ADMOB SDK not found. Dummy Banner Hide.");
#endif
        }
#if ADMOB

        /// <summary>
        /// Creates a 320x50 banner at bottom of the screen.
        /// </summary>
        public void CreateBannerView()
        {
            Debug.Log("Creating banner view.");

            // If we already have a banner, destroy the old one.
            if (_bannerView != null)
            {
                DestroyBannerAd();
            }
            AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            // Create a 320x50 banner at top of the screen.
            _bannerView = new BannerView(settings.BannerAdUnitId, adaptiveSize, AdPosition.Bottom);

            // Listen to events the banner may raise.
            ListenToBannerAdEvents();
        }

        /// <summary>
        /// Creates the banner view and loads a banner ad.
        /// </summary>
        public void LoadBannerAd()
        {
            // Create an instance of a banner view first.
            if (_bannerView == null)
            {
                CreateBannerView();
            }

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            Debug.Log("Loading banner ad.");
            _bannerView.LoadAd(adRequest);
        }

       

        /// <summary>
        /// Destroys the ad.
        /// When you are finished with a BannerView, make sure to call
        /// the Destroy() method before dropping your reference to it.
        /// </summary>
        public void DestroyBannerAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("Destroying banner view.");
                _bannerView.Destroy();
                _bannerView = null;
            }

            // Inform the UI that the ad is not ready.
            //AdLoadedStatus?.SetActive(false);
        }

        

        /// <summary>
        /// Listen to events the banner may raise.
        /// </summary>
        private void ListenToBannerAdEvents()
        {
            // Raised when an ad is loaded into the banner view.
            _bannerView.OnBannerAdLoaded += () =>
            {
                Debug.Log("Banner view loaded an ad with response : "
                    + _bannerView.GetResponseInfo());

                // Inform the UI that the ad is ready.
                //AdLoadedStatus?.SetActive(true);
            };
            // Raised when an ad fails to load into the banner view.
            _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                Debug.LogError("Banner view failed to load an ad with error : " + error);
            };
            // Raised when the ad is estimated to have earned money.
            _bannerView.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Banner view paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            _bannerView.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Banner view recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            _bannerView.OnAdClicked += () =>
            {
                Debug.Log("Banner view was clicked.");
            };
            // Raised when an ad opened full screen content.
            _bannerView.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Banner view full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            _bannerView.OnAdFullScreenContentClosed += () =>
            {
                LoadBannerAd();
                Debug.Log("Banner view full screen content closed.");
            };
        }


        private void InitializeGoogleMobileAdsConsent()
        {
            AdsConsentController.GatherConsent((string error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Failed to gather consent with error: " +
                        error);
                }
                else
                {
                    Debug.Log("Google Mobile Ads consent updated: "
                        + ConsentInformation.ConsentStatus);
                }

                if (AdsConsentController.CanRequestAds)
                {
                    InitializeGoogleMobileAds();
                }
            });
        }
        
        private void InitializeGoogleMobileAds()
        {

            if (_isInitialized.HasValue)
            {
                return;
            }

            _isInitialized = false;

            // Apply consent only if we're about to initialize
            AdsConsentController.ApplyConsentToAdNetworks();

            MobileAds.Initialize((InitializationStatus initstatus) =>
            {
                if (initstatus == null)
                {
                    _isInitialized = null;
                    return;
                }

                _isInitialized = true;

                LoadInterstitialAd();
                LoadRewardedAd();
            });
        }


        public void ShowPrivacyOptionsForm(Action<string> onComplete = null)
        {
            AdsConsentController.ShowPrivacyOptionsForm((string error) =>
            {
                if (error != null)
                {
                    onComplete?.Invoke(error);
                }
                else
                {
                    onComplete?.Invoke(null);
                }
            });
        }
        /// <summary>
        /// Interstitial ad Start.
        /// </summary>
        public void LoadInterstitialAd()
        {

            DestroyInterstitialAd();

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            InterstitialAd.Load(settings.InterstitialAdUnitId, adRequest,
                (InterstitialAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("interstitial ad failed to load an ad " +
                                        "with error : " + error);
                        return;
                    }
                    _interstitialAd = ad;
                    RegisterEventHandlers(ad);
                });
        }

        
        public void DestroyInterstitialAd()
        {
            if (_interstitialAd != null)
            {
                //Debug.Log("Destroying interstitial ad.");
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

        }
        private void RegisterEventHandlers(InterstitialAd interstitialAd)
        {
            // Raised when the ad is estimated to have earned money.
            interstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            interstitialAd.OnAdImpressionRecorded += () =>
            {
                //Debug.Log("Interstitial ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            interstitialAd.OnAdClicked += () =>
            {
                // Debug.Log("Interstitial ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            interstitialAd.OnAdFullScreenContentOpened += () =>
            {
                //Debug.Log("Interstitial ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                LoadInterstitialAd();
                //Debug.Log("Interstitial ad full screen content closed.");
            };
            // Raised when the ad failed to open full screen content.
            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                LoadInterstitialAd();
                Debug.LogError("Interstitial ad failed to open full screen content " +
                               "with error : " + error);
            };
        }
        /// <summary>
        /// Interstitial ad End.
        /// </summary>
        /// 
        /// <summary>
        /// Rewarded ad Start.
        /// </summary>



        /// <summary>
        /// Loads the rewarded ad.
        /// </summary>
        public void LoadRewardedAd()
        {
            // Clean up the old ad before loading a new one.
            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            //Debug.Log("Loading the rewarded ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            RewardedAd.Load(settings.RewardedAdUnitId, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("Rewarded ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    //Debug.Log("Rewarded ad loaded with response : "
                    //          + ad.GetResponseInfo());

                    _rewardedAd = ad;
                    // Register to ad events to extend functionality.
                    RewardedRegisterEventHandlers(ad);
                });
        }

        private void RewardedRegisterEventHandlers(RewardedAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Rewarded ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Rewarded ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                //Debug.Log("Rewarded ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                LoadRewardedAd();
                //Debug.Log("Rewarded ad full screen content closed.");
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                LoadRewardedAd();
                Debug.LogError("Rewarded ad failed to open full screen content " +
                               "with error : " + error);
            };
        }
        /// <summary>
        /// Rewarded ad End.
        /// </summary>
#endif
    }
}

