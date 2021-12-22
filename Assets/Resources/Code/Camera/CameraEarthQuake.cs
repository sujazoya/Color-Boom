using UnityEngine;
using System.Collections;

public class CameraEarthQuake : MonoBehaviour
{

    #region Variables
    public Shader SCShader;
    private float TimeX = 1.0f;
    private Vector4 ScreenResolution;
    private Material SCMaterial;
    [Range(0f, 100f)]
    public float Speed = 15f;
    [Range(0f, 0.2f)]
    public float X = 0.008f;
    [Range(0f, 0.2f)]
    public float Y = 0.008f;
    [Range(0f, 0.2f)]
    private float Value4 = 1f;
    public static float ChangeValue;
    public static float ChangeValue2;
    public static float ChangeValue3;
    public static float ChangeValue4;
    #endregion

    private bool activated;
    private Coroutine worker;

    #region Properties
    Material material
    {
        get
        {
            if (SCMaterial == null)
            {
                SCMaterial = new Material(SCShader);
                SCMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return SCMaterial;
        }
    }
    #endregion


    void Start()
    {
        ChangeValue = Speed;
        ChangeValue2 = X;
        ChangeValue3 = Y;
        ChangeValue4 = Value4;
        SCShader = Shader.Find("Game/EarthQuake");
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (activated == true)
        {
            if (SCShader != null)
            {
                TimeX += Time.deltaTime;
                if (TimeX > 100) TimeX = 0;
                material.SetFloat("_TimeX", TimeX);
                material.SetFloat("_Value", Speed);
                material.SetFloat("_Value2", X);
                material.SetFloat("_Value3", Y);
                material.SetFloat("_Value4", Value4);
                material.SetVector("_ScreenResolution", new Vector4(sourceTexture.width, sourceTexture.height, 0.0f, 0.0f));
                Graphics.Blit(sourceTexture, destTexture, material);
            }
            else
            {
                Graphics.Blit(sourceTexture, destTexture);
            }
        }
    }

    void OnValidate()
    {
        if (activated == true)
        {
            ChangeValue = Speed;
        ChangeValue2 = X;
        ChangeValue3 = Y;
        ChangeValue4 = Value4;
        }
    }

    void Update()
    {
        if (Application.isPlaying && activated == true)
        {
            Speed = ChangeValue;
            X = ChangeValue2;
            Y = ChangeValue3;
            Value4 = ChangeValue4;
        }
    }
    void OnDisable()
    {
        if (SCMaterial)
        {
            DestroyImmediate(SCMaterial);
        }
    }


    public void Shake(float duration)
    {
        this.enabled = true;
        if (activated == true) return;
        activated = true;
        worker = StartCoroutine(doShake(duration));

    }

    private IEnumerator doShake(float duration)
    {
   

        float elapsedTime = 0;

        while (elapsedTime <= duration)
        {

            if (Time.timeScale > 0)
            {
                elapsedTime += Time.deltaTime;
            }

            yield return new WaitForEndOfFrame();
        }

        activated = false;
        this.enabled = false;
        yield break;

    }


}
