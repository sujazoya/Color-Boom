using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropBomb : MonoBehaviour {


    public GameObject[] explodeObjects;
    public float expolisonDelay;

    public void Expolde()
    {

        if (explodeObjects == null) return;
        float delay = 0;

        for (int i = 0; i < explodeObjects.Length; i++)
        {
            GameController.Instance.explodeObject(explodeObjects[i], delay);
            delay += expolisonDelay;
        }

    }


    public void discoverObstacles()
    {

    
        List<GameObject> objectsToExplode = new List<GameObject>();


        List<GameObject> objectToDiscover = new List<GameObject>();

        if (GameController.Instance.level.mode == LevelIdentifier.GameMode.Infinite)
        {
            objectToDiscover = InfiniteLevelController.Instance.levelElements;
        }
        else
        {
            objectToDiscover = GameController.Instance.level.levelData.levelElements;
        }



        foreach (GameObject element in objectToDiscover)
        {

            ObstacleSet obstacleSet = element.GetComponent<ObstacleSet>();

            if (obstacleSet != null)
            {

                if (element.transform.position.y > transform.position.y && element.transform.position.y < (transform.position.y + (obstacleSet.height + 3.0f)))
                {
                    foreach (Transform obstacle in element.transform)
                    {
                   
                        ElementIdentifier identify = obstacle.GetComponent<ElementIdentifier>();
                        if (identify != null)
                        {
                            if (identify.elementType == ElementIdentifier.ElementType.Obstacle)
                            {
                                objectsToExplode.Add(obstacle.gameObject);
                            }

                            if (identify.elementType == ElementIdentifier.ElementType.ObstacleGroup)
                            {
                                objectsToExplode.Add(obstacle.gameObject);
                            }
                        }
                    }
                }
            }

        }

        if (objectsToExplode.Count > 0)
        {
            explodeObjects = objectsToExplode.ToArray();
        }


    }

}
