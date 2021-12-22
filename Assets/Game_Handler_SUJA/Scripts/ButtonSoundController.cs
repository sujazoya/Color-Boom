using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundController : MonoBehaviour
{
    [SerializeField] string soundKey= "ButtonSound";
	[SerializeField] static string currentMusicKey;
	AudioSource myAudioSource;
    [SerializeField] AudioClip[] buttonClips;
    int clipIndex;
    Button[] sceneButtons;
	[SerializeField] Button[] musicButton;
	[SerializeField] GameObject[] offGameobject;
	// Start is called before the first frame update
	void Start()
    {
		currentMusicKey = soundKey;
		myAudioSource = gameObject.AddComponent<AudioSource>();
        sceneButtons = FindObjectsOfType<Button>();
        if (sceneButtons.Length > 0)
        {
            for (int i = 0; i < sceneButtons.Length; i++)
            {
                sceneButtons[i].onClick.AddListener(PlayButtonClip);
            }
        }
		if (musicButton.Length > 0)
		{
			for (int i = 0; i < musicButton.Length; i++)
			{
				musicButton[i].onClick.AddListener(ToggleMusic);
			}
		}
		if (offGameobject.Length > 0)
		{
			if (!PlayerPrefs.HasKey(currentMusicKey))
			{
				for (int i = 0; i < offGameobject.Length; i++)
				{
					offGameobject[i].SetActive(true);
				}

			}
			else
			{
				for (int i = 0; i < offGameobject.Length; i++)
				{
					offGameobject[i].SetActive(false);
				}
			}
		}
	}
    void PlayButtonClip()
    {
        if (!PlayerPrefs.HasKey(currentMusicKey))
        {
            clipIndex = Random.Range(0, buttonClips.Length);
            myAudioSource.clip = buttonClips[clipIndex];
            myAudioSource.Play();
        }
    }
	public void ToggleMusic()
	{
		if (!PlayerPrefs.HasKey(currentMusicKey))
		{
			PlayerPrefs.SetString(soundKey, soundKey);
			currentMusicKey = soundKey;
			myAudioSource.enabled = false;
			if (offGameobject.Length > 0)
			{
				for (int i = 0; i < offGameobject.Length; i++)
				{
					offGameobject[i].SetActive(true);

				}
			}

		}
		else
		{
			PlayerPrefs.DeleteKey(soundKey);
			currentMusicKey = string.Empty;
			myAudioSource.enabled = true;
			if (offGameobject.Length > 0)
			{
				for (int i = 0; i < offGameobject.Length; i++)
				{
					offGameobject[i].SetActive(false);
				}
			}
		}
	}
}
