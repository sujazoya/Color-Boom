using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelCamGuide : MonoBehaviour
{

    [HideInInspector]
    public Vector3[] path;
    private List<Transform> wayPoints;

   [HideInInspector]
    public float pathLenght;


    private void Start()
    {
        calcPath();
    }

    public void calcPath()
    {

        List<Vector3> wpPath = new List<Vector3>();
        wayPoints = GetComponentsInChildren<Transform>().ToList();

        if (wayPoints != null)
        {

            for (int i = 0; i < wayPoints.Count; i++)
            {
                wpPath.Add(wayPoints[i].position);
            }
        }


        path = iTween.PathControlPointGenerator(wpPath.ToArray());
        pathLenght = iTween.PathLength(path);

        //Debug.Log("pathLenght:" + pathLenght);
    }

    public float getPathPercentByY(float Y)
    {
        return 1.0f / pathLenght * Y;
    }


    public float getZPositionByY(float Y)
    {


        Vector3 curPos = iTween.Interp(path, getPathPercentByY(Y));

        return curPos.z;
    }

    void OnDrawGizmos()
    {

        if (Application.isPlaying == false)
        {

            List<Transform> gizmoWayPoints = GetComponentsInChildren<Transform>().ToList();

            if (gizmoWayPoints != null)
            {
                gizmoWayPoints = gizmoWayPoints.OrderBy(item => item.name).ToList();

                List<Vector3> movePath = new List<Vector3>();
                Gizmos.color = Color.yellow;

                for (int i = 0; i < gizmoWayPoints.Count; i++)
                {
                    movePath.Add(gizmoWayPoints[i].position);
                    Gizmos.DrawWireSphere(movePath[i], 0.5f);
                }

                iTween.DrawPath(movePath.ToArray(), Color.white);

            }



        }
        else
        {
            iTween.DrawPath(path, Color.white);
            Gizmos.color = Color.yellow;
            for (int i = 0; i < wayPoints.Count; i++)
            {
                Gizmos.DrawWireSphere(wayPoints[i].transform.position, 0.5f);
            }

        }

    }


}
