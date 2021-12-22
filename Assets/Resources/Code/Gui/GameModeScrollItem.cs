using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameModeScrollItem : MonoBehaviour {

    public static List<GameModeScrollItem> allGameModeItems;

    public GameObject fxHolder;


    private void Awake()
    {
        if (allGameModeItems == null)
        {
            allGameModeItems = new List<GameModeScrollItem>();
        }
    }

    private void Start()
    {
        allGameModeItems.Add(this);
    }

    public static void disableAllFx()
    {
        foreach (GameModeScrollItem item in allGameModeItems)
        {
            item.fxHolder.gameObject.SetActive(false);
        }

    }

}
