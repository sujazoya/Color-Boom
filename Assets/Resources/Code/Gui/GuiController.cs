using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CinemaDirector;
using System;

public class GuiController : MonoBehaviour
{
    // Singleton
    private static GuiController instance = null;
    public static GuiController Instance
    {
        get
        {
            return instance;
        }
    }
    
    public Cutscene howToCutScene;

    public GameObject btnToHome;
    public GameObject btnToGame;
    public GameObject btnToFail;

    public TweenPosition clockTimerHolder;
    public Text clockCountDownLabel;
    public TweenScale clockAnimation;

    public Text lblTime;
    public TweenScale lblTimeAnim;
    public UISprite sprNoTime;

    public GameObject xpHolder;
    public TweenScale xpAnim;
    public UILabel lblXp;
    public UILabel lblXpDesc;

    public Text lblHudStarTotal;
    public Text lblShopStarTotal;

    public Text lblPauseStartTotal;

    public Text lblFailLevelName;
    public Text lblFailCollectedStar;
    public Text lblFailTotalStar;
    public Text lblFailBestScore;
    public Text lblSuccessLevelName;
    public Text lblSuccessCollectedStar;
    public Text lblSuccessTotalStar;
    public Text lblMainMenuHighScore;

    [HideInInspector]
    public bool popupDialogOpen;

    public TweenScale MessagePopupHolder;
    public UILabel lblPopupMessageTitle;
    public UILabel lblPopupMessageText;

    [HideInInspector]
    public EventDelegate cancelPopupDelegate;

    public UIButton popupOkButton;
    public UIButton popupYesButton;
    public UIButton popupNoButton;

    // GAME MODES
    public UIScrollView gameModesScroll;
    public TweenPosition gameModesArrow;
    public UICenterOnChild gameModeCenterController;
    public Text lblLevelsTitle;
    public Text lblLevelsLoading;

    // INAPP BUTTONS
    public UIButton btnNoAds;
    public UIButton btnDoubleStars;

    public GameObject btnFastReload;

    // COLOR HIGHLIGHTS
    public UISprite highLightLeft;
    public UISprite highLightRight;


    public GameObject helpMenu;

    private void Awake()
    {
        // Singleton
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        instance = this;

    }


    // Use this for initialization
    void Start()
    {
        if (helpMenu) { helpMenu.SetActive(false); }
    }

    public void OffHelpMenu()
    {
        StartCoroutine(OffHelp());
    }
    IEnumerator OffHelp()
    {
        yield return new WaitForSeconds(1.5f);
        if (helpMenu) { helpMenu.SetActive(false); }
    }
    public void playHowToCutscene()
    {
        if (howToCutScene != null)
        {
            howToCutScene.RunningTime = 0;
        }

        Invoke("cutsceneStarter", 0.5f);
    }

    private void cutsceneStarter()
    {
        if (howToCutScene != null)
        {
            howToCutScene.IsLooping = true;
            howToCutScene.Play();
        }
    }

    public void resetHowToCutscene()
    {
        Invoke("cutsceneResetter", 0.5f);
    }

    private void cutsceneResetter()
    {
        if (howToCutScene != null)
        {
            howToCutScene.IsLooping = false;
            howToCutScene.Stop();
            howToCutScene.RunningTime = 0;
        }
    }

    public void setHelpMenuBackToMainMenu()
    {
        btnToHome.gameObject.SetActive(true);
        btnToGame.gameObject.SetActive(false);
        btnToFail.gameObject.SetActive(false);
    }

    public void setHelpMenuBackToGamne()
    {
        btnToHome.gameObject.SetActive(false);
        btnToGame.gameObject.SetActive(true);
        btnToFail.gameObject.SetActive(false);
    }
    public void setHelpMenuBackToFail()
    {
        btnToHome.gameObject.SetActive(false);
        btnToGame.gameObject.SetActive(false);
        btnToFail.gameObject.SetActive(true);
    }

