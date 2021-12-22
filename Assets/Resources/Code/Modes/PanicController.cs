using UnityEngine;
using System.Collections;

public class PanicController : MonoBehaviour {

    // Singleton
    private static PanicController instance = null;
    public static PanicController Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // Singleton
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        instance = this;



    }

    public AudioSource audioTick;

    private void Start()
    {

        leftAlpha = GuiController.Instance.highLightLeft.gameObject.GetComponent<TweenAlpha>();
        rightAlpha = GuiController.Instance.highLightRight.gameObject.GetComponent<TweenAlpha>();

    }

    private void OnDisable()
    { 
        if (audioTick != null)
        {
           audioTick.Stop();
        }
    }

    public void enablePanic(float delay)
    {
        Invoke("doEnablePanic", delay);
    }

    private void doEnablePanic()
    {
        this.enabled = true;
    }

    private TweenAlpha leftAlpha;
    private TweenAlpha rightAlpha;


    public void panicLevel()
    {

        if (GameController.Instance == null) return;
        if (GameController.Instance.level == null) return;
        if (GameController.Instance.level.levelData == null) return;


        foreach (GameObject item in GameController.Instance.level.levelData.levelElements)
        {

            ElementIdentifier element = item.GetComponent<ElementIdentifier>();
            if (element != null)
            {
                if (element.elementType == ElementIdentifier.ElementType.ColorChanger)
                {
                    element.gameObject.SetActive(false);
                }
            }


            ObstacleSet obstacleSet = item.GetComponent<ObstacleSet>();
            if (obstacleSet != null)
            {
                obstacleSet.changeObstacleTime(obstacleSet.speeds[Random.Range(0,obstacleSet.speeds.Length)]);
            }

        }

        nextMaterial = GameController.Instance.getNextMaterial();

    }


    public void Reset()
    {
        colorChangeCounter = 0;
        alarmTrig = false;
        this.enabled = false;
    }

    private int colorChangeInterval = 800;
    private int colorChangeCounter;
    private Material nextMaterial;

    private bool alarmTrig;

    private void FixedUpdate()
    {

        if (GameController.Instance == null) return;
        if (GameController.Instance.level == null) return;

        if (GameController.Instance.currentGameState == GameController.GameState.OnGamePlaying && GameController.Instance.level.mode == LevelIdentifier.GameMode.Panic)
        {

            if (colorChangeCounter > colorChangeInterval - 500)
            {
                GuiController.Instance.highLightLeft.gameObject.SetActive(true);
                GuiController.Instance.highLightRight.gameObject.SetActive(true);

                GuiController.Instance.highLightLeft.color = nextMaterial.GetColor("_Color");
                GuiController.Instance.highLightRight.color = nextMaterial.GetColor("_Color");

                leftAlpha.duration = 1.0f - (1.0f / colorChangeInterval * colorChangeCounter);
                rightAlpha.duration = 1.0f - (1.0f / colorChangeInterval * colorChangeCounter);

                audioTick.pitch = 2.0f / colorChangeInterval * colorChangeCounter;

                if (alarmTrig == false)
                {
                    GameController.Instance.playAudio("AudioAlarm");
                    audioTick.Play();

                    alarmTrig = true;
                }

            }
            else if (colorChangeCounter < 300)
            {
                GuiController.Instance.highLightLeft.gameObject.SetActive(false);
                GuiController.Instance.highLightRight.gameObject.SetActive(false);
            }


            if (colorChangeCounter >= colorChangeInterval)
            {
                colorChangeCounter = -50;
                GameController.Instance.changeBallMaterial(nextMaterial);

                GameController.Instance.playAudio("AudioPoof");
                GameController.Instance.playAudio("AudioColorPie");
                GameController.Instance.doFx("smokePoofSmall", GameController.Instance.ball.transform.position);

                alarmTrig = false;
                audioTick.Stop();
                nextMaterial = GameController.Instance.getNextMaterial();
              
            }

            colorChangeCounter++;
        }

    }


}
