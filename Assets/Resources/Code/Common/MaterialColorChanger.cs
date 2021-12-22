using UnityEngine;
using System.Collections;

public class MaterialColorChanger : MonoBehaviour {

    private MeshRenderer meshRenderer;
    public Material[] materials;

    public int peak;

    private bool switchTrig;
    private int counter;
    private int lastIndex;

    void Start()
    {
        meshRenderer = transform.GetComponent<MeshRenderer>();
    }

    void Update()
    {

        if (meshRenderer == null) return;
        if (materials == null) return;
        if (materials.Length <= 0) return;


        if (counter >= peak)
        {

            switchTrig = true;
            counter = 0;

            if (lastIndex >= materials.Length)
            {
                lastIndex = 0;
            }

            meshRenderer.material = materials[lastIndex];

            lastIndex++;

        }



        counter++;
    }
}
