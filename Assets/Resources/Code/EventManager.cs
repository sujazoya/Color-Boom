using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class EventManager : MonoBehaviour {
   
    // Singleton
    private static EventManager instance = null;
    public static EventManager Instance
    {
        get
        {
            return instance;
        }
    }

    // Menu State Events
    public UnityEvent onMainMenu = new UnityEvent();

    // Game State Events
    public UnityEvent onReset = new UnityEvent();
    public UnityEvent onEnter = new UnityEvent();
    public UnityEvent onStart = new UnityEvent();
    public UnityEvent onPause = new UnityEvent();
    public UnityEvent onResume = new UnityEvent();
    public UnityEvent onFailed = new UnityEvent();
    public UnityEvent onSuccess = new UnityEvent();

    // Game Element Events
    public UnityEvent onStartPicked = new UnityEvent();
    public UnityEvent onLevelEndPicked = new UnityEvent();
    public UnityEvent onClockStart = new UnityEvent();
    public UnityEvent onClockOver = new UnityEvent();

    private void Awake()
    {
        // Singleton
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        instance = this;

    }


}