    public void showMessagePoup(string title,Color32 titleColor, string message)
    {

        popupDialogOpen = true;

        MessagePopupHolder.gameObject.transform.localPosition = new Vector3(0, 0, -100);

        popupOkButton.gameObject.SetActive(true);
        popupYesButton.gameObject.SetActive(false);
        popupNoButton.gameObject.SetActive(false);

        MessagePopupHolder.gameObject.SetActive(true);

        lblPopupMessageTitle.text = title;
        lblPopupMessageTitle.color = titleColor;

        lblPopupMessageText.text = message;

        MessagePopupHolder.HardResetToEnd();
        MessagePopupHolder.PlayReverse();

        EventDelegate onOkClickDelegate = null;
        onOkClickDelegate = new EventDelegate(delegate ()
        {
       

            popupDialogOpen = false;
            MessagePopupHolder.PlayForward();
            popupOkButton.onClick.Remove(onOkClickDelegate);

        });
        EventDelegate.Add(popupOkButton.onClick, onOkClickDelegate);
        cancelPopupDelegate = onOkClickDelegate;

    }


    public void showDialogPoup(string title, Color32 titleColor, string message, Action yesAction, Action noAction)
    {

        popupDialogOpen = true;

        MessagePopupHolder.gameObject.transform.localPosition = new Vector3(0, 0, -100);

        popupOkButton.gameObject.SetActive(false);
        popupYesButton.gameObject.SetActive(true);
        popupNoButton.gameObject.SetActive(true);

        MessagePopupHolder.gameObject.SetActive(true);

        lblPopupMessageTitle.text = title;
        lblPopupMessageTitle.color = titleColor;

        lblPopupMessageText.text = message;

        EventDelegate pupupOnYesClickDelegate = null;
        EventDelegate pupupOnNoClickDelegate = null;

        pupupOnYesClickDelegate = new EventDelegate(delegate ()
        {
            if (yesAction != null)
            {
                yesAction.Invoke();
            }

            popupDialogOpen = false;
            MessagePopupHolder.PlayForward();

            popupYesButton.onClick.Remove(pupupOnYesClickDelegate);
            popupNoButton.onClick.Remove(pupupOnNoClickDelegate);

        });

        EventDelegate.Add(popupYesButton.onClick, pupupOnYesClickDelegate);
    
        pupupOnNoClickDelegate = new EventDelegate(delegate ()
        {

            if (noAction != null)
            {
                noAction.Invoke();
            }

            popupDialogOpen = false;
            MessagePopupHolder.PlayForward();

            popupYesButton.onClick.Remove(pupupOnYesClickDelegate);
            popupNoButton.onClick.Remove(pupupOnNoClickDelegate);

        });

        EventDelegate.Add(popupNoButton.onClick, pupupOnNoClickDelegate);
        cancelPopupDelegate = pupupOnNoClickDelegate;

        MessagePopupHolder.HardResetToEnd();
        MessagePopupHolder.PlayReverse();

    }


    private void Update()
    {

        if (GameController.Instance == null) return;
        if (GameController.Instance.currentGameState != GameController.GameState.OnMainMenu) return;


        // Toggle Check
        if (Input.GetMouseButtonDown(0))
        {

           // closeAllToggles();

        }

    }

    public void closeAllToggles()
    {
        if (UIToggle.list != null)
        {
            foreach (UIToggle toggle in UIToggle.list)
            {
                toggle.Set(false);
            }
        }
    }

    public void onToggleValueChange(bool val,GameObject posTweenHolder)
    {
        TweenPosition posTween = posTweenHolder.GetComponent<TweenPosition>();

        switch (val)
        {
            case false:

                posTween.PlayForward();

                break;
            case true:

                posTween.gameObject.SetActive(true);
                posTween.PlayReverse();

                break;
        }

      
      
    }

    public void watchToggle(TweenPosition posTween)
    {
        if (posTween.direction == AnimationOrTween.Direction.Forward)
        {
            posTween.gameObject.SetActive(false);
        }
    }


}
