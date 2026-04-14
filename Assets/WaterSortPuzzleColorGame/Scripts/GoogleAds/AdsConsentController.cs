#undef ADMOB
#undef USE_UNITYADS
#undef USE_IRONSOURCE
#if USE_IRONSOURCE
using GoogleMobileAds.Mediation.IronSource.Api;
#endif
#if USE_UNITYADS
using GoogleMobileAds.Mediation.UnityAds.Api;
#endif
#if ADMOB
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;
#endif
using System;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    public static class AdsConsentController
    {
#if ADMOB
        public static bool CanRequestAds => ConsentInformation.CanRequestAds();

        
        public static void GatherConsent(Action<string> onComplete)
        {
            var requestParameters = new ConsentRequestParameters
            {
                // False means users are not under age.
                TagForUnderAgeOfConsent = false,

                // Keep only if you are testing consent form
                //ConsentDebugSettings = new ConsentDebugSettings
                //{
                //    // For debugging consent settings by geography.
                //    DebugGeography = DebugGeography.EEA,//DebugGeography.Disabled,
                //    TestDeviceHashedIds = AdsManager.TestDeviceIds,
                //}
            };
            ConsentInformation.Update(requestParameters, (FormError updateError) =>
            {

                if (updateError != null)
                {
                    onComplete(updateError.Message);
                    return;
                }


                // Determine the consent-related action to take based on the ConsentStatus.
                if (CanRequestAds)
                {
                   
                    onComplete(null);
                    return;
                }

                ConsentForm.LoadAndShowConsentFormIfRequired((FormError showError) =>
                {
                   // UpdatePrivacyButton();
                    if (showError != null)
                    {
                        // Form showing failed.
                        if (onComplete != null)
                        {
                            onComplete(showError.Message);
                        }
                    }
                    // Form showing succeeded.
                    else if (onComplete != null)
                    {
                        onComplete(null);
                    }
                });
            });
        }
        public static void ApplyConsentToAdNetworks()
        {
            bool hasConsent = CanRequestAds;
#if USE_IRONSOURCE
            // For U.S. state privacy laws (like CCPA / "Do Not Sell")
            IronSource.SetMetaData("do_not_sell", hasConsent ? "false" : "true"); // or "false" based on user choice

            // For GDPR (Europe)
            IronSource.SetConsent(hasConsent); // or false
#endif
#if USE_UNITYADS
            UnityAds.SetConsentMetaData("gdpr.consent", hasConsent);
            UnityAds.SetConsentMetaData("privacy.consent", hasConsent);
#endif
        }
        public static bool PrivacyStatus()
        {
            return ConsentInformation.PrivacyOptionsRequirementStatus ==
                     PrivacyOptionsRequirementStatus.Required;
        }
        public static void ShowPrivacyOptionsForm(Action<string> onComplete)
        {
            Debug.Log("Showing privacy options form.");

            if (ConsentInformation.PrivacyOptionsRequirementStatus
                != PrivacyOptionsRequirementStatus.Required)
            {
                Debug.LogWarning("Privacy options not required in this region.");
                onComplete?.Invoke("Not required");
                return;
            }

            ConsentForm.ShowPrivacyOptionsForm((FormError showError) =>
            {
                UpdatePrivacyButton();

                if (showError != null)
                {
                    onComplete?.Invoke(showError.Message);
                }
                else
                {
                    onComplete?.Invoke(null);
                }
            });
        }
        private static void UpdatePrivacyButton()
        {

            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                bool privacyStatus = ConsentInformation.PrivacyOptionsRequirementStatus ==
                            PrivacyOptionsRequirementStatus.Required;

                Debug.Log("Consent Privacy Status:" + privacyStatus);
            });
        }
        public static void ResetConsent()
        {
            ConsentInformation.Reset();
        }
#endif
    }
}