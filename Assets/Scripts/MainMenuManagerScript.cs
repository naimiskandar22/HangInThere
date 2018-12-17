using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class MainMenuManagerScript : MonoBehaviour {

	public Rigidbody2D rbBaby;

	public GameObject soundManagerPrefab;

	void Awake()
	{
//		GameObject[] soundManager = GameObject.FindGameObjectsWithTag("SoundManager");
//
//		if(soundManager.Length == 0)
//		{
//			Instantiate(soundManagerPrefab, transform.position, Quaternion.identity);
//		}
//		else if(soundManager.Length > 1)
//		{
//			for(int i = 0; i < soundManager.Length; i++)
//			{
//				if(!soundManager[i].GetComponent<SoundManagerScript>().first)
//				{
//					Destroy(soundManager[i]);
//				}
//			}
//		}

		GameObject[] gameManager = GameObject.FindGameObjectsWithTag("GameManager");

		if(gameManager.Length > 1)
		{
			for(int i = 0; i < gameManager.Length; i++)
			{
				if(!gameManager[i].GetComponent<GameManagerScript>().first)
				{
					Destroy(gameManager[i]);
				}
			}
		}
	}

	// Use this for initialization
	void Start () 
	{
		GameManagerScript.instance.soundManager.bgmAudioSource.loop = false;
		GameManagerScript.instance.soundManager.PlayBGM(AudioClipID.BGM_MAINMENUINTRO);
	}

//	IEnumerator Start()
//	{
//		SoundManagerScript.Instance.bgmAudioSource.loop = false;
//		SoundManagerScript.Instance.PlayBGM(AudioClipID.BGM_MAINMENUINTRO);
//
//		Debug.Log("Showing splash screen");
//		SplashScreen.Begin();
//		while (!SplashScreen.isFinished)
//		{
//			SplashScreen.Draw();
//			yield return null;
//		}
//		Debug.Log("Finished showing splash screen");
//	}

	// Update is called once per frame
	void Update () 
	{
		if(!GameManagerScript.instance.soundManager.bgmAudioSource.loop)
		{
			if(!GameManagerScript.instance.soundManager.bgmAudioSource.isPlaying)
			{
				GameManagerScript.instance.soundManager.bgmAudioSource.loop = true;
				GameManagerScript.instance.soundManager.PlayBGM(AudioClipID.BGM_MAINMENU);
			}
		}
	}

	public void GoToNextScene()
	{
		rbBaby.simulated = true;

		GameManagerScript.instance.soundManager.StartCoroutine(SoundManagerScript.Instance.BGMFadeVolume(0.0f, 1f, 0f));

		GameManagerScript.instance.Invoke("LoadFirstLevel", 5);
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
