using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class PropMultiObstacle : MonoBehaviour {

    public enum PathDirection
    {
        Forward = 0,
        Backwards = 1
    }

    public GameObject obstacleMeshRef;

    public PathType pathType;
    public DG.Tweening.Ease easeType;
    public int loops;
    public LoopType loopType;
    public bool closedPath;
    public PathDirection direction;

    public int obstacleCount;
    public Vector3 obstacleSize;

    public float time;


    //public float delay;

    public Material[] materials;

    private Vector3[] path;

    void Start ()
    {

        Transform pathRoot = transform.Find("path");
        if (pathRoot == null) return;
        if (obstacleMeshRef == null) return;
        if (obstacleCount == 0) return;
        if (materials == null) return;

        if (obstacleSize.x <= 0 && obstacleSize.y <= 0 && obstacleSize.z <= 0)
            obstacleSize = new Vector3(1,1,1);

        // Creating Pth
        List<Transform> wayPoints = pathRoot.gameObject.GetComponentsInChildren<Transform>().ToList();
        if (wayPoints != null)
        {
            wayPoints = wayPoints.OrderBy(item => item.name).ToList();
        
            Vector3[] movePath = new Vector3[wayPoints.Count - 1];

            switch (direction)
            {
                case PathDirection.Forward:

                    for (int i = 0; i < wayPoints.Count - 1; i++)
                    {
                        movePath[i] = wayPoints[i].position;
                    }

                    break;
                case PathDirection.Backwards:

                    for (int i = 0; i < wayPoints.Count - 1; i++)
                    {
                        movePath[i] = wayPoints[(wayPoints.Count - 1) - i].position;
                    }


                    break;
                default:
                    break;
            }

        
            path = movePath;
        }


        // Creating Obstacles
        List<iTween> tweens = new List<iTween>();

        float curDelay = 0;

        for (int i = 0; i < obstacleCount; i++)
        {

            GameObject curObs = Instantiate(obstacleMeshRef);
            curObs.transform.parent = transform;
            curObs.name = "obs" + i;
            curObs.transform.localPosition = Vector3.zero;
            curObs.transform.rotation = Quaternion.identity;
            curObs.transform.localScale = obstacleSize;

            MeshRenderer renderer = curObs.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                int matIndex = (int)((float)materials.Length / (float)obstacleCount * (float)i);
                renderer.material = materials[matIndex];
            }

            Tween tween = curObs.transform.DOPath(path, time, pathType)
                    .SetEase(easeType)
                    .SetLoops(loops, loopType)
                    .SetDelay(curDelay)
                    .SetOptions(closedPath)
                    .SetLookAt(0.001f)
                    ;


            curDelay += (time / (float)obstacleCount);

        }


    }


    private IEnumerator startMove(iTween tween,float delay)
    {

        yield return new WaitForSeconds(delay);
        tween.enabled = true;

        yield break;

    }



    // Update is called once per frame
    void Update ()
    {
	
	}


    void OnDrawGizmos()
    {

        if (Application.isPlaying == false)
        {

            Transform pathRoot = transform.Find("path");
            if (pathRoot != null)
            {

                List<Transform> wayPoints = pathRoot.gameObject.GetComponentsInChildren<Transform>().ToList();

                if (wayPoints != null)
                {
                    wayPoints = wayPoints.OrderBy(item => item.name).ToList();

                    List<Vector3> movePath = new List<Vector3>();
                    Gizmos.color = Color.yellow;

                    for (int i = 0; i < wayPoints.Count - 1; i++)
                    {
                        movePath.Add(wayPoints[i].position);
                        Gizmos.DrawWireSphere(movePath[i], 0.5f);
                    }

                    if (closedPath == true)
                    {
                        movePath.Add(wayPoints[0].position);
                        Gizmos.DrawWireSphere(movePath[0], 0.5f);
                    }
                    
                    switch (pathType)
                    {
                        case PathType.Linear:
                            iTween.DrawLine(movePath.ToArray(), Color.yellow);
                            break;
                        case PathType.CatmullRom:
                            iTween.DrawPath(movePath.ToArray(), Color.yellow);
                            break;
                        default:
                            break;
                    }
               

                }


            }

        }
        else
        {
            switch (pathType)
            {
                case PathType.Linear:
                    //iTween.DrawLine(path, Color.yellow);
                    break;
                case PathType.CatmullRom:
                    //iTween.DrawPath(path, Color.yellow);
                    break;
                default:
                    break;
            }
        }

    }

}
