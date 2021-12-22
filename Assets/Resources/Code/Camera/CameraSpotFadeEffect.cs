using UnityEngine;
using System.Collections;


public class CameraSpotFadeEffect : MonoBehaviour {
	#region Variables
	public Shader SCShader;
	private float TimeX = 1.0f;
	private Vector4 ScreenResolution;
	private Material SCMaterial;
	public Vector2 center = new Vector2(0.5f,0.5f);
	public float Radius = 0.2f;

	public static Vector2 Changecenter;
	public static float ChangeRadius;
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
	void Start () 
	{

		Changecenter = center;
		ChangeRadius = Radius;
		SCShader = Shader.Find("Hidden/Spot");

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
			TimeX+=Time.deltaTime;
			if (TimeX>100)  TimeX=0;
		
			material.SetFloat("_TimeX", TimeX);
			material.SetFloat("_PositionX", center.x);
			material.SetFloat("_PositionY", center.y);
			material.SetFloat("_Radius", Radius);
			material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);	
		}
		
		
	}
		void OnValidate()
        {
		    Changecenter = center;
		    ChangeRadius = Radius;
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

    public IEnumerator SpotFadeEffect(FadeType fadeType, float duration)
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
                            Radius = 1.0f / duration * time;
                            break;
                        case FadeType.FadeOut:
                            Radius = 1.0f - (1.3f / duration * time);
                            break;
                    }

                }

                yield return new WaitForEndOfFrame();
            }

        this.enabled = false;
        yield break;
    }


    public void doSpotFadeInEffect(float duration)
    {
        if (worker != null) StopCoroutine(worker);
        this.enabled = true;
        Radius = 0;
        worker = StartCoroutine(SpotFadeEffect(FadeType.FadeIn, duration));
    }

    public void doSpotFadeOutEffect(float duration)
    {
        if (worker != null) StopCoroutine(worker);
        this.enabled = true;
        Radius = 1;
        worker =  StartCoroutine(SpotFadeEffect(FadeType.FadeOut, duration));
    }

}