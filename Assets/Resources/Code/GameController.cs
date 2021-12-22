using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;
using CinemaDirector;
using System.Collections.Generic;
using UnityEngine.UI;


[System.Serializable]
public class MenuHolder
{
    public GameObject[] menus;
    public GameObject transition;
    [Header("Sprites")]
    public Sprite audioOn, audioOff;
    public Image[] musicSpriteRenderers;
}

public class GameController : MonoBehaviour
{


    // Singleton
    private static GameController instance = null;
    public static GameController Instance
    {
        get
        {
            return instance;
        }
    }
    public MenuHolder menuHolder;


    public bool TestMode;

    [HideInInspector]
    public int collectedStarCount;

    public LevelIdentifier level;

    public GameObject ball;
   // [HideInInspector]
    public GameObject ballMesh;
    private DOTweenAnimation ballRotator;
    private Material ballLinesMat;

    //[HideInInspector]
    public Rigidbody ballRigid;
    private SphereCollider ballCollider;
    private MeshRenderer ballRenderer;
    public GameObject ballRocketFx;
    public GameObject ballShieldFx;
    private TrailRenderer ballTrailRenderer;

    public ForceMode forceMode;
    public float hitForce;
    public float hitForceLimit;

    public Material[] gameMaterials;
    [HideInInspector]
    public Material selectedMaterial;

    public GameObject startCutsceneHolder;
    private Cutscene startCutscene;

    public GameObject endCutsceneHolder;
    private Cutscene endCutscene;
    public GameObject endCutsceneBall;

    public GameObject startPlatform;
    public GameObject endPlatform;
    public GameObject highScorePlatform;
    public GameObject highScorePlatformFireworks;

   // [HideInInspector]
    public GameObject mainCamera;

    public CameraEarthQuake cameraShake;

    //[HideInInspector]
    public CameraFollow cameraFollower;

    // Infinite
    public GameObject infiniteRunHolder;

    // Level
    public GameObject levelNameHolder;


    [HideInInspector]
    public bool infiniteHighScorePassedTrig;

    // Audio
    public GameObject audioRoot;
    private List<AudioSource> audios;   
    //[SerializeField] GameObject startMenu;
    [SerializeField] Text lifeText;
    [SerializeField] Text debugText;
    //Animator lifeAnimator;   
    AdmobAdmanager admobAdmanager;

    [SerializeField] BoxCollider baseTrig;
    [SerializeField] GameObject endPlatform_Star;

    public enum GameState
    {
        None = 0,
        OnMainMenu = 1,
        OnGameEnter = 2,
        OnGamePlaying = 3,
        OnPaused = 4,
        OnFailed = 5,
        OnSucceed = 6,
        OnLevelMenu = 7
    }
    public GameState currentGameState;


    public void ActivateMenu(int state)
    {
        StartCoroutine(ShowUI(state));
    }
    public void DisableAllUI()
    {
        for (int i = 0; i < menuHolder.menus.Length; i++)
        {
            menuHolder.menus[i].SetActive(false);
        }
    }
    IEnumerator  ShowUI(int state)
    {
        menuHolder.transition.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        menuHolder.transition.SetActive(true);
        yield return new WaitForSeconds(1f);
        DisableAllUI();
        if (state == 0)
        {
            Time.timeScale = 1;
            menuHolder.menus[0].SetActive(true);
            yield return new WaitForSeconds(1f);
            setOnMainMenu();            
        }
        else
             if (state == 5)
        {
            menuHolder.menus[5].SetActive(true);
        }
        else
             if (state == 1)
        {
            menuHolder.menus[1].SetActive(true);
        }
        else
             if (state ==2)
        {
            menuHolder.menus[2].SetActive(true);
            yield return new WaitForSeconds(1f);
            setOnPause();
        }
        else
             if (state == 3)
        {
            menuHolder.menus[3].SetActive(true);
        }
        else
             if (state == 4)
        {
            menuHolder.menus[4].SetActive(true);
        }
        yield return new WaitForSeconds(1f);
        menuHolder.transition.SetActive(false);
    }
    public void BackToFail()
    {      
        ActivateMenu(4);
    }
    public static int Life
    {
        get { return PlayerPrefs.GetInt("Life", 0); }
        set { PlayerPrefs.SetInt("Life", value); }
    }
    public static void AddLife()
    {
        Life++;   
        instance.UpdateLifeText();
    }
    void UpdateLifeText()
    {
        lifeText.text = Life.ToString();
    }

   public void Awake()
    {

        // Singleton
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        instance = this;       
    }

    public void FirstPlay()
    {
        //startMenu.SetActive(false);
        //setOnMainMenu();

    }  
    void Start()
    {     
     
        //levelName = "Level1";
        if (!ballMesh)
        {
            ballMesh = ball.transform.Find("mesh").gameObject;
        }       

       // ballRigid = ball.GetComponent<Rigidbody>();
        ballCollider = ball.GetComponent<SphereCollider>();

        ballRotator = ballMesh.GetComponent<DOTweenAnimation>();
        ballRenderer = ballMesh.GetComponent<MeshRenderer>();
        ballLinesMat = ballMesh.GetComponent<MeshRenderer>().sharedMaterials[0];
        ballTrailRenderer = ballMesh.GetComponent<TrailRenderer>();

        ColliderTrigger ballEnterTrigger = ball.GetComponent<ColliderTrigger>();
        ballEnterTrigger.OnEnter = (ColliderTrigger.OnEnterDelegate)Delegate.Combine(ballEnterTrigger.OnEnter, new ColliderTrigger.OnEnterDelegate(this.ballTriggerEnter));

        ColliderTrigger ballExitTrigger = ball.GetComponent<ColliderTrigger>();
        ballExitTrigger.OnExit = (ColliderTrigger.OnExitDelegate)Delegate.Combine(ballExitTrigger.OnExit, new ColliderTrigger.OnExitDelegate(this.ballTriggerExit));

        // Audio
        //audioRoot = GameObject.Find("Audios");
        cacheAllAudioFiles();

        // Cam
        //mainCamera = GameObject.Find("Main Camera");
        //cameraFollower = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        //cameraShake = GameObject.Find("Main Camera").GetComponent<CameraEarthQuake>();


        // End Cutscene
        if (endCutsceneHolder != null)
        {
            endCutscene = endCutsceneHolder.GetComponent<Cutscene>();

        }
        // Start Cutscene
        if (startCutsceneHolder != null)
        {
            startCutscene = startCutsceneHolder.GetComponent<Cutscene>();
        }
        if (startCutscene.State == Cutscene.CutsceneState.Playing)
        {
            startCutscene.Stop();
        }
        //EventManager.Instance.onMainMenu.Invoke();
        //currentGameState = GameState.OnMainMenu;
        mainCamera.transform.localPosition = new Vector3(0, -1000, 0);
        //StartCoroutine(GoOnMenu());      
      
        setOnResetGame();
        //startMenu.SetActive(true);
        refreshAudioSetting();
        refreshAllAudioButtons();
        ActivateMenu(0);
        //setOnMainMenu();
        UpdateLifeText();
    }
    #region AUDIOSETTING

