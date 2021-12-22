using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLibrary : MonoBehaviour
{
    [HideInInspector]
    public List<LevelIdentifier> allLevels;

    public GameObject[] classicLevels;
    public GameObject[] timedLevels;
    public GameObject[] lightsOutLevels;
    public GameObject[] panicLevels;

    [HideInInspector]
    public GameObject levelHolder;


    // Singleton
    private static LevelLibrary instance = null;
    public static LevelLibrary Instance
    {
        get
        {
            return instance;
        }
    }

    // Use this for initialization
    void Awake()
    {

        // Singleton
        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        instance = this;
        allLevels = new List<LevelIdentifier>();

        // Initing Levels
        initLevels(classicLevels);
        initLevels(timedLevels);
        initLevels(lightsOutLevels);
        initLevels(panicLevels);


    }

    public void initLevels(GameObject[] levelHolders)
    {
        foreach (GameObject levelHolder in levelHolders)
        {
            if (levelHolder != null)
            {
                LevelIdentifier level = levelHolder.GetComponent<LevelIdentifier>();
                if (level != null)
                {
                    level.init();
                    allLevels.Add(level);
                }
            }
        }

    }

    public LevelIdentifier getLevel(string levelKEY)
    {

        for (int i = 0; i < allLevels.Count; i++)
        {
            if (allLevels[i].levelKEY == levelKEY)
            {
                return allLevels[i];
            }
        }

        return null;
    }

    public LevelIdentifier getNextLevel(LevelIdentifier currentLevel)
    {

        GameObject[] levels = null;

        switch (currentLevel.mode)
        {
            case LevelIdentifier.GameMode.None:
                break;
            case LevelIdentifier.GameMode.Infinite:
                break;
            case LevelIdentifier.GameMode.Classic:
                levels = classicLevels;
                break;
            case LevelIdentifier.GameMode.Timed:
                levels = timedLevels;
                break;
            case LevelIdentifier.GameMode.LightsOut:
                levels = lightsOutLevels;
                break;
            case LevelIdentifier.GameMode.Panic:
                levels = panicLevels;
                break;
        }


        if (levels != null)
        {

            int nextIndex = currentLevel.levelIndex + 1;
            for (int i = 0; i < levels.Length; i++)
            {
                LevelIdentifier level = levels[i].gameObject.GetComponent<LevelIdentifier>();
                if (level != null)
                {
                    if (level.levelIndex == nextIndex)
                    {
                        return level;
                    }
                }
            }
        }

        return null;
    }


    public void loadLevel(LevelIdentifier level)
    {


        if (levelHolder != null)
        {
            Destroy(levelHolder);
        }

        if (level == null) return;


        if (level != null)
        {

            level.transform.position = Vector3.zero;
            levelHolder = (GameObject)Instantiate(Resources.Load(level.path));

            loadLevelData(level);

        }


    }


    private void loadLevelData(LevelIdentifier level)
    {


        // Creating Elements
        if (level.levelData != null)
        {

            // Cam Guide
            GameObject cameraGuide;
 
            cameraGuide = new GameObject();
            cameraGuide.transform.parent = levelHolder.transform;
            cameraGuide.transform.position = Vector3.zero;
            cameraGuide.transform.rotation = Quaternion.identity;
            cameraGuide.name = "CameraGuide";
            addCameraGuidePoint(cameraGuide,new Vector3(0, 0, -level.levelData.defDistance));

            cameraGuide.AddComponent<LevelCamGuide>();

            GameController.Instance.cameraFollower.camGuideHolder = cameraGuide;
            GameController.Instance.cameraFollower.initGuide();


            level.levelData.levelElements = new List<GameObject>();
            float currentY = level.levelData.startPos;

            foreach (LevelData.LevelItem item in level.levelData.items)
            {

                if (item.elementHolder != null)
                {

                    GameObject newItemHolder = (GameObject)Instantiate(item.elementHolder, Vector3.zero, Quaternion.identity);
                    newItemHolder.transform.parent = levelHolder.transform;

                    // Cam Distance Set
                    float curDistance = 0;
                    if (item.distance <= 0)
                    {
                        curDistance = -level.levelData.defDistance;
                    }
                    else
                    {
                        curDistance = -item.distance;
                    }

                    // Obstacle Sets
                    ObstacleSet obstacleSet = newItemHolder.GetComponent<ObstacleSet>();
                    if (obstacleSet != null)
                    {

                        currentY += item.offset;

                        newItemHolder.transform.position = new Vector3(0, currentY, 0);

                        // Speed Set
                        if (level.mode != LevelIdentifier.GameMode.Panic)
                        {
                            if (item.speed > 0)
                            {
                                obstacleSet.changeObstacleTime(item.speed);
                            }
                            else
                            {
                                obstacleSet.changeObstacleTime(level.levelData.defSpeed);
                            }
                        }


                        // Adding cam guide point
                        addCameraGuidePoint(cameraGuide, new Vector3(0, currentY + (obstacleSet.height / 2.0f), curDistance));

                        currentY += obstacleSet.height + item.offset;

                    }



                    // Elements 
                    ElementIdentifier element = newItemHolder.GetComponent<ElementIdentifier>();
                    if (element != null)
                    {

                        switch (element.elementType)
                        {
                            case ElementIdentifier.ElementType.Obstacle:
                            case ElementIdentifier.ElementType.Star:
                            case ElementIdentifier.ElementType.ColorChanger:
                            case ElementIdentifier.ElementType.Bomb:
                            case ElementIdentifier.ElementType.Shield:
                            case ElementIdentifier.ElementType.Rocket:
                            case ElementIdentifier.ElementType.SlowClock:
                            case ElementIdentifier.ElementType.HourGlass:
                            case ElementIdentifier.ElementType.ObstacleGroup:

                                currentY += item.offset;

                                currentY += 5;
                                newItemHolder.transform.position = new Vector3(0, currentY, 0);

                                // Adding cam guide point
                                addCameraGuidePoint(cameraGuide, new Vector3(0, currentY, curDistance));

                                currentY += 5;

                                break;

                            case ElementIdentifier.ElementType.Decor:

                                newItemHolder.transform.position = new Vector3(0, currentY, 0);

                                break;
                        }

                    }


                    level.levelData.levelElements.Add(newItemHolder);

                }
 
            }

            // Level End
            currentY += 10;

            // Adding cam guide point
            addCameraGuidePoint(cameraGuide, new Vector3(0, currentY, -level.levelData.defDistance));

            GameObject levelEnd = new GameObject();
            levelEnd.name = "LevelEnd";
            levelEnd.transform.parent = levelHolder.transform;
            levelEnd.AddComponent<LevelEndPointer>();
            levelEnd.transform.position = new Vector3(0, currentY, 0);


        }

    }

    private void addCameraGuidePoint(GameObject camGuide, Vector3 pos)
    {
        GameObject newPoint = new GameObject();
        newPoint.transform.parent = camGuide.transform;
        newPoint.name = "P" + System.String.Format("{0:D4}", camGuide.transform.childCount + 1);
        newPoint.transform.position = pos;
    }

    private void addCameraGuidePointSet(GameObject camGuide, Vector3 elementPos, float cameraDistance, float height)
    {

        Vector3 startPoint = new Vector3(0, elementPos.y, -cameraDistance);
        Vector3 centerPoint = new Vector3(0, (elementPos.y + (height / 2.0f)), -cameraDistance);
        Vector3 endPoint = new Vector3(0, (elementPos.y + height), -cameraDistance);

        addCameraGuidePoint(camGuide,startPoint);
        addCameraGuidePoint(camGuide,centerPoint);
        addCameraGuidePoint(camGuide,endPoint);

        camGuide.GetComponent<LevelCamGuide>().calcPath();
    }


    public void completeGameModeLevel(LevelIdentifier.GameMode mode, string levelKEY)
    {
        PlayerPrefs.SetInt(levelKEY.ToString(), 1);
    }

    public void unlockGameModeLevel(LevelIdentifier.GameMode mode, string levelKEY)
    {
        if (PlayerPrefs.HasKey(levelKEY.ToString()) == false)
        {
            PlayerPrefs.SetInt(levelKEY.ToString(), 0);
        }
    }


    public string getGameModeLastLevel(LevelIdentifier.GameMode mode)
    {
        return PlayerPrefs.GetString(mode.ToString() + "_Lastlevel", "");
    }
    public void setGameModeLastLevel(LevelIdentifier.GameMode mode, string levelKEY)
    {
        PlayerPrefs.SetString(mode.ToString() + "_Lastlevel", levelKEY);
    }


   
    public LevelIdentifier createInfiniteLevel()
    {
        levelHolder = new GameObject();
        levelHolder.name = "InfiniteLevel";

        LevelIdentifier level = levelHolder.AddComponent<LevelIdentifier>();

        Guid g = Guid.NewGuid();
        string GuidString = Convert.ToBase64String(g.ToByteArray());
        GuidString = GuidString.Replace("=", "");
        GuidString = GuidString.Replace("+", "");

        level.levelName = GuidString;
        level.mode = LevelIdentifier.GameMode.Infinite;

        return level;

    }

    public void destroyCurrentLevel()
    {
        Destroy(levelHolder);
    }


}
