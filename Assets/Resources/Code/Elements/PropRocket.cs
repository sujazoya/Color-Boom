using UnityEngine;
using System.Collections;

public class PropRocket : MonoBehaviour {

    public float distance;
    public float time;


    void OnDrawGizmos()
    {

        Vector3[] path = new Vector3[2];

        path[0] = transform.position;
        path[1] = new Vector3(transform.position.x, transform.position.y + distance, transform.position.z);


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(path[0], 0.5f);
        Gizmos.DrawWireSphere(path[1], 0.5f);

        iTween.DrawLine(path, Color.white);

    }

}
