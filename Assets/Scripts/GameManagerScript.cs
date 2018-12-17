using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {

	public static GameManagerScript instance;

	public GameObject soundManagerPrefab;

	public SoundManagerScript soundManager;

	public bool first;

	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);

		if(instance == null)
		{
			instance = this;
		}

		if(soundManager == null)
		{
			GameObject go = Instantiate(soundManagerPrefab, transform.position, Quaternion.identity);

			soundManager = go.GetComponent<SoundManagerScript>();
		}
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void LoadMainMenu()
	{
		SceneManager.LoadScene(0);
	}

	public void LoadFirstLevel()
	{
		SceneManager.LoadScene(1);
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
