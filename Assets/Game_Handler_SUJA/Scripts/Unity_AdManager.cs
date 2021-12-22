using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;


public class Unity_AdManager : MonoBehaviour
{
   // private string gameID = "4097683";

    public bool testMode = false;
    private string bannerPlacementId = "banner";
    private string myrewardedPlacementId = "rewardedVideo";
    private string interstitialPlacementId = "video";  

  
    private float adInterval;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        string gameID = "4097683";
#elif UNITY_IPHONE
        string gameID = "4097682";
#else
        string appKey = "unexpected_platform";
#endif
        // Advertisement.AddListener(this);
        Advertisement.Initialize(gameID, testMode);     
    }  
   
    //IEnumerator CheckForAd()
    //{
    //    ShowAd();
    //    adInterval = Random.Range(200, 350);
    //    yield return new WaitForSeconds(adInterval);           
    //    StartCoroutine(CheckForAd());
        
    //}

    //int adIndex = 0;
    //public void ShowAd()
    //{
    //    adIndex++;
    //    if (adIndex >= 8)
    //    {
    //        adIndex = 0;
    //    }
    //    switch (adIndex)
    //    {
    //        case 0:
    //            ShowInterstitialVideo();
    //            break;
    //        case 1:
    //            ShowRewardedVideo();
    //            break;
    //        case 2:
    //            ShowOnScreenVideo();
    //            break;
    //        case 3:
    //            ShowInterstitialVideo1();
    //            break;
    //        case 4:
    //            ShowOnTheGoVideo();
    //            break;
    //        case 5:
    //            ShowPlayTimeVideo();
    //            break;
    //        case 6:
    //           // admanager.ShowFullScreenAd();
    //            break;
    //        case 7:
    //           // admanager.ShowRewardedAd();
    //            break;
    //    }
       
    //}
    public IEnumerator ShowBannerWhenReady(BannerPosition bannerPosition)
    {
        while (!Advertisement.IsReady(bannerPlacementId))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.Show(bannerPlacementId);
        Advertisement.Banner.SetPosition(bannerPosition);
    }
    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }
    public void ShowInterstitialVideo()
    {
        Advertisement.Show(interstitialPlacementId);
    }   
   
    public void ShowRewardedVideo()
    {
        Advertisement.Show(myrewardedPlacementId);
    }
    
    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, activate the button: 
        if (placementId == myrewardedPlacementId)
        {
            // myButton.interactable = true;
        }
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
        }
        else if (showResult == ShowResult.Skipped)
        {               
               
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }
}
