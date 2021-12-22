using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ColorChanger : MonoBehaviour {

    ////private Sprite sprite;
    public Color32[] colors;

    public int peak;
         
	// Use this for initialization
	void Start ()
    {
        //sprite = transform.GetComponent<Sprite>();
    }


    private bool switchTrig;
    private int counter;
    private int lastIndex;

	void Update ()
    {
        if (colors == null) return;
        if (colors.Length <= 0) return;


        if (counter >= peak)
        {
  
            switchTrig = true;
            counter = 0;

            if (lastIndex >= colors.Length)
            {
                lastIndex = 0;
            }            
            if (transform.GetComponent<Image>())
            {
                transform.GetComponent<Image>().color = colors[lastIndex];
            }
            if (transform.GetComponent<SpriteRenderer>())
            {
                transform.GetComponent<SpriteRenderer>().color = colors[lastIndex];
            }
            lastIndex++;

        }

       

        counter++;
    }
}
