using UnityEngine;
using System.Collections;

public class CameraGrayscaleEffect : MonoBehaviour
{

    public Shader shader;
    protected Material _material;
    private Coroutine worker;

    protected Material material
    {
        get
        {
            if (_material == null)
            {
                _material = new Material(shader);
                _material.hideFlags = HideFlags.HideAndDontSave;
            }

            return _material;
        }
    }

    public static bool IsLinear()
    {
        return QualitySettings.activeColorSpace == ColorSpace.Linear;
    }

    protected virtual void Start()
    {
        // Disable if we don't support image effects
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        // Disable the image effect if the shader can't
        // run on the users graphics card
        if (!shader || !shader.isSupported)
            enabled = false;
    }

    protected virtual void OnDisable()
    {
        if (_material)
            DestroyImmediate(_material);
    }

    // ------------------

    [Range(0f, 1f)]
    public float redLuminance = 0.299f;

    [Range(0f, 1f)]
    public float greenLuminance = 0.587f;

    [Range(0f, 1f)]
    public float blueLuminance = 0.114f;

    [Range(0f, 1f)]
    public float amount = 1.0f;


    protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (amount == 0f)
        {
            Graphics.Blit(source, destination);
            return;
        }

        material.SetVector("_Data", new Vector4(redLuminance, greenLuminance, blueLuminance, amount));
        Graphics.Blit(source, destination, material);
    }


    public enum FadeType
    {
        FadeIn = 0,
        FadeOut = 1,
    }

    private IEnumerator GrayScaleEffect(FadeType type, float duration)
    {

        this.enabled = true;
        float start = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup < start + duration)
        {

            float time = duration - ((start + duration) - Time.realtimeSinceStartup);

            if (time <= duration)
            {
                switch (type)
                {
                    case FadeType.FadeIn:
                        amount = 1.0f / duration * time;
                        break;
                    case FadeType.FadeOut:
                        amount = 1.0f - (1.0f / duration * time);
                        break;
                }
            }

            yield return new WaitForEndOfFrame();
        }


        yield break;

    }

    public void doGrayScaleInEffect(float duration)
    {
        if (worker != null) StopCoroutine(worker);
        amount = 0;
        worker = StartCoroutine(GrayScaleEffect(FadeType.FadeIn, duration));
    }

    public void doGrayScaleOutEffect(float duration)
    {
        if (worker != null) StopCoroutine(worker);
        amount = 1;
        worker = StartCoroutine(GrayScaleEffect(FadeType.FadeOut, duration));
    }



}
