using UnityEngine;
using System.Collections;

public class Achievements : MonoBehaviour {


    // Singleton
    private static Achievements instance = null;
    public static Achievements Instance
    {
        get
        {
            return instance;
        }
    }



    private void Awake()
    {

        //PlayerPrefs.DeleteAll();
        //setInfiniteBestScore(10);

        // Unlock All Levels
        //for (int i = 1; i < 50; i++)
        //{
        //    unlockLevel(GameController.GameMode.Classic, i);
        //}


        // Singleton
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        instance = this;


        if (PlayerPrefs.HasKey("InfiniteScore") == false)
        {
            PlayerPrefs.SetInt("InfiniteScore", 10);
        }

    }



    public int getInfiniteBestScore()
    {
        return PlayerPrefs.GetInt("InfiniteScore", 0);
    }

    public void setInfiniteBestScore(int score)
    {
        PlayerPrefs.SetInt("InfiniteScore", score);
    }



    public void addStartPoints(int startsToAdd)
    {
        int currentStartCount = PlayerPrefs.GetInt("TotalStars", 0);
        PlayerPrefs.SetInt("TotalStars", currentStartCount + 1);
    }

    public int getTotalStarts()
    {
        return PlayerPrefs.GetInt("TotalStars");
    }


    public void updateGuiLabels()
    {

        GuiController.Instance.lblHudStarTotal.text = GameController.Instance.collectedStarCount.ToString();
        GuiController.Instance.lblPauseStartTotal.text = GameController.Instance.collectedStarCount.ToString();

        GuiController.Instance.lblFailCollectedStar.text = GameController.Instance.collectedStarCount.ToString();
        GuiController.Instance.lblFailTotalStar.text = (Achievements.Instance.getTotalStarts() - GameController.Instance.collectedStarCount).ToString();


        GuiController.Instance.lblSuccessCollectedStar.text = GameController.Instance.collectedStarCount.ToString();
        GuiController.Instance.lblSuccessTotalStar.text = (Achievements.Instance.getTotalStarts() - GameController.Instance.collectedStarCount).ToString();

        GuiController.Instance.lblShopStarTotal.text = Achievements.Instance.getTotalStarts().ToString();
        GuiController.Instance.lblMainMenuHighScore.text = Achievements.Instance.getInfiniteBestScore().ToString();

    }


}
