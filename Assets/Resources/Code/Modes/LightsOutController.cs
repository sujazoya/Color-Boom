using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightsOutController : MonoBehaviour {

    // Singleton
    private static LightsOutController instance = null;
    public static LightsOutController Instance
    {
        get
        {
            return instance;
        }
    }

    public GameObject mainLight;
    public Material skyBoxMaterial;
    public GameObject ballSpotLight;

    public Material[] materialsToLight;
    

    private List<Color> defaults;

    private void Awake()
    {
        // Singleton

        instance = this;

        defaults = new List<Color>();

        foreach (Material item in materialsToLight)
        {
            defaults.Add(item.GetColor("_EmissionColor"));
        }


    }


    public void setLightsOff(float delay)
    {

        Invoke("doLightsOff", delay);
        
    }

    private void doLightsOff()
    {

        GameController.Instance.showXP("Lights", new Color32(255, 224, 53, 255), "Off", 2.5f);
        GameController.Instance.playAudio("AudioLightSwitch");

        mainLight.gameObject.SetActive(false);
        ballSpotLight.gameObject.SetActive(true);

        skyBoxMaterial.SetFloat("_Exposure", 0.8f);

        foreach (Material item in materialsToLight)
        {
            item.SetColor("_EmissionColor", Color.black);
        }
    }

    public void setLightsOn()
    {
        doLightsOn();
    }

    private void doLightsOn()
    {

        if (ballSpotLight != null)
        {
            ballSpotLight.gameObject.SetActive(false);
        }

        if (mainLight != null)
        {
            mainLight.gameObject.SetActive(true);
        }

        skyBoxMaterial.SetFloat("_Exposure", 1.87f);
        for (int i = 0; i < materialsToLight.Length; i++)
        {
            materialsToLight[i].SetColor("_EmissionColor", defaults[i]);
        }

    }

    private void OnDisable()
    {
        doLightsOn();
    }

}
