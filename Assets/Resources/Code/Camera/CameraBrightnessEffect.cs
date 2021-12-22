using UnityEngine;
using System.Collections;

public class CameraBrightnessEffect : MonoBehaviour
{
    // Singleton
    private static CameraBrightnessEffect instance = null;
    public static CameraBrightnessEffect Instance
    {
        get
        {
            return instance;
        }
    }

    #region Variables
    public Shader SCShader;
	[Range(0, 2)]
	public float _Brightness = 1.5f;
	private Material SCMaterial;

	public static float ChangeBrightness;

    private Coroutine worker;

	#endregion
	
	#region Properties
	Material material
	{
		get
		{
			if(SCMaterial == null)
			{
				SCMaterial = new Material(SCShader);
				SCMaterial.hideFlags = HideFlags.HideAndDontSave;	
			}
			return SCMaterial;
		}
	}
    #endregion

    void Awake()
    {
        // Singleton
        instance = this;
    }

    void Start () 
	{
		ChangeBrightness = _Brightness;
		SCShader = Shader.Find("Hidden/Brightness");

		if(!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
	}
	
	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if(SCShader != null)
		{
				material.SetFloat("_Val", _Brightness);

			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);	
		}
		
		
	}
    void OnValidate()
    {
	    ChangeBrightness=_Brightness;
    }


	void OnDisable ()
	{
		if(SCMaterial)
		{
			DestroyImmediate(SCMaterial);	
		}
		
	}



    public enum FadeType
    {
        FadeIn = 0,
        FadeOut = 1,
    }


    public enum BrighnessFadeColor
    {
        Black = 0,
        White = 1
    }

    public IEnumerator BrightnessFadeEffect(FadeType fadeType, BrighnessFadeColor color, float duration)
    {

            enabled = true;
            float start = Time.realtimeSinceStartup;

            while (Time.realtimeSinceStartup < start + duration)
            {

                float time = duration - ((start + duration) - Time.realtimeSinceStartup);

                if (time <= duration)
                {
                    switch (fadeType)
                    {
                        case FadeType.FadeIn:

                            if (color == BrighnessFadeColor.Black)
                            {
                                _Brightness = 1.0f / duration * time;
                            }
                            else if (color == BrighnessFadeColor.White)
                            {
                                _Brightness = 1.0f + (1.0f / duration * time);
                            }



                            break;
                        case FadeType.FadeOut:

                            if (color == BrighnessFadeColor.Black)
                            {
                                _Brightness = 1.0f - (1.0f / duration * time);
                            }
                            else if (color == BrighnessFadeColor.White)
                            {
                                _Brightness = 2.0f - (1.0f / duration * time);
                            }

                            break;
                    }

                }

                yield return new WaitForEndOfFrame();
            }

        this.enabled = false;
        yield break;
    }


    public void doBlackBrightnessIn()
    {
        if (worker != null) StopCoroutine(worker);
        this.enabled = true;
        _Brightness = 1;
        worker =  StartCoroutine(BrightnessFadeEffect(FadeType.FadeIn, BrighnessFadeColor.Black, 1.0f));
    }

    public void doBlackBrightnessOut()
    {
        if (worker != null) StopCoroutine(worker);
        this.enabled = true;
        _Brightness = 0;
        worker = StartCoroutine(BrightnessFadeEffect(FadeType.FadeOut, BrighnessFadeColor.Black, 1.0f));
    }

    public void doWhiteBrightnessIn()
    {
        if (worker != null) StopCoroutine(worker);
        this.enabled = true;
        _Brightness = 1;
        worker = StartCoroutine(BrightnessFadeEffect(FadeType.FadeIn, BrighnessFadeColor.White, 1.0f));
    }

    public void doWhiteBrightnessOut(float duration)
    {
        if (worker != null) StopCoroutine(worker);
        this.enabled = true;
        _Brightness = 2;
        worker = StartCoroutine(BrightnessFadeEffect(FadeType.FadeOut, BrighnessFadeColor.White, 1.0f));
    }


}