    public void switchAudioSetting()
    {


        if (PlayerPrefs.GetInt("Mute", 0) == 0)
        {
            PlayerPrefs.SetInt("Mute", 1);
        }
        else if (PlayerPrefs.GetInt("Mute", 0) == 1)
        {
            PlayerPrefs.SetInt("Mute", 0);
        }

        refreshAudioSetting();
        refreshAllAudioButtons();
    }
    private void refreshAudioSetting()
    {

        if (Time.timeScale <= 0) return;

        if (PlayerPrefs.GetInt("Mute", 0) == 0)
        {
            AudioListener.volume = 1;
        }
        else if (PlayerPrefs.GetInt("Mute", 0) == 1)
        {
            AudioListener.volume = 0;
        }
    }

    private void refreshAllAudioButtons()
    {
        if (menuHolder.musicSpriteRenderers == null) return;

        foreach (Image btn in menuHolder.musicSpriteRenderers)
        {
            if (PlayerPrefs.GetInt("Mute", 0) == 0)
            {
                for (int i = 0; i < menuHolder.musicSpriteRenderers.Length; i++)
                {
                    menuHolder.musicSpriteRenderers[i].sprite = menuHolder.audioOn;
                }              
            }
            else if (PlayerPrefs.GetInt("Mute", 0) == 1)
            {
                for (int i = 0; i < menuHolder.musicSpriteRenderers.Length; i++)
                {
                    menuHolder.musicSpriteRenderers[i].sprite = menuHolder.audioOff;
                }
            }
        }

    }

    #endregion




    //IEnumerator GoOnMenu()
    //{
    //    yield return new WaitForSeconds(.5f);
    //    EventManager.Instance.onReset.Invoke();
    //    currentGameState = GameState.None;
    //    yield return new WaitForSeconds(.5f);
    //    EventManager.Instance.onMainMenu.Invoke();
    //    currentGameState = GameState.OnMainMenu;
    //}
    public void setOnMainMenu()
    {
        //EventManager.Instance.onMainMenu.Invoke();
        currentGameState = GameState.OnMainMenu;
        setOnResetGame();

        if (startCutscene.State == Cutscene.CutsceneState.Playing)
        {
            startCutscene.Stop();
        }
        //this.enabled = false;

        mainCamera.transform.position = new Vector3(0, -1000, 0);

    }

    public void setOnResetGame()
    {

        //EventManager.Instance.onReset.Invoke();

        //  this.enabled = true;

        stopPshycs();
        isGameRunning = false;

        infiniteHighScorePassedTrig = false;

        cameraFollower.enabled = false;
        cameraFollower.reset();

        mainCamera.transform.position = new Vector3(0, 0, -cameraFollower.distance);

        ball.transform.position = new Vector3(0, 5, 0);
        ball.gameObject.SetActive(true);
        ballMesh.gameObject.SetActive(true);
        ball.transform.position = Vector3.zero;
        ballTrailRenderer.enabled = true;

        startPlatform.transform.position = new Vector3(0, 0, 0);
        startPlatform.transform.rotation = Quaternion.identity;

        endPlatform.transform.position = new Vector3(0, 35, 0);
        endPlatform.transform.rotation = Quaternion.identity;

        ClockTimer.Instance.stopClockTimer();



        PanicController.Instance.Reset();

        LevelLibrary.Instance.destroyCurrentLevel();

        GuiController.Instance.lblTime.gameObject.SetActive(false);
        GuiController.Instance.lblTime.color = Color.white;
        GuiController.Instance.lblTime.text = "00:00";
        GuiController.Instance.lblTimeAnim.HardResetToBeginning();

        GuiController.Instance.sprNoTime.gameObject.SetActive(true);

        stopRocketWorker();
        stopExplodeObjectWorker();
        stopObstacleSpeedChangeWorker();

        collectedStarCount = 0;
        GuiController.Instance.lblHudStarTotal.text = "0";
        GuiController.Instance.lblPauseStartTotal.text = "0";

        Time.timeScale = 1;
        DOTween.timeScale = 1;

        if (PlayerPrefs.GetInt("Mute", 0) == 0)
        {
            AudioListener.volume = 1;
        }

        changeMusic("Music");
        stopAudio("AudioShield");

        // Fast Reload Button
        //GuiController.Instance.btnFastReload.gameObject.SetActive(TestMode);
        if (level != null)
        {
            switch (level.mode)
            {
                case LevelIdentifier.GameMode.Infinite:
                    InfiniteLevelController.Instance.reset();
                    break;
                case LevelIdentifier.GameMode.Classic:
                case LevelIdentifier.GameMode.Timed:
                case LevelIdentifier.GameMode.LightsOut:
                case LevelIdentifier.GameMode.Panic:
                    stopLevelTime();
                    break;
            }
        }

        //adManager.ShowIntersitial();
    }




    #region "Methods"


