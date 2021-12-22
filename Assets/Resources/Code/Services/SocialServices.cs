using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SocialPlatforms;

#if (UNITY_ANDROID)
using GooglePlayGames;
#endif

public class SocialServices : MonoBehaviour
{

    // Singleton
    private static SocialServices instance = null;
    public static SocialServices Instance
    {
        get
        {
            return instance;
        }
    }

    private GoogleAnalyticsV4 googleAnalytics;


    [HideInInspector]
    public bool signedOnline;

    void Awake()
    {
        // Singleton
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        instance = this;

        googleAnalytics = transform.GetComponent<GoogleAnalyticsV4>();


        // SETUP ANDROID
        #if (UNITY_ANDROID)
        if (Application.platform == RuntimePlatform.Android)
        {
            PlayGamesPlatform.DebugLogEnabled = false;
            PlayGamesPlatform.Activate();
        }
        #endif

        // SETUP IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {

        }



    }

    void Start()
    {


        Social.Active.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                //Debug.Log("Signed In Successfuly");
                getHighScoreFromSocialNetwork();

                signedOnline = true;
            }
            else
            {
                //Debug.Log("Failed To Sign In");
                signedOnline = false;
            }
        });


    }

    public void showAchievementsUI()
    {
        // ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            #if (UNITY_ANDROID)
            PlayGamesPlatform.Instance.ShowAchievementsUI();
            #endif
        }


        // IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Social.ShowAchievementsUI();
        }


    }

    public void showLeaderBoardUI()
    {
        // ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            #if (UNITY_ANDROID)
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
            #endif
        }

        // IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Social.ShowLeaderboardUI();
        }


    }

    public void postLeaderBoardScore(int score)
    {

        try
        {


            // ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                Social.ReportScore(score, GooglePlayServiceIDS.leaderboard_infiniterun, (bool success) => { });
            }

            // IOS
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Social.ReportScore(score, "grp.InfiniteRun", (bool success) => { });
            }

        }
        catch (Exception)
        {

   
        }
  


    }

    public enum Achievements
    {
        None = 0,
        PassFirstObstacle = 1,
        FirstRecord = 2,
        CollectBomb = 3,
        CollectRocket = 4,
        CollectShield = 5
    }

    public void postAchievement(Achievements achievement)
    {


        if (PlayerPrefs.HasKey(achievement.ToString()) == true)
        {
            return;
        }

        // Saving Achievemnt
        PlayerPrefs.SetInt(achievement.ToString(), 1);

        string achievementID = "";


        // ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            switch (achievement)
            {
                case Achievements.None:
                    break;
                case Achievements.PassFirstObstacle:
                    achievementID = GooglePlayServiceIDS.achievement_pass_first_obstacle;
                    break;
                case Achievements.FirstRecord:
                    achievementID = GooglePlayServiceIDS.achievement_new_record;
                    break;
                case Achievements.CollectBomb:
                    achievementID = GooglePlayServiceIDS.achievement_collect_bomb;
                    break;
                case Achievements.CollectRocket:
                    achievementID = GooglePlayServiceIDS.achievement_collect_rocket;
                    break;
                case Achievements.CollectShield:
                    achievementID = GooglePlayServiceIDS.achievement_collect_shield;
                    break;
                default:
                    break;
            }
        }

        // IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            switch (achievement)
            {
                case Achievements.None:
                    break;
                case Achievements.PassFirstObstacle:
                    achievementID = "grp.passFirstObstacle";
                    break;
                case Achievements.FirstRecord:
                    achievementID = "grp.FirstRecord";
                    break;
                case Achievements.CollectBomb:
                    achievementID = "grp.CollectBomb";
                    break;
                case Achievements.CollectRocket:
                    achievementID = "grp.CollectRocket";
                    break;
                case Achievements.CollectShield:
                    achievementID = "grp.CollectShield";
                    break;
                default:
                    break;
            }

           
        }

        // Posting Achievement
        try
        {
            Social.ReportProgress(achievementID, 100, (bool success) => { });

        }
        catch (Exception)
        {

   
        }

    }

    public void getHighScoreFromSocialNetwork()
    {


        try
        {

            ILeaderboard leaderboard = Social.CreateLeaderboard();
            leaderboard.id = GooglePlayServiceIDS.leaderboard_infiniterun;
            leaderboard.LoadScores(result =>
            {

                if (result == true)
                {
              
                    int score = (int)leaderboard.localUserScore.value;

                    if (score > PlayerPrefs.GetInt("InfiniteScore", 0))
                    {
                        PlayerPrefs.SetInt("InfiniteScore", score);
                    }

                }


            });

            if (PlayerPrefs.GetInt("InfiniteScore", 0) < 10)
            {
                PlayerPrefs.SetInt("InfiniteScore", 10);
            }

        }
        catch (Exception ex)
        {
       
        }


    }

    public void showRateApp()
    {
        Invoke("rateAppWorker", 1.0f);
    }

    private void rateAppWorker()
    {

        if (PlayerPrefs.HasKey("AppRated") == false)
        {

            if (PlayerPrefs.HasKey("AppRateCounter") == false)
            {
                PlayerPrefs.SetInt("AppRateCounter", 0);
            }

            if (PlayerPrefs.GetInt("AppRateCounter", 0) >= 3)
            {

                GuiController.Instance.showDialogPoup("DO YOU LIKE OUR GAME ?", Color.yellow, "WOULD YOU LIKE TO RATE US ON THE MARKET ?", delegate () { PlayerPrefs.SetInt("AppRated", 1); gotoStore(); }, null);
                PlayerPrefs.SetInt("AppRateCounter", 0);
            }


            PlayerPrefs.SetInt("AppRateCounter", PlayerPrefs.GetInt("AppRateCounter", 0) + 1);
        }

    }


    public void logAnalyticScreen(string screenName)
    {
        googleAnalytics.LogScreen(screenName);
    }

    public void logAnalyticEvent(string eventCategory, string eventAction, string eventName, long value)
    {
        googleAnalytics.LogEvent(eventCategory, eventAction, eventName, value);
    }

    public void logAnalyticShopScreen()
    {
        googleAnalytics.LogScreen("Shop");
    }
    public void logAnalyticTutorialScreen()
    {
        googleAnalytics.LogScreen("Tutorial");
    }

    public void logAnalyticModesScreen()
    {
        googleAnalytics.LogScreen("GameModes");
    }

    public void logAnalyticLevelScreen()
    {
        googleAnalytics.LogScreen("Levels");
    }

    public void logAnalyticAchievementsScreen()
    {
        googleAnalytics.LogScreen("Achievements");
    }

    public void logAnalyticLeaderBoardScreen()
    {
        googleAnalytics.LogScreen("LeaderBoards");
    }


    public void logAnalyticTestScreen()
    {
        googleAnalytics.LogScreen("TEST_CALLER");
    }


    public void gotoStore()
    {

        //// ANDROID
        //if (Application.platform == RuntimePlatform.Android)
        //{
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.zoya.ColorBoom");
        //}

        //// IOS
        //if (Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    Application.OpenURL("itms-apps://");
        //}


      

    }



}
