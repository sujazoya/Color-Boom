using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
  

    // The target we are following
    public Transform target;
    // The distance in the x-z plane to the target
    public float distance = 10.0f;
    private float defDistance;
    // the height we want the camera to be above the target
    public float height = 5.0f;
    private float defHeight;
    // How much we 
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;

    private float lastY;

    public GameObject camGuideHolder;
    private LevelCamGuide camGuide;

    //private GameObject cube;

    public void reset()
    {
        distance = defDistance;
        height = defHeight;
        lastY = 0;
    }

    public void initGuide()
    {
        if (camGuideHolder != null)
        {
            camGuide = camGuideHolder.GetComponent<LevelCamGuide>();
        }
    }


    void Awake()
    {

        defDistance = distance;
        defHeight = height;
    }

    void Start()
    {

 

        //cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {


            // Calculate the current rotation angles
            float wantedRotationAngle = target.eulerAngles.y;
            float wantedHeight = target.position.y + height;

            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

    
            // Damp the rotation around the y-axis
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

            // Damp the height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Convert the angle into a rotation
            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            Vector3 pos = target.position;
            pos -= currentRotation * Vector3.forward * distance;


            if (currentHeight > lastY)
            {
                lastY = currentHeight;
                moveCameraAlongGuide();
            }
            else if (currentHeight < lastY && GameController.Instance.ballRigid.velocity.y < - 15.0f)
            {
                lastY = currentHeight;
                moveCameraAlongGuide();
            }

            pos.y = lastY;

            transform.position = pos;
   
            // Always look at the target

        }

    }


    private void moveCameraAlongGuide()
    {
        if (camGuide != null)
        {
            if (camGuide.pathLenght > 0)
            {

                if (camGuide.getPathPercentByY(lastY) <= 1.0f)
                {

                    float percent = 0;
                    float pathLen = camGuide.pathLenght * 50;
                    Vector3 curPos = Vector3.zero;

                    for (int i = 0; i < pathLen; i++)
                    {

                        curPos = iTween.Interp(camGuide.path, 1.0f / (pathLen) * (float)i);

                        if (curPos.y >= lastY)
                        {
                            percent = 1.0f / (pathLen) * (float)i;
                            //cube.transform.position = curPos;
                            break;
                        }

                    }

                    distance = -curPos.z;
                    //Debug.Log(cube.transform.position.z);

                }
                else
                {
                    distance = defDistance;

                }

            }

        }

    }

}