    private void cacheAllAudioFiles()
    {

        if (audioRoot == null) return;

        audios = new List<AudioSource>();

        foreach (Transform audio in audioRoot.transform)
        {
            AudioSource audioSource = audio.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audios.Add(audioSource);
            }
        }
    }

    public void playAudio(string name)
    {
        if (audios == null) return;
        foreach (AudioSource audio in audios)
        {
            if (audio.gameObject.name.ToLower().Equals(name.ToLower()))
            {
                audio.Play();
            }
        }
    }
    public void stopAudio(string name)
    {
        if (audios == null) return;
        foreach (AudioSource audio in audios)
        {
            if (audio.name.ToLower().Equals(name.ToLower()))
            {
                audio.Stop();
            }
        }
    }

    public void changeMusic(string name)
    {
        if (audios == null) return;

        AudioSource themeAudioHolder = GameObject.Find("MainMusic").GetComponent<AudioSource>();
        if (themeAudioHolder != null)
        {

            AudioClip clip = Resources.Load("Audio/" + name) as AudioClip;
            if (clip != null)
            {
                themeAudioHolder.clip = clip;
            }

        }

    }


    public void hitTheBall()
    {
        ballRigid.AddForce(Vector3.up * hitForce, forceMode);
        playAudio("AudioBounce");
        ballRotator.DOPause();
    }
    public void changeBallMaterial(Material mat)
    {
        selectedMaterial = mat;
        ballRenderer.materials = new Material[] { ballLinesMat, mat };
    }

    public Material getNextMaterial()
    {

        List<Material> newlist = new List<Material>();

        for (int i = 0; i < GameController.Instance.gameMaterials.Length; i++)
        {
            if (!GameController.Instance.gameMaterials[i].name.ToLower().Equals(GameController.Instance.selectedMaterial.name.ToLower()))
            {
                newlist.Add(GameController.Instance.gameMaterials[i]);
            }
        }


        if (newlist.Count <= 0)
        {
            return GameController.Instance.gameMaterials[UnityEngine.Random.Range(0, GameController.Instance.gameMaterials.Length)];
        }

        return newlist[UnityEngine.Random.Range(0, newlist.Count)];

    }


    private GameObject fxRoot;

    public void doFx(string fxName, Vector3 pos)
    {

        if (fxRoot == null)
        {
            fxRoot = new GameObject();
            fxRoot.name = "FX Root";
            fxRoot.transform.position = Vector3.zero;
        }

        if (fxRoot.transform.childCount > 6) return;

        GameObject fx = (GameObject)Instantiate(Resources.Load("Prefabs/Effects/" + fxName));

        if (fx != null)
        {
            fx.transform.parent = fxRoot.transform;
            fx.AddComponent<ParticleEffectDestroyer>();
            fx.transform.position = pos;
        }

    }

    public void closeLevelName()
    {
        if (levelNameHolder == null) return;

        Invoke("levelNameDisabler", 2.0f);

    }

    private void levelNameDisabler()
    {

        TweenScale levelNameAnim = levelNameHolder.GetComponent<TweenScale>();
        if (levelNameAnim != null)
        {
            levelNameAnim.PlayReverse();
        }

    }

    public void closeInfiniteRun()
    {
        if (infiniteRunHolder == null) return;

        Invoke("infiniteRunDisabler", 2.0f);

    }

    private void infiniteRunDisabler()
    {

        TweenScale tweenAnim = infiniteRunHolder.GetComponent<TweenScale>();
        if (tweenAnim != null)
        {
            tweenAnim.PlayReverse();
        }

    }


    private void setLevelLabel()
    {
        Transform levelNumberHolder = levelNameHolder.transform.Find("LevelNumber");

        if (levelNumberHolder != null)
        {
            // Clearing
            foreach (Transform item in levelNumberHolder)
            {
                Destroy(item.gameObject);
            }

            Transform numbersHolder = levelNameHolder.transform.Find("Numbers");
            Material numberMaterial = (Material)Resources.Load("Maps/Materials/ColorC", typeof(Material));
            float curX = 0;

            for (int i = 0; i < level.levelIndex.ToString().Length; i++)
            {
                string currentLetter = level.levelIndex.ToString().Substring(i, 1);

                GameObject newLetter = new GameObject();
                newLetter.transform.parent = levelNumberHolder.transform;
                newLetter.name = currentLetter;
                newLetter.transform.position = new Vector3(0, 0, 0);
                newLetter.transform.localPosition = new Vector3(curX, 0, 0);
                newLetter.transform.localScale = new Vector3(1, 1, 1);
                newLetter.transform.Rotate(0, 180, 0);

                MeshFilter newLetterMF = newLetter.AddComponent<MeshFilter>();

                foreach (Transform letter in numbersHolder)
                {
                    if (letter.name.ToLower().Equals(currentLetter.ToLower()))
                    {
                        newLetterMF.mesh = letter.GetComponent<MeshFilter>().mesh;
                    }
                }

                MeshRenderer newLetterMR = newLetter.AddComponent<MeshRenderer>();
                newLetterMR.material = numberMaterial;

                curX -= 2.2f;
            }

        }

        TweenScale scaleAnim = levelNumberHolder.GetComponent<TweenScale>();
        if (scaleAnim != null)
        {
            scaleAnim.HardResetToBeginning();
        }

    }

    #endregion

    #region "Powerups"

    // BOMB
    private List<Coroutine> explodeObjectWotkerCoroutines;

    public void explodeObject(GameObject objectToExplode, float delay)
    {

        explodeObjectWotkerCoroutines = new List<Coroutine>();
        explodeObjectWotkerCoroutines.Add(StartCoroutine(explodeObjectWorker(objectToExplode, delay)));

    }

    private void stopExplodeObjectWorker()
    {
        if (explodeObjectWotkerCoroutines != null)
        {
            foreach (Coroutine worker in explodeObjectWotkerCoroutines)
            {
                StopCoroutine(worker);
            }

        }
    }

    private IEnumerator explodeObjectWorker(GameObject objectToExplode, float delay)
    {
        yield return new WaitForSeconds(delay);

        objectToExplode.gameObject.SetActive(false);
        doFx("smokePoof", objectToExplode.transform.position);
        playAudio("AudioExplosion");
    }

    // SLOW MO
    private Coroutine obstacleSpeedChangeWorkerCoroutine;

    public void changeObstaclesSpeed(float speed)
    {
        float speedChangeDuration = 1.0f;

        if (obstacleSpeedChangeWorkerCoroutine != null)
        {
            StopCoroutine(obstacleSpeedChangeWorkerCoroutine);
        }

        obstacleSpeedChangeWorkerCoroutine = StartCoroutine(changeObstaclesSpeedWorker(speedChangeDuration, speed));
    }

    private void stopObstacleSpeedChangeWorker()
    {

        if (obstacleSpeedChangeWorkerCoroutine != null)
        {
            StopCoroutine(obstacleSpeedChangeWorkerCoroutine);
        }

        DOTween.timeScale = 1.0f;

    }

    private IEnumerator changeObstaclesSpeedWorker(float duration, float speed)
    {

        float elapsedTime = 0;
        float currentSpeed = DOTween.timeScale;

        while (elapsedTime <= duration)
        {

            if (Time.timeScale > 0)
            {
                float percent = 1.0f / duration * elapsedTime;
                DOTween.timeScale = Mathf.Lerp(DOTween.timeScale, speed, percent);
                elapsedTime += Time.deltaTime;
            }

            yield return new WaitForEndOfFrame();
        }



        yield break;

    }

    // ROCKET
    private Coroutine rocketDisableWorkerCoroutine;
    private bool rocketActive;

    private void launchRocket(Vector3 startPos, float distance, float time)
    {
        if (rocketDisableWorkerCoroutine != null)
        {
            StopCoroutine(rocketDisableWorkerCoroutine);
        }

        if (distance <= 0) return;
        if (time <= 0) return;

        ballRigid.isKinematic = true;
        ballRotator.DOPause();
        rocketActive = true;
        ballRocketFx.gameObject.SetActive(true);
        ballTrailRenderer.enabled = false;

        // Deleting Obstacle Where Rocket End
        if (level.mode == LevelIdentifier.GameMode.Infinite)
        {

            List<int> objectsToDelete = new List<int>();

            for (int i = 0; i < InfiniteLevelController.Instance.levelElements.Count; i++)
            {
                GameObject item = InfiniteLevelController.Instance.levelElements[i];

                float endPos = startPos.y + distance;

                if (item.transform.position.y > (endPos - 5) && item.transform.position.y < (endPos + 5))
                {
                    objectsToDelete.Add(i);
                }

            }

            for (int i = 0; i < objectsToDelete.Count; i++)
            {
                InfiniteLevelController.Instance.levelElements[objectsToDelete[i]].gameObject.SetActive(false);
            }

        }


        playAudio("AudioRocketLoop");

        Vector3[] rocketPath = new Vector3[2];
        rocketPath[0] = startPos;
        rocketPath[1] = new Vector3(startPos.x, startPos.y + distance, startPos.z);

        iTween.MoveTo(ball, iTween.Hash("path", rocketPath, "movetopath", false, "orienttopath", true, "time", time, "delay", 0.0f, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.none));

        rocketDisableWorkerCoroutine = StartCoroutine(disableRocket(time));

    }

    private void stopRocketWorker()
    {

        if (rocketDisableWorkerCoroutine != null)
        {
            StopCoroutine(rocketDisableWorkerCoroutine);
        }

        rocketActive = false;
        ballRocketFx.gameObject.SetActive(false);
        ballTrailRenderer.enabled = true;

        stopAudio("AudioRocketLoop");


    }

    private IEnumerator disableRocket(float duration)
    {

        float elapsedTime = 0;
        float currentSpeed = DOTween.timeScale;

        while (elapsedTime <= duration)
        {
            if (Time.timeScale > 0)
            {
                elapsedTime += Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }

        rocketActive = false;
        ballRocketFx.gameObject.SetActive(false);
        ballTrailRenderer.enabled = true;
        stopAudio("AudioRocketLoop");

        ballRigid.isKinematic = false;
        ballRotator.DOPlay();
        yield break;

    }


    public void showXP(string xp, Color xpLabelColor, string xpDesc, float duration)
    {

        GuiController.Instance.xpHolder.gameObject.SetActive(true);
        GuiController.Instance.lblXp.text = xp;
        GuiController.Instance.lblXp.color = xpLabelColor;
        GuiController.Instance.lblXpDesc.text = xpDesc;
        GuiController.Instance.xpAnim.HardResetToBeginning();
        GuiController.Instance.xpAnim.PlayForward();

        Invoke("xpBackAnim", duration);

    }


    private void xpBackAnim()
    {
        GuiController.Instance.xpAnim.PlayReverse();
        Invoke("hideXpPanel", 1.0f);
    }

    private void hideXpPanel()
    {
        GuiController.Instance.lblXp.color = Color.white;
        GuiController.Instance.xpHolder.gameObject.SetActive(false);
    }


    private void highScoreFireworksDisable()
    {
        highScorePlatformFireworks.gameObject.SetActive(false);
    }


    #endregion

    #region "Collisions"
    bool ballHided;
    IEnumerator HideBallCollider()
    {
        ballShieldFx.gameObject.SetActive(true);
        ballHided = true;
        yield return new WaitForSeconds(.5f);
        ballShieldFx.gameObject.SetActive(false);
        ballHided = false;
    }
    private void ballTriggerEnter(Collider collider)
    {

        ElementIdentifier actionItem = collider.gameObject.GetComponent<ElementIdentifier>();

        if (actionItem == null)
        {
            actionItem = collider.transform.parent.GetComponent<ElementIdentifier>();
        }


        if (actionItem != null)
        {
            //Debug.Log(actionItem.objectType);

            switch (actionItem.elementType)
            {
                case ElementIdentifier.ElementType.None:
                    break;

                case ElementIdentifier.ElementType.Base:
                    hitTheBall();
                    ballRotator.DOPlay();
                    break;

                case ElementIdentifier.ElementType.Obstacle:

                    collider.enabled = false;

                    MeshRenderer obsRenderrer = collider.gameObject.GetComponent<MeshRenderer>();

                    if (obsRenderrer == null)
                    {
                        obsRenderrer = collider.transform.parent.GetComponent<MeshRenderer>();
                    }

                    if (obsRenderrer != null)
                    {
                        bool cantTouchThis = false;

                        // Shield
                        if (ClockTimer.Instance.clockEnabled == true)
                        {
                            if (ClockTimer.Instance.currentElementType == ElementIdentifier.ElementType.Shield)
                            {
                                cantTouchThis = true;
                            }
                        }

                        // Rocket
                        if (rocketActive == true)
                        {
                            cantTouchThis = true;
                        }


                        if (cantTouchThis == true)
                        {
                            collider.enabled = false;

                            if (collider.GetComponent<MeshRenderer>() == null)
                            {
                                collider.transform.parent.gameObject.SetActive(false);
                            }
                            else
                            {
                                collider.gameObject.SetActive(false);
                            }

                            playAudio("AudioPoof");
                            doFx("smokePoofSmall", collider.gameObject.transform.position);

                            return;
                        }


                        if (!ballRenderer.materials[1].name.ToLower().Equals(obsRenderrer.material.name.ToLower()))
                        {
                            if (TestMode == false)
                            {
                                lifeText.text = Life.ToString();
                               // lifeAnimator.enabled = false;
                               // lifeAnimator.enabled = true;
                                if (Life <= 0 && !ballHided)
                                {
                                    doFailed();

                                    int bestScore = Achievements.Instance.getInfiniteBestScore();
                                    SocialServices.Instance.postLeaderBoardScore(bestScore);

                                    if (collider.gameObject.transform.parent != null)
                                    {
                                        SocialServices.Instance.logAnalyticEvent("GameEvents", "ObstacleHit", collider.gameObject.transform.parent.name, 1);
                                    }
                                }
                                else
                                {
                                    if (!ballHided)
                                    {
                                        StartCoroutine(HideBallCollider());
                                        Life--;
                                    }
                                   
                                }
                               

                            }
                        }

                    }


                    break;
                case ElementIdentifier.ElementType.Star:

                    collider.enabled = false;
                    collider.gameObject.SetActive(false);

                    EventManager.Instance.onStartPicked.Invoke();
                    playAudio("AudioStar");

                    doFx("starPoof", collider.gameObject.transform.position);

                    int startCountToAdd = 1;

                    if (PlayerPrefs.GetInt("doubleStars", 0) == 1)
                    {
                        startCountToAdd = 2;
                    }

                    collectedStarCount += startCountToAdd;

                    Achievements.Instance.addStartPoints(startCountToAdd);
                    Achievements.Instance.updateGuiLabels();

                    switch (level.mode)
                    {
                        case LevelIdentifier.GameMode.Infinite:

                            int bestScore = Achievements.Instance.getInfiniteBestScore();

                            if (collectedStarCount > bestScore)
                            {
                                Achievements.Instance.setInfiniteBestScore(collectedStarCount);
                                GuiController.Instance.lblFailBestScore.text = collectedStarCount.ToString();
                                SocialServices.Instance.postLeaderBoardScore(collectedStarCount);
                            }


                            if (actionItem.recordObject == true)
                            {
                                infiniteHighScorePassedTrig = true;
                                playAudio("AudioHighScore");
                                playAudio("AudioHighScoreFlame");
                                showXP("New", new Color32(255, 255, 255, 255), "Record", 2.0f);
                                CameraBrightnessEffect.Instance.doWhiteBrightnessOut(0.5f);
                                highScorePlatformFireworks.gameObject.SetActive(true);
                                Invoke("highScoreFireworksDisable", 6.5f);

                                SocialServices.Instance.logAnalyticEvent("GameEvents", "Achievement", "HighScroreRecord", collectedStarCount);
                                SocialServices.Instance.postAchievement(SocialServices.Achievements.FirstRecord);

                            }

                            break;
                        case LevelIdentifier.GameMode.Classic:
                        case LevelIdentifier.GameMode.Timed:
                        case LevelIdentifier.GameMode.LightsOut:
                        case LevelIdentifier.GameMode.Panic:

                            // DO 

                            break;

                    }



                    break;
                case ElementIdentifier.ElementType.ColorChanger:

                    collider.enabled = false;
                    collider.gameObject.SetActive(false);

                    playAudio("AudioPoof");
                    playAudio("AudioColorPie");

                    doFx("smokePoofSmall", collider.gameObject.transform.position);
                    //logEvent("GameEvents", "Collect", "ColorChanger", 1);

                    if (collectedStarCount == 1)
                    {
                        SocialServices.Instance.postAchievement(SocialServices.Achievements.PassFirstObstacle);
                    }

                    PropColorChanger colorChanger = collider.gameObject.GetComponent<PropColorChanger>();
                    if (colorChanger != null)
                    {
                        changeBallMaterial(getNextMaterial());
                    }

                    if (level.mode == LevelIdentifier.GameMode.Infinite)
                    {
                        InfiniteLevelController.Instance.refreshAllTriangles();
                    }

                    break;
                case ElementIdentifier.ElementType.Bomb:

                    collider.enabled = false;
                    collider.gameObject.SetActive(false);

                    playAudio("AudioPoof");
                    playAudio("AudioPowerupCollect");

                    doFx("smokePoofSmall", collider.gameObject.transform.position);
                    SocialServices.Instance.logAnalyticEvent("GameEvents", "Collect", "Bomb", 1);
                    SocialServices.Instance.postAchievement(SocialServices.Achievements.CollectBomb);


                    PropBomb bomb = collider.gameObject.GetComponent<PropBomb>();
                    if (bomb != null)
                    {
                        bomb.discoverObstacles();
                        bomb.Expolde();
                        showXP("Bomb", new Color32(255, 224, 53, 255), "Active", bomb.expolisonDelay * bomb.explodeObjects.Length);
                    }



                    break;
                case ElementIdentifier.ElementType.Shield:


                    collider.enabled = false;
                    collider.gameObject.SetActive(false);

                    playAudio("AudioPowerupCollect");
                    playAudio("AudioShield");

                    doFx("smokePoofSmall", collider.gameObject.transform.position);
                    //logEvent("GameEvents", "Collect", "Shield", 1);
                    SocialServices.Instance.postAchievement(SocialServices.Achievements.CollectShield);

                    PropTimer timer = collider.gameObject.GetComponent<PropTimer>();
                    if (timer != null)
                    {
                        ClockTimer.Instance.startClockTimer(timer.duration, timer.elementType);
                        showXP("Shield", new Color32(255, 224, 53, 255), "Active", timer.duration - 1.0f);

                    }

                    break;

                case ElementIdentifier.ElementType.Rocket:

                    collider.enabled = false;
                    collider.gameObject.SetActive(false);

                    playAudio("AudioPoof");
                    playAudio("AudioPowerupCollect");

                    doFx("smokePoofSmall", collider.gameObject.transform.position);

                    SocialServices.Instance.logAnalyticEvent("GameEvents", "Collect", "Rocket", 1);
                    SocialServices.Instance.postAchievement(SocialServices.Achievements.CollectRocket);

                    PropRocket rocket = collider.gameObject.GetComponent<PropRocket>();
                    if (rocket != null)
                    {
                        launchRocket(collider.transform.position, rocket.distance, rocket.time);
                        showXP("Rocket", new Color32(255, 224, 53, 255), "Active", rocket.time - 1.0f);
                    }


                    break;

                case ElementIdentifier.ElementType.SlowClock:

                    collider.enabled = false;
                    collider.gameObject.SetActive(false);

                    playAudio("AudioPoof");
                    playAudio("AudioPowerupCollect");

                    doFx("smokePoofSmall", collider.gameObject.transform.position);
                    SocialServices.Instance.logAnalyticEvent("GameEvents", "Collect", "SlowClock", 1);

                    PropTimer slowClockTimer = collider.gameObject.GetComponent<PropTimer>();
                    if (slowClockTimer != null)
                    {

                        PropSlowClock slowClock = collider.gameObject.GetComponent<PropSlowClock>();
                        if (slowClock != null)
                        {
                            changeObstaclesSpeed(slowClock.speed);

                            ClockTimer.Instance.startClockTimer(slowClockTimer.duration, slowClockTimer.elementType);
                            showXP("Time Warp", new Color32(255, 224, 53, 255), "Active", slowClockTimer.duration - 1.0f);
                        }

                    }


                    break;

                case ElementIdentifier.ElementType.HourGlass:

                    collider.enabled = false;
                    collider.gameObject.SetActive(false);

                    playAudio("AudioPoof");
                    playAudio("AudioPowerupCollect");

                    doFx("smokePoofSmall", collider.gameObject.transform.position);
                    SocialServices.Instance.logAnalyticEvent("GameEvents", "Collect", "HourGlass", 1);

                    PropHourGlass hourGlass = collider.gameObject.GetComponent<PropHourGlass>();
                    if (hourGlass != null)
                    {
                        level.levelData.levelTime += hourGlass.timetoAdd;
                        showXP("+" + hourGlass.timetoAdd + " sec", Color.green, "extra time", 2.0f);

                        if (level.levelData.levelTime - levelTimeEplased > 15)
                        {

                            GuiController.Instance.lblTime.color = Color.green;
                            //GuiController.Instance.lblTimeAnim.enabled = false;
                            stopAudio("AudioTimeAlert");
                            timeRedAlertTrig = false;
                        }

                    }


                    break;

                case ElementIdentifier.ElementType.LevelEnd:

                    collider.enabled = false;
                    collider.gameObject.SetActive(false);

                    ClockTimer.Instance.stopClockTimer();

                   
                    stopLevelTime();
                    stopRocketWorker();
                    stopExplodeObjectWorker();
                    stopObstacleSpeedChangeWorker();
                    Time.timeScale = 1;
                    DOTween.timeScale = 1;


                    isGameRunning = false;
                    stopPshycs();

                    playAudio("AudioStar");
                    doFx("smokePoofSmall", ball.transform.position);

                    if (endCutsceneBall != null)
                    {
                        endCutsceneBall.GetComponent<MeshRenderer>().materials = ballRenderer.materials;
                    }

                    ballMesh.gameObject.SetActive(false);
                    endCutscene.transform.position = ball.transform.position;
                    endCutscene.Play();                  

                    EventManager.Instance.onLevelEndPicked.Invoke();


                    break;
            }
        }


    }

    private void ballTriggerExit(Collider collider)
    {

    }
    public GameObject ballExplode;
    private void doFailed()
    {

        isGameRunning = false;
        stopPshycs();
        cameraFollower.enabled = false;

        cameraShake.Shake(0.8f);
        playAudio("AudioPoof");
        playAudio("AudioDeath");
        doFx("smokePoofSmall", ball.transform.position);
        //doFx("ballExplode", ball.transform.position);

        if (ballExplode != null)
        {
            ballExplode.SetActive(true);
            ballExplode.transform.position = ball.transform.position;
            Invoke("OffballExplode", 3f);
        }

        ballMesh.gameObject.SetActive(false);

        Invoke("setOnFailed", 0.5f);

    }
    void OffballExplode()
    {
        ballExplode.SetActive(false);
    }

    #endregion

    #region "Update"  

    [HideInInspector]
    public bool isGameRunning;



    void Update()
    {


        if (isGameRunning == false) return;
        if (rocketActive == true) return;

       
        if (ballRigid.velocity.y > hitForceLimit)
        {
            ballRigid.velocity = new Vector3(0, hitForceLimit, 0);
        }

        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1"))
        {
            hitTheBall();
        }

        if (Input.GetMouseButtonUp(0) || Input.GetButtonUp("Fire1"))
        {
            ballRotator.DOPlay();
        }
        
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                hitTheBall();
            }
            if (Input.GetTouch(i).phase == TouchPhase.Ended)
            {
                ballRotator.DOPlay();
            }
        }      
        if (debugText)
        {
            debugText.text = currentGameState.ToString();
        }
    }

    private Coroutine levelTimeCoroutine;

    private void startLevelTime()
    {

        if (level.levelData.levelTime <= 0) return;

        if (levelTimeCoroutine != null)
        {
            StopCoroutine(levelTimeCoroutine);
        }

        levelTimeEplased = 0;
        GuiController.Instance.lblTime.color = Color.green;
        levelTimeCoroutine = StartCoroutine(levelTimeWorker());

    }

    private void stopLevelTime()
    {
        if (levelTimeCoroutine != null)
        {
            StopCoroutine(levelTimeCoroutine);
        }

        GuiController.Instance.lblTime.color = Color.white;
        //GuiController.Instance.lblTimeAnim.HardResetToBeginning();
        timeRedAlertTrig = false;

        levelTimeEplased = 0;
        stopAudio("AudioTimeAlert");
        //adManager.ShowIntersitial();

    }

    private bool timeRedAlertTrig;
    private float levelTimeEplased;

    private IEnumerator levelTimeWorker()
    {

        //float elapsedTime = 0;
        levelTimeEplased = 0;

        while (levelTimeEplased <= level.levelData.levelTime)
        {

            if (Time.timeScale > 0)
            {

                float levelTimeRemain = level.levelData.levelTime - levelTimeEplased;

                if (levelTimeRemain > 60)
                {
                    GuiController.Instance.lblTime.text = ClockTimer.FloatToTime(levelTimeRemain, "#00:00");
                }
                else if (levelTimeRemain < 60)
                {
                    GuiController.Instance.lblTime.text = ClockTimer.FloatToTime(levelTimeRemain, "#00.000");
                }

                
                levelTimeEplased += Time.deltaTime;

                if (levelTimeRemain < 15.0f & timeRedAlertTrig == false)
                {
                    timeRedAlertTrig = true;
                    GuiController.Instance.lblTime.color = Color.red;
                    //GuiController.Instance.lblTimeAnim.PlayForward();
                    playAudio("AudioTimeAlert");

                }

            }

            yield return new WaitForEndOfFrame();
        }


        // Failed
        playAudio("AudioAlarm");
        stopAudio("AudioTimeAlert");
        doFailed();
        yield break;

    }


    #endregion

    #region "GUI Buttons"


    public void btnMainMenuPlayOnClick()
    {

        GameObject startButton = GameObject.Find("Circles");
        Vector3 fxPos = new Vector3(startButton.transform.position.x, startButton.transform.position.y, startButton.transform.position.z);

        playAudio("AudioStart");
        doFx("starPoof", fxPos);
        Invoke("startInfiniteGame", 0.4f);

    }

    private void startInfiniteGame()
    {
        insertLevelAndStartNewGame(LevelLibrary.Instance.createInfiniteLevel());
    }

    public void btnFailedMenuReplayOnClick()
    {
        ActivateMenu(1);
        setOnResetGame();

        switch (level.mode)
        {
            case LevelIdentifier.GameMode.Infinite:

                insertLevelAndStartNewGame(LevelLibrary.Instance.createInfiniteLevel());

                break;
            case LevelIdentifier.GameMode.Classic:
            case LevelIdentifier.GameMode.Timed:
            case LevelIdentifier.GameMode.LightsOut:
            case LevelIdentifier.GameMode.Panic:

                replayCurrentLevel();

                break;

        }

    }
    public void btnSuccesMenuNextLevelOnClick()
    {

        if (level == null) return;
        endPlatform_Star.SetActive(true);
        BoxCollider collider = endPlatform_Star.GetComponent<BoxCollider>();
        collider.enabled = true;
         LevelIdentifier nextLevel = LevelLibrary.Instance.getNextLevel(level);
        if (nextLevel == null)
        {
            GuiController.Instance.showMessagePoup("Level " + (level.levelIndex + 1), Color.yellow, "Coming Soon...");
            return;
        }
        else
        {
            insertLevelAndStartNewGame(nextLevel);
        }



    }


    #endregion


    #region "Events"


    public void insertLevelAndStartNewGame(LevelIdentifier gameLevel)
    {
        level = gameLevel;
        setOnEnter();
    }

    public void replayCurrentLevel()
    {
        // Selecting Random Ball Color
        Material selectedMat = gameMaterials[UnityEngine.Random.Range(0, gameMaterials.Length)];
        changeBallMaterial(selectedMat);

        initLevel();
        setOnStart();
    }



    private void initLevel()
    {

        LevelLibrary.Instance.loadLevel(level);

        // Assign Cam Guide
        Transform camGuideHolder = level.gameObject.transform.Find("CameraGuide");
        if (camGuideHolder != null)
        {
            cameraFollower.camGuideHolder = camGuideHolder.gameObject;
            cameraFollower.initGuide();
        }

        setLevelLabel();

        // Move End Platfrom
        Transform endPosition = LevelLibrary.Instance.levelHolder.transform.Find("LevelEnd");
        if (endPosition != null)
        {
            endPlatform.transform.position = new Vector3(endPosition.transform.position.x, endPosition.transform.position.y - 33, endPosition.transform.position.z);
        }

    }


  
   

    public void setOnEnter()
    {

        if (level == null) return;

        //this.enabled = true;
        ActivateMenu(1);

        Material selectedMat = gameMaterials[UnityEngine.Random.Range(0, gameMaterials.Length)];
        changeBallMaterial(selectedMat);


        EventManager.Instance.onEnter.Invoke();
        currentGameState = GameState.OnGameEnter;

        stopAudio("AudioMusicMenu");
        startCutscene.Play();
        switch (level.mode)
        {
            case LevelIdentifier.GameMode.Infinite:


                GuiController.Instance.lblFailLevelName.text = "INFINITE RUN";
                GuiController.Instance.lblFailBestScore.gameObject.SetActive(true);
                GuiController.Instance.lblFailBestScore.text = Achievements.Instance.getInfiniteBestScore().ToString();

                InfiniteLevelController.Instance.init();


                break;
            case LevelIdentifier.GameMode.Classic:
            case LevelIdentifier.GameMode.Timed:
            case LevelIdentifier.GameMode.LightsOut:
            case LevelIdentifier.GameMode.Panic:

                initLevel();

                if (level != null)
                {

                    GuiController.Instance.lblFailLevelName.text = level.levelName.ToUpper();
                    GuiController.Instance.lblSuccessLevelName.text = level.levelName.ToUpper();
                    GuiController.Instance.lblFailBestScore.gameObject.SetActive(false);

                }
                else
                {
                    GuiController.Instance.sprNoTime.gameObject.SetActive(true);
                    GuiController.Instance.lblTime.gameObject.SetActive(false);
                    GuiController.Instance.lblFailLevelName.text = "NO LEVEL";
                    GuiController.Instance.lblSuccessLevelName.text = "NO LEVEL";
                    GuiController.Instance.lblFailBestScore.text = "0";
                    GuiController.Instance.lblHudStarTotal.text = "0";
                }


                break;
        }

        // Selecting Random Ball Color
     
    }
    public void StartCut()
    {
        startCutscene.Play();
    }
    IEnumerator EnablebaseTrig()
    {
        yield return new WaitForSeconds(1f);
        if (baseTrig) { baseTrig.enabled = true; }
    }
    public void setOnStart()
    {
        startPshycs();
        hitTheBall();
        StartCoroutine(EnablebaseTrig());
       // lifeAnimator.SetTrigger("show");
        
        cameraFollower.enabled = true;
        isGameRunning = true;
        StartCoroutine(AdmobAdmanager.Instance.ShowRewardedInterstitialStarting_Popup());
        EventManager.Instance.onStart.Invoke();
        currentGameState = GameState.OnGamePlaying;      
        // Logging Screen
        SocialServices.Instance.logAnalyticScreen("Game" + level.mode.ToString());
       
        switch (level.mode)
        {
            case LevelIdentifier.GameMode.Infinite:


                endPlatform.gameObject.SetActive(false);
                levelNameHolder.gameObject.SetActive(false);
                infiniteRunHolder.gameObject.SetActive(true);

                infiniteRunHolder.GetComponent<TweenScale>().PlayForward();

                break;
            case LevelIdentifier.GameMode.Classic:
            case LevelIdentifier.GameMode.Timed:
            case LevelIdentifier.GameMode.LightsOut:
            case LevelIdentifier.GameMode.Panic:

                endPlatform.gameObject.SetActive(true);
                levelNameHolder.gameObject.SetActive(true);
                infiniteRunHolder.gameObject.SetActive(false);

                levelNameHolder.GetComponent<TweenScale>().PlayForward();


                // Time
                if (level != null)
                {
                    if (level.levelData.levelTime <= 0)
                    {
                        GuiController.Instance.sprNoTime.gameObject.SetActive(true);
                        GuiController.Instance.lblTime.gameObject.SetActive(false);
                    }
                    else
                    {
                        GuiController.Instance.sprNoTime.gameObject.SetActive(false);
                        GuiController.Instance.lblTime.gameObject.SetActive(true);

                        startLevelTime();
                    }

                }

                break;
        }

        // Game Mode Controllers
        if (level.mode == LevelIdentifier.GameMode.LightsOut)
        {
            if (TestMode == false)
            {
                LightsOutController.Instance.setLightsOff(2.5f);
            } 
        }
        // Game Mode Controllers
        if (level.mode == LevelIdentifier.GameMode.Panic)
        {
            PanicController.Instance.panicLevel();
            PanicController.Instance.enablePanic(2.5f);
        }

        Achievements.Instance.updateGuiLabels();
       

    }



    public void setOnPause()
    {
        //lifeAnimator.SetTrigger("hide");

        //EventManager.Instance.onPause.Invoke();
        currentGameState = GameState.OnPaused;
        AudioListener.volume = 0;
                //this.enabled = false;
        StartCoroutine(ShowIntersitial(2f));

        stopPshycs();
        //Time.timeScale = 0.95f;
        isGameRunning = false;
       
    }

    public void setOnResume()
    {
        // lifeAnimator.SetTrigger("show");
        //EventManager.Instance.onResume.Invoke();
        startPshycs();
        ActivateMenu(1);
        currentGameState = GameState.OnGamePlaying;

        if (PlayerPrefs.GetInt("Mute", 0) == 0)
        {
            AudioListener.volume = 1;
        }


        //this.enabled = true;
        Time.timeScale = 1;
        isGameRunning = true;
      
    }

    public void setOnFailed()
    {
       // lifeAnimator.SetTrigger("hide");
        //EventManager.Instance.onFailed.Invoke();
        currentGameState = GameState.OnFailed;
        ActivateMenu(4);
        //this.enabled = false;
        stopLevelTime();
        StartCoroutine(ShowIntersitial(2f));
    }


    public void setOnSuccess()
    {
        //lifeAnimator.SetTrigger("hide");
        // Complete Level
        LevelLibrary.Instance.completeGameModeLevel(level.mode, level.levelKEY);

        // Unlock And Select Next Level
        LevelIdentifier nextLevel = LevelLibrary.Instance.getNextLevel(level);
        if (nextLevel != null)
        {
            LevelLibrary.Instance.unlockGameModeLevel(nextLevel.mode, nextLevel.levelKEY);
            LevelLibrary.Instance.setGameModeLastLevel(nextLevel.mode, nextLevel.levelKEY);
        }
        ActivateMenu(3);
        setOnResetGame();

        //EventManager.Instance.onSuccess.Invoke();
        currentGameState = GameState.OnSucceed;

        mainCamera.transform.position = new Vector3(0, -1000, 0);

        //this.enabled = false;
      StartCoroutine(ShowIntersitial(2f));
    }

    IEnumerator ShowIntersitial(float wait)
    {
        yield return new WaitForSeconds(wait);
        AdmobAdmanager.Instance.ShowInterstitial();
    }

    public void startPshycs()
    {
        ballCollider.enabled = true;
        ballRigid.isKinematic = false;
        ballRotator.DOPlay();
    }

    public void stopPshycs()
    {
        ballCollider.enabled = false;
        ballRigid.isKinematic = true;
        ballRotator.DOPause();
    }


    public void onClockStart(ElementIdentifier.ElementType elementType)
    {

        EventManager.Instance.onClockStart.Invoke();

        switch (elementType)
        {
            case ElementIdentifier.ElementType.Shield:
                ballShieldFx.gameObject.SetActive(true);

                break;
            case ElementIdentifier.ElementType.SlowClock:

                break;
        }

    }

    public void onClockEnd(ElementIdentifier.ElementType elementType)
    {

        EventManager.Instance.onClockOver.Invoke();

        switch (elementType)
        {
            case ElementIdentifier.ElementType.Shield:

                ballShieldFx.gameObject.SetActive(false);
                stopAudio("AudioShield");

                break;
            case ElementIdentifier.ElementType.SlowClock:

                changeObstaclesSpeed(1.0f);
                break;
        }

    }


    #endregion
    private void OnDisable()
    {
        this.enabled = true;
    }

}
