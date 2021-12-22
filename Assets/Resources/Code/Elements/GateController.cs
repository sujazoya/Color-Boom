using UnityEngine;
using System.Collections;

public class GateController : MonoBehaviour {


    public GameObject doorLeft;
    public GameObject doorRight;
    public AudioSource doorAudio;

    private TweenPosition leftDoorAnim;
    private TweenPosition rightDoorAnim;

    void Start()
    {

        if (doorLeft != null)
        {
            leftDoorAnim = doorLeft.GetComponent<TweenPosition>();
        }

        if (doorRight != null)
        {
            rightDoorAnim = doorRight.GetComponent<TweenPosition>();
        }

    }

    public void OnTriggerEnter(Collider collider)
    {

        if (collider.name.Equals("Ball"))
        {
            if (leftDoorAnim != null)
            {
                leftDoorAnim.PlayForward();
            }

            if (rightDoorAnim != null)
            {
                rightDoorAnim.PlayForward();
            }

            if (doorAudio != null)
            {
                doorAudio.Play();
            }

        }

    }

    public void OnTriggerExit(Collider collider)
    {

        if (collider.name.Equals("Ball"))
        {
            if (leftDoorAnim != null)
            {
                leftDoorAnim.PlayReverse();
            }

            if (rightDoorAnim != null)
            {
                rightDoorAnim.PlayReverse();
            }


            if (doorAudio != null)
            {
                doorAudio.Play();
            }

        }

     
    }


}
