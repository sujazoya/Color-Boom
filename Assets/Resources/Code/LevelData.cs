using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelData : MonoBehaviour {

    [Serializable]
    public class LevelItem
    {
        public GameObject elementHolder;
        public float speed;
        public float offset;
        public float distance;
    }

    public float startPos;
    public float defSpeed;
    public float defDistance;

    public float levelTime;
    public Material ballStartMaterial;


    public List<LevelItem> items = new List<LevelItem>();

    [HideInInspector]
    public List<GameObject> levelElements;


    // Editor Helpers
    [HideInInspector]
    public ObstacleCategory randomElementCategory;
    [HideInInspector]
    public int randomElementCount;

    public enum ObstacleCategory
    {
        Simple = 0,
        Cat1 = 1,
        Cat2 = 2,
        Cat3 = 3
    }

    void Start ()
    {
	
	}

}
