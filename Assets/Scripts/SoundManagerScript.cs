using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AudioClipID
{
	BGM_MAINMENUINTRO = 0,
	BGM_MAINMENU = 1,
	BGM_LEVELINTRO = 2,
	BGM_LEVEL = 3,

	//UI
	SFX_UI_BUTTON = 4,
	SFX_CRY00 = 5,
	SFX_CRY01 = 6,
	SFX_CRY02 = 7,
	SFX_CRY03 = 8,


	SFX_BATTLE_TRANSACTION = 350,

	TOTAL = 9001
}

[System.Serializable]
public class AudioClipInfo
{
	public AudioClipID audioClipID;
	public AudioClip audioClip;
}

public class SoundManagerScript : MonoBehaviour 
{
	#region Singleton
	public static SoundManagerScript Instance;


	#endregion Singleton

	public float bgmVolume = 1.0f;
	public float sfxVolume = 1.0f;
	public float brightness = 1.0f;


	public List<AudioClipInfo> audioClipInfoList = new List<AudioClipInfo>();

	public AudioSource bgmAudioSource;
	public AudioSource sfxAudioSource;
	public Image brightnessMask;

	public List<AudioSource> sfxAudioSourceList = new List<AudioSource>();
	public List<AudioSource> bgmAudioSourceList = new List<AudioSource>();

	public bool first;

	// Preload before any Start() rins in other scripts
	void Awake () 
	{
		Instance = this;
		DontDestroyOnLoad(Instance.gameObject);

		AudioSource[] audioSourceList = this.GetComponentsInChildren<AudioSource>();

		if(audioSourceList[0].gameObject.name == "BGMAudioSource")
		{
			bgmAudioSource = audioSourceList[0];
			sfxAudioSource = audioSourceList[1];
		}
		else 
		{
			bgmAudioSource = audioSourceList[1];
			sfxAudioSource = audioSourceList[0];
		}
	}

	public AudioClip FindAudioClip(AudioClipID audioClipID)
	{
		for(int i=0; i<audioClipInfoList.Count; i++)
		{
			if(audioClipInfoList[i].audioClipID == audioClipID)
			{
				return audioClipInfoList[i].audioClip;
			}
		}

		Debug.LogError("Cannot Find Audio Clip : " + audioClipID);

		return null;
	}

	//! BACKGROUND MUSIC (BGM)
	public void PlayBGM(AudioClipID audioClipID)
	{
		bgmAudioSource.clip = FindAudioClip(audioClipID);
		Debug.Log (audioClipID);
		bgmAudioSource.volume = bgmVolume;
		//bgmAudioSource.loop = true;
		bgmAudioSource.Play();
	}

	public void PauseBGM()
	{
		if(bgmAudioSource.isPlaying)
		{
			bgmAudioSource.Pause();
		}
	}

	public void StopBGM()
	{
		if(bgmAudioSource.isPlaying)
		{
			bgmAudioSource.Stop();
		}
	}


	//! SOUND EFFECTS (SFX)
	public void PlaySFX(AudioClipID audioClipID)
	{
		sfxAudioSource.PlayOneShot(FindAudioClip(audioClipID), sfxVolume / 2.5f);
	}

	public void PlayLoopingSFX(AudioClipID audioClipID)
	{
		AudioClip clipToPlay = FindAudioClip(audioClipID);

		for(int i=0; i<sfxAudioSourceList.Count; i++)
		{
			if(sfxAudioSourceList[i].clip == clipToPlay)
			{
				if(sfxAudioSourceList[i].isPlaying)
				{
					return;
				}

				sfxAudioSourceList[i].volume = sfxVolume;
				sfxAudioSourceList[i].Play();
				return;
			}
		}

		AudioSource newInstance = gameObject.AddComponent<AudioSource>();
		newInstance.clip = clipToPlay;
		newInstance.volume = sfxVolume;
		newInstance.loop = true;
		newInstance.Play();
		sfxAudioSourceList.Add(newInstance);
	}

	public void PauseLoopingSFX(AudioClipID audioClipID)
	{
		AudioClip clipToPause = FindAudioClip(audioClipID);

		for(int i=0; i<sfxAudioSourceList.Count; i++)
		{
			if(sfxAudioSourceList[i].clip == clipToPause)
			{
				sfxAudioSourceList[i].Pause();
				return;
			}
		}
	}	

	public void StopLoopingSFX(AudioClipID audioClipID)
	{
		AudioClip clipToStop = FindAudioClip(audioClipID);

		for(int i=0; i<sfxAudioSourceList.Count; i++)
		{
			if(sfxAudioSourceList[i].clip == clipToStop)
			{
				sfxAudioSourceList[i].Stop();
				return;
			}
		}
	}

	public void ChangePitchLoopingSFX(AudioClipID audioClipID, float value)
	{
		AudioClip clipToStop = FindAudioClip(audioClipID);

		for(int i=0; i<sfxAudioSourceList.Count; i++)
		{
			if(sfxAudioSourceList[i].clip == clipToStop)
			{
				sfxAudioSourceList[i].pitch = value;
				return;
			}
		}
	}

	public void SetBGMVolume(float value)
	{
		bgmVolume = value;
		bgmAudioSource.volume = bgmVolume;
	}

	public void SetSFXVolume(float value)
	{
		sfxVolume = value;
		sfxAudioSource.volume = sfxVolume;
	}

	public void SetBrightness(float value)
	{
		brightness = value;
		brightnessMask.color = new Color(0, 0, 0, 1 - brightness);
	}

	public void PlayButtonSound()
	{
		SoundManagerScript.Instance.PlaySFX(AudioClipID.SFX_UI_BUTTON);
	}

	public IEnumerator BGMFadeVolume(float end, float time, float delay)
	{
		yield return new WaitForSeconds(delay);

		float curr = bgmVolume;

		if(curr > end)
		{
			for(float t = curr; t > end; t-= Time.deltaTime / time)
			{
				Debug.Log(t);

				//bgmAudioSource.volume = Mathf.Lerp(curr, end, time);
				bgmVolume = t;
				bgmAudioSource.volume = t;

				yield return null;
			}

			bgmVolume = bgmAudioSource.volume;
			yield break;
		}
		else
		{
			for(float t = curr; t < end; t+= Time.deltaTime / time)
			{
				//bgmAudioSource.volume = Mathf.Lerp(curr, end, time);
				bgmVolume = t;
				bgmAudioSource.volume = t;

				yield return null;
			}

			bgmVolume = bgmAudioSource.volume;
			yield break;
		}
	}

	public IEnumerator SFXFadeVolume(float end, float time)
	{
		float curr = sfxVolume;

		if(curr > end)
		{
			for(float t = curr; t > end; t-= Time.deltaTime / time)
			{
				//bgmAudioSource.volume = Mathf.Lerp(curr, end, time);
				sfxAudioSource.volume = t;

				yield return null;
			}

			sfxVolume = sfxAudioSource.volume;
			yield break;
		}
		else
		{
			for(float t = curr; t < end; t+= Time.deltaTime / time)
			{
				//bgmAudioSource.volume = Mathf.Lerp(curr, end, time);
				sfxAudioSource.volume = t;

				yield return null;
			}

			sfxVolume = sfxAudioSource.volume;
			yield break;
		}
	}
}
