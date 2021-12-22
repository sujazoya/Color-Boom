using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelIdentifier : MonoBehaviour
{
    public enum GameMode
    {
        None = 0,
        Infinite = 1,
        Classic = 2,
        Timed = 3,
        LightsOut = 4,
        Panic = 5
    }



    [HideInInspector]
    public string levelKEY;

    public GameMode mode;
    public int levelIndex;

    [HideInInspector]
    public string levelName;

    [HideInInspector]
    public string path;

    [HideInInspector]
    public LevelData levelData;

    public void init()
    {

        levelKEY = mode.ToString() + "_Level" + levelIndex.ToString();
        levelName = "Level " + levelIndex.ToString();

        levelData = gameObject.GetComponent<LevelData>();

        switch (mode)
        {
            case GameMode.None:
                break;
            case GameMode.Infinite:
                // No Levels
                break;
            case GameMode.Classic:
                path = "Prefabs/Levels/Classic/" + mode.ToString() + "_Level" + levelIndex.ToString();
                break;
            case GameMode.Timed:
                path = "Prefabs/Levels/Timed/" + mode.ToString() + "_Level" + levelIndex.ToString();
                break;
            case GameMode.LightsOut:
                path = "Prefabs/Levels/LightsOut/" + mode.ToString() + "_Level" + levelIndex.ToString();
                break;
            case GameMode.Panic:
                path = "Prefabs/Levels/Panic/" + mode.ToString() + "_Level" + levelIndex.ToString();
                break;
        }

    }




}
