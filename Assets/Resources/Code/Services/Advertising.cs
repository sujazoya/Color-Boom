using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class Advertising : MonoBehaviour {

    private int advertInterval = 3;
    private int advertCounter;

    private void Start()
    {
    
    }


    public void cacheAd()
    {
        
    }

    public void showAd()
    {

        advertCounter++;

        if (advertCounter == advertInterval)
        {

            bool doNotShowAd = false;

            if (PlayerPrefs.GetInt("noAds", 0) == 1)
            {
                doNotShowAd = true;
            }

            if (doNotShowAd == false)
            {
                //if (Advertisement.IsReady())
                //{
                //    Advertisement.Show();
                //}
            }
          

            advertCounter = 0;

        }

    }






}
