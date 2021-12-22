using UnityEngine;
using System.Collections;

public class LevelEndPointer : MonoBehaviour {


    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(45, 2, 45));

    }

}
