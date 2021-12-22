using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ClockTimer : MonoBehaviour {

    // Singleton
    private static ClockTimer instance = null;
    public static ClockTimer Instance
    {
        get
        {
            return instance;
        }
    }


    [HideInInspector]
    public bool clockEnabled;
    private Coroutine currentWorker;


    [HideInInspector]
    public ElementIdentifier.ElementType currentElementType;

    void Awake()
    {
       
        // Singleton
        //if (instance != null && instance != this)
        //{
        //    Destroy(this.gameObject);
        //}
        instance = this;

    }


    public void startClockTimer(float duration,ElementIdentifier.ElementType elementType)
    {

        if (duration <= 0) return;

        currentElementType = elementType;

        GameController.Instance.onClockStart(currentElementType);

        CameraBrightnessEffect fx = GameController.Instance.mainCamera.GetComponent<CameraBrightnessEffect>();
        if (fx != null)
        {
            fx.doWhiteBrightnessOut(0.3f);
        }

        GuiController.Instance.clockCountDownLabel.color = Color.white;
        GuiController.Instance.clockAnimation.HardResetToBeginning();
      
        GameController.Instance.changeMusic("ClockTick");
        GameController.Instance.playAudio("MainMusic");


        countDownClockTime(duration);
        clockEnabled = true;

    }

    public void pauseClockTimer()
    {
        if (clockEnabled == true)
        {
            GameController.Instance.stopAudio("AudioClockTick");
            GuiController.Instance.clockTimerHolder.PlayForward();
        }
 
    }

    public void resumeClockTimer()
    {
        if (clockEnabled == true)
        {
            GameController.Instance.playAudio("AudioClockTick");
           GuiController.Instance.clockTimerHolder.PlayReverse();
        }
    }



    private void countDownClockTime(float duration)
    {
      
        currentWorker = StartCoroutine(clockCountDownWorker(duration));
    }

    private IEnumerator clockCountDownWorker(float duration)
    {
       

        float elapsedTime = 0;
        bool redTrig = false;

        while (elapsedTime <= duration)
        {

            if (Time.timeScale > 0)
            {

                float timeRemain = duration - elapsedTime;

                GuiController.Instance.clockCountDownLabel.text = FloatToTime(timeRemain, "#00.000");
                elapsedTime += Time.deltaTime;

                if (timeRemain < 2.0f & redTrig == false)
                {
                    redTrig = true;
                    GuiController.Instance.clockCountDownLabel.color = Color.red;
                    GuiController.Instance.clockAnimation.PlayForward();
                }

            }

            yield return new WaitForEndOfFrame();
        }


        endClockTimer();
        yield break;

    }

    public static string FloatToTime(float toConvert, string format)
    {
        switch (format)
        {
            case "00.0":
                return string.Format("{0:00}:{1:0}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
              
            case "#0.0":
                return string.Format("{0:#0}:{1:0}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
              
            case "00.00":
                return string.Format("{0:00}:{1:00}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 100) % 100));//miliseconds
               
            case "00.000":
                return string.Format("{0:00}:{1:000}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
                
            case "#00.000":
                return string.Format("{0:#00}:{1:#00}",
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 100) % 100));//miliseconds
             
            case "#0:00":
                return string.Format("{0:#0}:{1:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60);//seconds
              
            case "#00:00":
                return string.Format("{0:#00}:{1:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60);//seconds
              
            case "0:00.0":
                return string.Format("{0:0}:{1:00}.{2:0}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
              
            case "#0:00.0":
                return string.Format("{0:#0}:{1:00}.{2:0}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 10) % 10));//miliseconds
             
            case "0:00.00":
                return string.Format("{0:0}:{1:00}.{2:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 100) % 100));//miliseconds
          
            case "#0:00.00":
                return string.Format("{0:#0}:{1:00}.{2:00}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 100) % 100));//miliseconds
               
            case "0:00.000":
                return string.Format("{0:0}:{1:00}.{2:000}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
              
            case "#0:00.000":
                return string.Format("{0:#0}:{1:00}.{2:000}",
                    Mathf.Floor(toConvert / 60),//minutes
                    Mathf.Floor(toConvert) % 60,//seconds
                    Mathf.Floor((toConvert * 1000) % 1000));//miliseconds
            
        }

        return "error";
    }


    private void endClockTimer()
    {
 
        stopClockTimer();

        GameController.Instance.playAudio("AudioAlarm");
        GameController.Instance.stopAudio("AudioClockTick");

        GameController.Instance.changeMusic("Music");
        GameController.Instance.playAudio("MainMusic");

    }

    public void stopClockTimer()
    {

        GameController.Instance.onClockEnd(currentElementType);

        if (currentWorker != null)
        {
            StopCoroutine(currentWorker);
        }

        GuiController.Instance.clockCountDownLabel.color = Color.white;
        GuiController.Instance.clockCountDownLabel.text = "00:00";
        GuiController.Instance.clockAnimation.HardResetToBeginning();

        clockEnabled = false;

    }

   
}
