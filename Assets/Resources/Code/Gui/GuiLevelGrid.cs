using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[System.Serializable]
public class ItemsHolderGuiLevelGrid
{
    public Button classicButton;
    public Button timedButton;
    public Button lightOutButton;
    public Button panicButton;
    public Button levelCloseButton;
    public Button nextButton;
}


public class GuiLevelGrid : MonoBehaviour
{
    [SerializeField] ItemsHolderGuiLevelGrid items;



    public GameObject transition;
    // Singleton
    private static GuiLevelGrid instance = null;
    public static GuiLevelGrid Instance
    {
        get
        {
            return instance;
        }
    }

   // public UIScrollView levelScroll;
   // public GameObject levelGridHolder;
    public Transform levelParent;
    public GameObject levelPanel;
    public GameObject levelButton;
    [HideInInspector] public List<GuiLevelItem> levelGridItems;



    void Awake()
    {

        // Singleton
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        instance = this;
    }

    int modeIndex;

    private void OnEnable()
    {
        levelPanel.SetActive(false);
    }
    void Start()
    {

         levelGridItems = new List<GuiLevelItem>();
        //  levelGridItem.gameObject.SetActive(false);
        // grid = levelGridHolder.GetComponent<UIGrid>();
        //  addGameModeScrollOperator();


      items. classicButton.onClick.AddListener(loadClassicLevels);
      items. timedButton.onClick.AddListener(loadTimedLevels); 
      items. lightOutButton.onClick.AddListener(loadLightsOutLevels); 
      items. panicButton.onClick.AddListener(loadPanicLevels); 
      items. levelCloseButton.onClick.AddListener(CloseLevelPanel); 
      items. nextButton.onClick.AddListener(ActiveNext); 

    }
    public void CloseLevelPanel()
    {
        levelPanel.SetActive(false);
    }
    void ActiveNext()
    {
        StartCoroutine(DoTrans());
        StartCoroutine(Next());
    }
    IEnumerator Next()
    {
        modeIndex++;
        if (modeIndex > 3)
        {
            modeIndex = 0;
        }
        yield return new WaitForSeconds(1f);
        items. classicButton.gameObject.SetActive(false);
        items. timedButton.gameObject.SetActive(false);
        items. lightOutButton.gameObject.SetActive(false);
        items.panicButton.gameObject.SetActive(false);
        if (modeIndex == 0)
        {
            items.classicButton.gameObject.SetActive(true);
        }else
             if (modeIndex == 1)
        {
            items.timedButton.gameObject.SetActive(true);
        }
        else
             if (modeIndex == 2)
        {
            items.lightOutButton.gameObject.SetActive(true);
        }
        else
             if (modeIndex == 3)
        {
            items.panicButton.gameObject.SetActive(true);
        }
        
    }
    IEnumerator DoTrans()
    {
        transition.SetActive(false);
        yield return new WaitForSeconds(.1f);
        transition.SetActive(true);
        yield return new WaitForSeconds(1f);




        yield return new WaitForSeconds(1f);
        transition.SetActive(false);
    }

    public void loadClassicLevels()
    {
        StartCoroutine(loadLevelGrid(LevelIdentifier.GameMode.Classic));
    }


    public void loadTimedLevels()
    {
        StartCoroutine(loadLevelGrid(LevelIdentifier.GameMode.Timed));
    }

    public void loadLightsOutLevels()
    {
        StartCoroutine(loadLevelGrid(LevelIdentifier.GameMode.LightsOut));
    }

    public void loadPanicLevels()
    {
        StartCoroutine(loadLevelGrid(LevelIdentifier.GameMode.Panic));
    }


    private bool gameModeSelected;
    //private void addGameModeScrollOperator()
    //{

    //    // Arrow Play When Game Mode Scroll Change
    //    GuiController.Instance.gameModeCenterController.onCenter = new UICenterOnChild.OnCenterCallback(delegate (GameObject centeredObject)
    //    {
    //        if (gameModeSelected == false)
    //        {
    //            gameModeSelected = true;

    //            GameModeScrollItem.disableAllFx();

    //            GameModeScrollItem item = centeredObject.GetComponent<GameModeScrollItem>();
    //            if (item != null)
    //            {
    //                item.fxHolder.gameObject.SetActive(true);
    //            }

    //           // GuiController.Instance.gameModesArrow.HardResetToBeginning();
    //            //GuiController.Instance.gameModesArrow.PlayForward();
    //        }


    //    });

    //  //  GuiController.Instance.gameModesScroll.onDragStarted = new UIScrollView.OnDragNotification(delegate ()
    //    {
    //        gameModeSelected = false;
    //    });


    //}

