using UnityEngine;
using System.Collections;

public class BulbSwitch : MonoBehaviour {

    public Material lightOff;
    public Material lightOn;
    public SpriteRenderer glow;
    public TweenScale scaleFX;

    private MeshRenderer mRenderer;

    private bool isOn;
    public int interval;
    private int counter;

    void Start()
    {
        mRenderer = gameObject.GetComponent<MeshRenderer>();
    }


	void Update ()
    {

        if (GameController.Instance == null) return;
        if (GameController.Instance.currentGameState !=  GameController.GameState.OnMainMenu) return;

        if (counter >= interval)
        {
            scaleFX.HardResetToBeginning();
            scaleFX.PlayForward();

            if (isOn == false)
            {
                mRenderer.material = lightOn;
                isOn = true;
                counter = 0;
                glow.color = Color.white;
                return;
            }
            else if (isOn == true)
            {
                mRenderer.material = lightOff;
                isOn = false;
                counter = 0;
                glow.color = Color.black;
                return;
            }
  
        }

        counter++;

    }
}
