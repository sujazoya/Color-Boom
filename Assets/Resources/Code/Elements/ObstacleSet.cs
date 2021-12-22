using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class ObstacleSet : MonoBehaviour {


    public float height;
    public float cameraDistance;

  
    public float[] speeds;


    void OnDrawGizmos()
    {

        if (Application.isPlaying == true)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(15, 1, 5));
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y + height, transform.position.z), new Vector3(15, 1, 5));
        
    }

    public void changeObstacleTime(float time)
    {

        // Updating MultiObstacles
        PropMultiObstacle[] multiObstacles = transform.GetComponentsInChildren<PropMultiObstacle>();

        foreach (PropMultiObstacle multiObstacle in multiObstacles)
        {
            multiObstacle.time = time;
        }


        // Updating DoTweens
        List<DOTweenAnimation> tweens = new List<DOTweenAnimation>();

        foreach (Transform obstacle in transform)
        {
            DOTweenAnimation[] curTwens = obstacle.transform.GetComponents<DOTweenAnimation>();
            for (int i = 0; i < curTwens.Length; i++)
            {
                tweens.Add(curTwens[i]);
            }

        }
        foreach (DOTweenAnimation tweenAnim in tweens)
        {

            switch (tweenAnim.animationType)
            {
                case DG.Tweening.Core.DOTweenAnimationType.Rotate:
                    Tweener tweenRotate = tweenAnim.gameObject.transform.DORotate(tweenAnim.endValueV3, time, tweenAnim.optionalRotationMode).SetLoops(tweenAnim.loops, tweenAnim.loopType).SetEase(tweenAnim.easeType);
                    break;
                case DG.Tweening.Core.DOTweenAnimationType.LocalMove:
                    Tweener tweenMove = tweenAnim.gameObject.transform.DOLocalMove(tweenAnim.endValueV3, time).SetLoops(tweenAnim.loops, tweenAnim.loopType).SetEase(tweenAnim.easeType);
                    break;

            }

        }

    }


}