    private IEnumerator loadLevelGrid(LevelIdentifier.GameMode mode)
    {
        levelPanel.SetActive(true);
        // Destroying
        if (levelGridItems.Count > 0)
        {
            foreach (GuiLevelItem item in levelGridItems)
            {
                Destroy(item.gameObject);
            }
            levelGridItems.Clear();
        }      

      //  GuiController.Instance.lblLevelsLoading.gameObject.SetActive(true);
        // Unlocking First Level
        string firstLevelKey = mode.ToString() + "_Level1";
        if (PlayerPrefs.HasKey(firstLevelKey) == false)
        {
            LevelLibrary.Instance.unlockGameModeLevel(mode, firstLevelKey);
            if (LevelLibrary.Instance.getGameModeLastLevel(mode) == "")
            {
                LevelLibrary.Instance.setGameModeLastLevel(mode, firstLevelKey);
            }
        }


        yield return new WaitForSeconds(0.4f);

    

        GameObject[] currentLevels = null;

        switch (mode)
        {
            case LevelIdentifier.GameMode.None:
                break;
            case LevelIdentifier.GameMode.Infinite:
                // No Levels
                break;
            case LevelIdentifier.GameMode.Classic:
                //GuiController.Instance.lblLevelsTitle.text = "CLASSIC";
                currentLevels = LevelLibrary.Instance.classicLevels;
                break;
            case LevelIdentifier.GameMode.Timed:
               // GuiController.Instance.lblLevelsTitle.text = "TIMED";
                currentLevels = LevelLibrary.Instance.timedLevels;
                break;
            case LevelIdentifier.GameMode.LightsOut:
               // GuiController.Instance.lblLevelsTitle.text = "LIGHTS OUT";
                currentLevels = LevelLibrary.Instance.lightsOutLevels;
                break;
            case LevelIdentifier.GameMode.Panic:
               // GuiController.Instance.lblLevelsTitle.text = "PANIC";
                currentLevels = LevelLibrary.Instance.panicLevels;
                break;
        }



        //grid.enabled = false;

        if (currentLevels != null)
        {

            // Creating Level Array And Sort
            List<LevelIdentifier> levels = new List<LevelIdentifier>();

            currentLevels = currentLevels.OrderBy(go => go.name).ToArray();
            for (int i = 0; i < currentLevels.Length; i++)
            {
                LevelIdentifier level = currentLevels[i].gameObject.GetComponent<LevelIdentifier>();
                if (level != null)
                {
                  
                    levels.Add(level);
                }
            }

            // Sort
            levels = levels.OrderBy(lvl => lvl.levelIndex).ToList();


            // Creating Level Items
            for (int i = 0; i < levels.Count; i++)
            {

                LevelIdentifier level = levels[i];

                GameObject newLevel = (GameObject)Instantiate(levelButton);
                newLevel.transform.parent = levelParent;
                newLevel.name = level.levelKEY;
                //newLevel.transform.position = levelGridHolder.transform.position;
                newLevel.transform.localScale = new Vector3(1, 1, 1);
                //newLevel.gameObject.SetActive(true);

                GuiLevelItem guiLevelItem = newLevel.GetComponent<GuiLevelItem>();
                if (guiLevelItem != null)
                {
                
                    guiLevelItem.level = LevelLibrary.Instance.getLevel(level.levelKEY);
                    levelGridItems.Add(guiLevelItem);
                }
               
            }


            // Updating Level Grid
            for (int i = 0; i < levelGridItems.Count; i++)
            {

                GuiLevelItem currentItem = levelGridItems[i];
                Button currentTtemButton = currentItem.GetComponent<Button>();
                //currentLevelItem = currentItem;
                //currentTtemButton.onClick.AddListener(guiLevelItemOnClick);
                currentItem.Label.text = currentItem.level.levelIndex.ToString();

                string levelKey = currentItem.level.levelKEY;
       

                if (PlayerPrefs.HasKey(levelKey) == true)
                {
                    currentTtemButton.enabled = true;
                    currentItem.Label.enabled = true;
                    currentItem.OkIcon.SetActive(false);
                    currentItem.LockIcon.SetActive(false);
                    currentItem.Selected.SetActive(true);
                    //currentItem.Selected.GetComponent<ColorChanger>().enabled = false;
                    //currentItem.Selected.GetComponent<TweenScale>().enabled = false;

                    int levelState = PlayerPrefs.GetInt(levelKey);

                    switch (levelState)
                    {
                        case 0: // Not Completed

                            currentItem.Background.color = currentItem.unlockColor;
                            currentTtemButton.transform.GetComponent<Image>().color = currentItem.unlockColor;

                            currentTtemButton.transform.GetComponent<Image>().color = currentItem.unlockColor;


                            break;
                        case 1:  // Completed

                            currentItem.Background.color = currentItem.completedColor;
                            currentTtemButton.transform.GetComponent<Image>().color = currentItem.completedColor;

                            currentTtemButton.transform.GetComponent<Image>().color = currentItem.completedColor;


                            currentItem.OkIcon.SetActive(true);
                            break;
                    }

                    if (LevelLibrary.Instance.getGameModeLastLevel(currentItem.level.mode) == currentItem.level.levelKEY)
                    {
                        currentItem.Selected.SetActive(true);
                        //currentItem.Selected.GetComponent<ColorChanger>().enabled = true;
                        //currentItem.Selected.GetComponent<TweenScale>().enabled = true;
                    }

                }
                else
                {
                    currentTtemButton.enabled = false;

                    currentItem.Background.color = currentItem.lockedColor;
                    currentTtemButton.transform.GetComponent<Image>().color = currentItem.lockedColor;
                    currentItem.LockIcon.SetActive(true);
                    currentItem.Label.enabled = false;
                    currentItem.OkIcon.SetActive(false);                   
                    //currentItem.Selected.GetComponent<ColorChanger>().enabled = false;
                    currentItem.Selected.SetActive(false);
                }


            }

        }

        //grid.enabled = true;
        //GuiController.Instance.lblLevelsLoading.gameObject.SetActive(false);

        //levelScroll.InvalidateBounds();

        yield break;

    }


    [HideInInspector]public GuiLevelItem currentLevelItem;
    public void guiLevelItemOnClick()
    {


        //GuiLevelItem item
        if (currentLevelItem != null)
        {

            if (currentLevelItem.level == null)
            {
                //GuiController.Instance.showMessagePoup("Level" + item.level.levelIndex.ToString(), Color.yellow, "Coming Soon...");
                return;
            }
            else
            {
                LevelLibrary.Instance.setGameModeLastLevel(currentLevelItem.level.mode, currentLevelItem.level.levelKEY);
                GameController.Instance.level = currentLevelItem.level;
                GameController.Instance.setOnEnter();
            }


        }


    }

}

