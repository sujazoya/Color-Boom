using UnityEngine;
using System.Collections;
using CinemaDirector;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {

    public Cutscene laodCutscene;
    public UIProgressBar progress;

    private AsyncOperation async = null;


    private void Start()
    {
        if (laodCutscene != null)
        {
            laodCutscene.Play();
        }
    }


    public void loadGame()
    {
        StartCoroutine(LoadALevel("MainMenu"));
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
