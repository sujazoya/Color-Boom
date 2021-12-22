using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class InfiniteLevelController : MonoBehaviour {


    // Singleton
    private static InfiniteLevelController instance = null;
    public static InfiniteLevelController Instance
    {
        get
        {
            return instance;
        }
    }

    [HideInInspector]
    public GameObject infiniteLevelRoot;

    private GameObject cameraGuide;

    public GameObject[] obstacleSetRefs;
    public GameObject[] simpleObstacleSetRefs;

    public GameObject colorChangerRef;
    public GameObject starRef;

    public GameObject[] powerupRefs;

    [HideInInspector]
    public List<GameObject> levelElements;

    public float startY;
    private float currentY;

    private GameObject currentObstacleSet;

   
    public float fillDistance;
    public float colorChangerGap;

    private int placedObstacleCount;
    private int placedStarCount;

    private List<float> timeSpeeds;

    private bool highScoreTrig;

    void Awake()
    {

        // Singleton
        //if (instance != null && instance != this)
        //{
        //    Destroy(this.gameObject);
        //}
        instance = this;

        timeSpeeds = new List<float>();

        timeSpeeds.Add(4.0f);

    }

    public void reset()
    {

        if (infiniteLevelRoot != null)
        {
            Destroy(infiniteLevelRoot);
        }

        currentY = startY;
        placedObstacleCount = 0;
        placedStarCount = 0;
        cameraGuidePointCounter = 0;
        highScoreTrig = false;

        levelElements = new List<GameObject>();

    }

    public void init()
    {

        reset();

        infiniteLevelRoot = new GameObject();
        infiniteLevelRoot.transform.position = Vector3.zero;
        infiniteLevelRoot.transform.rotation = Quaternion.identity;
        infiniteLevelRoot.name = "InfiniteLevelRoot";

        cameraGuide = new GameObject();
        cameraGuide.transform.parent = infiniteLevelRoot.transform;
        cameraGuide.transform.position = Vector3.zero;
        cameraGuide.transform.rotation = Quaternion.identity;
        cameraGuide.name = "CameraGuide";
        addPoint(new Vector3(0, 0, -GameController.Instance.cameraFollower.distance));

        cameraGuide.AddComponent<LevelCamGuide>();

        //GameController.Instance.cameraFollower.camGuideHolder = cameraGuide;
        //GameController.Instance.cameraFollower.initGuide();

        nextPowerup = Random.Range(8, 15);
        StartCoroutine(createElementsNext());

    }


    private void FixedUpdate()
    {

        if (GameController.Instance == null) return;
        if (GameController.Instance.level == null) return;

        if (GameController.Instance.level.mode == LevelIdentifier.GameMode.Infinite && GameController.Instance.isGameRunning == true)
        {
            StartCoroutine(createElementsNext());
        }

    }

    public IEnumerator createElementsNext()
    {

        float forwardTargetDistance = GameController.Instance.ball.transform.position.y + fillDistance;

        if (currentY < forwardTargetDistance)
        {
            StartCoroutine(clearElementsBehind());
        }

        while (currentY < forwardTargetDistance)
        {

            if (highScoreTrig == false && placedStarCount > Achievements.Instance.getInfiniteBestScore())
            {
                highScoreTrig = true;

                currentY += 10.0f;

                GameController.Instance.highScorePlatform.gameObject.SetActive(true);
                GameController.Instance.highScorePlatform.transform.position = new Vector3(0, currentY - 33.0f, 0);

                GameObject nweStar = Instantiate(starRef);
                nweStar.gameObject.GetComponent<ElementIdentifier>().recordObject = true;

                nweStar.transform.parent = infiniteLevelRoot.transform;
                nweStar.name = "RecordStar";
                nweStar.transform.position = new Vector3(0, currentY + 17, 0);

                levelElements.Add(nweStar);

                placedObstacleCount++;
                placedStarCount++;

                currentY += 40.0f;

            }
            else
            {
                // Color Changer
                if (currentY > startY)
                {

                    if (hazPowerUp() == false)
                    {
                        currentY += colorChangerGap;

                        GameObject newColorChangerHolder = Instantiate(colorChangerRef);
                   
                        newColorChangerHolder.transform.parent = infiniteLevelRoot.transform;
                        newColorChangerHolder.transform.position = new Vector3(0, currentY, 0);

                        levelElements.Add(newColorChangerHolder);

                        currentY += colorChangerGap;

                    }
                    else
                    {

                        currentY += 5;

                        GameObject newPowerup = Instantiate(getNextPowerup());
                        newPowerup.transform.parent = infiniteLevelRoot.transform;
                        newPowerup.transform.position = new Vector3(0, currentY, 0);

                        levelElements.Add(newPowerup);


                        addCameraGuidePoints(new Vector3(0, currentY - 5, 0), 30, 10);

                        nextPowerup = Random.Range(placedObstacleCount + 5, placedObstacleCount + 10);

                        switch (newPowerup.GetComponent<ElementIdentifier>().elementType)
                        {
                            case ElementIdentifier.ElementType.Bomb:

                                PropBomb bomb = newPowerup.GetComponent<PropBomb>();
                                if (bomb != null)
                                {
                                    bomb.expolisonDelay = 0.2f;
                                }

                                lastPowerupType = ElementIdentifier.ElementType.Bomb;

                                break;
                            case ElementIdentifier.ElementType.Shield:

                                PropTimer timer = newPowerup.GetComponent<PropTimer>();
                                if (timer != null)
                                {
                                    timer.duration = Random.Range(5.0f, 10.0f);
                                }

                                lastPowerupType = ElementIdentifier.ElementType.Shield;

                                break;
                            case ElementIdentifier.ElementType.Rocket:

                                PropRocket rocket = newPowerup.GetComponent<PropRocket>();
                                if (rocket != null)
                                {
                                    rocket.distance = Random.Range(40.0f, 50.0f);
                                    rocket.time = 1.0f;
                                }


                                lastPowerupType = ElementIdentifier.ElementType.Rocket;

                                break;
                        }


                        currentY += 5;

                    }

                }

                // Obstacle
                GameObject newObstacleSetHolder = null;

                if (placedObstacleCount < 3)
                {
                    newObstacleSetHolder = Instantiate(getNextObstacleSet(simpleObstacleSetRefs));
                }
                else
                {
                    newObstacleSetHolder = Instantiate(getNextObstacleSet(obstacleSetRefs));
                }


                ObstacleSet obstacleSet = newObstacleSetHolder.GetComponent<ObstacleSet>();

                newObstacleSetHolder.transform.parent = infiniteLevelRoot.transform;
                newObstacleSetHolder.transform.position = new Vector3(0, currentY, 0);

                currentObstacleSet = newObstacleSetHolder;
                levelElements.Add(newObstacleSetHolder);

                obstacleSet.changeObstacleTime(obstacleSet.speeds[getCurrentObstacleSpeed()]);

                countStars(newObstacleSetHolder);

                addCameraGuidePoints(newObstacleSetHolder.transform.position, obstacleSet.cameraDistance, obstacleSet.height);

                placedObstacleCount++;

                currentY += obstacleSet.height;

            }


        }

        yield break;

    }

    private IEnumerator clearElementsBehind()
    {

        List<int> itemsForDelete = new List<int>();

        for (int i = 0; i < levelElements.Count; i++)
        {

            if (GameController.Instance.ball.transform.position.y > levelElements[i].gameObject.transform.position.y + 80.0f)
            {
                itemsForDelete.Add(i);
            }

        }

        // Destroying Injected Tracks
        for (int i = 0; i < itemsForDelete.Count; i++)
        {
            Destroy(levelElements[itemsForDelete[i]].gameObject);
            levelElements.RemoveAt(itemsForDelete[i]);
        }

        yield break;
    }


    private GameObject getNextObstacleSet(GameObject[] obstacleCollection)
    {
        if (obstacleCollection == null) return null;

        if (currentObstacleSet == null) return obstacleCollection[UnityEngine.Random.Range(0, obstacleCollection.Length)];

        List<GameObject> newlist = new List<GameObject>();

        for (int i = 0; i < obstacleCollection.Length; i++)
        {
            if (!obstacleCollection[i].gameObject.name.Replace("(Clone)", "").ToLower().Equals(currentObstacleSet.name.ToLower()))
            {
                newlist.Add(obstacleCollection[i]);
            }
        }

        if (newlist.Count <= 0)
        {
            return obstacleCollection[UnityEngine.Random.Range(0, obstacleCollection.Length)];
        }

        return newlist[UnityEngine.Random.Range(0, newlist.Count)];
    }




    private GameObject getNextPowerup()
    {

        if (lastPowerupType == ElementIdentifier.ElementType.None) return powerupRefs[UnityEngine.Random.Range(0, powerupRefs.Length)];

        List<GameObject> newlist = new List<GameObject>();

        for (int i = 0; i < powerupRefs.Length; i++)
        {
            if (powerupRefs[i].gameObject.GetComponent<ElementIdentifier>().elementType != lastPowerupType)
            {
                newlist.Add(powerupRefs[i]);
            }
        }

        if (newlist.Count <= 0)
        {
            return powerupRefs[UnityEngine.Random.Range(0, powerupRefs.Length)];
        }

        return newlist[UnityEngine.Random.Range(0, newlist.Count)];

    }

    private int nextPowerup;
    private ElementIdentifier.ElementType lastPowerupType;

    private bool hazPowerUp()
    {

        if (placedObstacleCount >= nextPowerup)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    int cameraGuidePointCounter;
    private void addCameraGuidePoints(Vector3 elementPos, float cameraDistance, float height)
    {

        Vector3 startPoint = new Vector3(0, elementPos.y, -cameraDistance);
        Vector3 centerPoint = new Vector3(0, (elementPos.y + (height / 2.0f)), -cameraDistance + -1.0f);
        Vector3 endPoint = new Vector3(0, (elementPos.y + height), -cameraDistance);

        addPoint(startPoint);
        addPoint(centerPoint);
        addPoint(endPoint);

        cameraGuide.GetComponent<LevelCamGuide>().calcPath();
    }

    private void addPoint(Vector3 pos)
    {
        GameObject newPoint = new GameObject();
        newPoint.transform.parent = cameraGuide.transform;
        newPoint.name = "P" + System.String.Format("{0:D4}", cameraGuidePointCounter);
        newPoint.transform.position = pos;

        cameraGuidePointCounter++;
    }

 

    public void refreshAllTriangles()
    {
        foreach (GameObject obsSet in levelElements)
        {
            PropTriangleObstacle[] triangles = obsSet.transform.GetComponentsInChildren<PropTriangleObstacle>();

            foreach (PropTriangleObstacle tri in triangles)
            {
                tri.setTriangleColors();
            }

        }

    }

    private void countStars(GameObject obstacleSet)
    {

        ElementIdentifier[] objections = obstacleSet.transform.GetComponentsInChildren<ElementIdentifier>();

        foreach (ElementIdentifier element in objections)
        {
            if (element.elementType == ElementIdentifier.ElementType.Star)
            {
                placedStarCount++;
            }
        }

    }

    private int getCurrentObstacleSpeed()
    {

        //int result = 2;

        //if (GameController.Instance.ball.transform.position.y > 0 && GameController.Instance.ball.transform.position.y < 300)
        //{
        //    result = 2;
        //}

        //if (GameController.Instance.ball.transform.position.y > 300 && GameController.Instance.ball.transform.position.y < 600)
        //{
        //    result = 1;
        //}

        //if (GameController.Instance.ball.transform.position.y > 600)
        //{
        //    result = 0;
        //}

        return 2;
    }

}
