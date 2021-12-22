using UnityEngine;
using System.Collections;

public class ElementIdentifier : MonoBehaviour {


    public enum ElementType
    {
        None = 0,
        Base = 1,
        Obstacle = 2,
        Star = 3,
        ColorChanger = 4,
        LevelEnd = 5,
        Bomb = 6,
        Shield = 7,
        Rocket = 8,
        SlowClock = 9,
        HourGlass = 10,
        ObstacleGroup = 11,
        Decor = 12

    }

    public ElementType elementType;

    [HideInInspector]
    public bool recordObject;

}
