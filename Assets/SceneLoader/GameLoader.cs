using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CinemaDirector;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour {

    
    public Slider progress;

    private AsyncOperation async = null;
    public bool autoLoad;
    [SerializeField] string nextLevelName;
    private void Start()
    {
        if (autoLoad)
        {
            loadGame();
        }
       
    }


    public void loadGame()
    {
        StartCoroutine(LoadALevel(nextLevelName));
    }

    private void Update()
    {
        if (null == async)
            return;

        if (progress != null)
        {
            progress.value = async.progress / 0.9f;
        }

        if (async.progress >= 0.9f)
            async.allowSceneActivation = true;
    }


    private IEnumerator LoadALevel(string levelName)
    {
        async = SceneManager.LoadSceneAsync(levelName);
        yield return async;
    }


}
