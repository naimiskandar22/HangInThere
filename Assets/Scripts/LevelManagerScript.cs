using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct PlatformWalkers
{
	public List<EnemyScript> walkers;
}

public class LevelManagerScript : MonoBehaviour 
{

	public static LevelManagerScript instance;

	public EnemySpawnerScript enemySpawner;
	public PlayerScript player;

	//Pause Manager
	public bool isPaused;
	public GameObject pauseMenu;

	//WinLose
	public Text winLoseText;
	public Text scoreText;

	//Tutorial
	public GameObject tutorialMenu;
	public Text ruleText;
	float flashTime = 1f;

	//Particle Pool 
	public List<GameObject> liveParticlePool = new List<GameObject>();
	public List<GameObject> reserveParticlePool = new List<GameObject>();

	//UI
	public RectTransform soundIcon;
	public Image noiseMeter;
	public Image noiseMeterBorder;
	public Text scorePointText;
	public Text scoreNotifierText;
	public float scoreTimer;
	public bool scoreSet = false;

	//Start game
	bool startGame = false;
	bool onetime = false;

	public List<PlatformWalkers> platformers = new List<PlatformWalkers>();

	void Awake()
	{
		if(instance == null) instance = this;
	}

	// Use this for initialization
	void Start () 
	{
		SoundManagerScript.Instance.StartCoroutine(SoundManagerScript.Instance.BGMFadeVolume(0.01f, 1f, 0f));

		if(!SoundManagerScript.Instance.bgmAudioSource.loop)
		{
			SoundManagerScript.Instance.bgmAudioSource.loop = true;
		}

		GameManagerScript.instance.soundManager.PlayBGM(AudioClipID.BGM_LEVELINTRO);

		for(int i = 0; i < 10; i++)
		{
			GameObject go = Instantiate(enemySpawner.explodeParticlePrefab, transform.position, Quaternion.identity);

			go.SetActive(false);

			reserveParticlePool.Add(go);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!startGame)
		{
			if(onetime)
			{
				flashTime -= Time.deltaTime;

				if(flashTime <= 0.5f)
				{
					if(ruleText.color != Color.red)
					{
						ruleText.color = Color.red;
					}

					if(flashTime <= 0f)
					{
						flashTime = 1f;
					}
				}
				else if(flashTime <= 1f)
				{
					if(ruleText.color != Color.yellow)
					{
						ruleText.color = Color.yellow;
					}
				}
			}
		}

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(!startGame)
			{
				if(!onetime)
				{
					onetime = true;
					StartCoroutine(StartLevel(8f));

					GameManagerScript.instance.soundManager.bgmAudioSource.loop = false;
				}
			}
			else
			{

				isPaused = !isPaused;

				if(isPaused)
				{
					pauseMenu.SetActive(true);
				}
				else
				{
					pauseMenu.SetActive(false);
				}
			}
		}

		UpdateNoiseMeter();

		if(isPaused) return;

		if(scoreSet)
		{
			scoreTimer -= Time.deltaTime;

			if(scoreTimer <= 0f)
			{
				scoreSet = false;
				scoreTimer = 1f;

				scoreNotifierText.text = "";
			}
		}

		if(player.health >= 100f)
		{
			if(!pauseMenu.activeSelf)
			{
				noiseMeter.gameObject.SetActive(false);
				soundIcon.gameObject.SetActive(false);
				scorePointText.gameObject.SetActive(false);

				isPaused = true;

				pauseMenu.SetActive(true);
				scoreText.gameObject.SetActive(true);
				winLoseText.gameObject.SetActive(true);

				winLoseText.text = "You Made Too Much Noise";
			}
			else
			{
				return;
			}
		}

		CheckLiveEnemies();
	}

	IEnumerator StartLevel(float time)
	{
		float timer = 1.0f;
		bool playOnce = false;

		while(timer >= 0.0f)
		{
			timer -= Time.deltaTime / time;

			player.health = timer * 100f;

			if(!playOnce)
			{
				if(timer > 0f && timer < 0.75f)
				{
					playOnce = true;

					GameManagerScript.instance.soundManager.bgmAudioSource.loop = true;

					GameManagerScript.instance.soundManager.PlayBGM(AudioClipID.BGM_LEVEL);
				}
			}


			yield return null;
		}

		startGame = true;
		isPaused = false;
		pauseMenu.SetActive(false);


		if(tutorialMenu.activeSelf)
		{
			tutorialMenu.SetActive(false);
		}

	}

	void CheckLiveEnemies()
	{
		bool allDead = true;

		for(int i = 0; i < enemySpawner.enemyList.Count; i++)
		{
			if(enemySpawner.enemyList[i].isAlive)
			{
				allDead = false;

				break;
			}
		}

		if(enemySpawner.enemyList.Count >= 20 && allDead)
		{
			for(int i = 0; i < LevelManagerScript.instance.platformers.Count; i++)
			{
				if(LevelManagerScript.instance.platformers[i].walkers.Count > 0)
				{
					for(int j = 0; j < LevelManagerScript.instance.platformers[i].walkers.Count; j++)
					{
						if(j >= 1)
						{
							enemySpawner.enemyList.Remove(LevelManagerScript.instance.platformers[i].walkers[j]);

							if(LevelManagerScript.instance.platformers[i].walkers[j].gameObject != null)
							{
								Destroy(LevelManagerScript.instance.platformers[i].walkers[j].gameObject);
							}

							LevelManagerScript.instance.platformers[i].walkers.Remove(LevelManagerScript.instance.platformers[i].walkers[j]);
						}
					}
				}
			}

			for(int i = 0; i < liveParticlePool.Count; i++)
			{
				GameObject go = liveParticlePool[i];

				liveParticlePool.Remove(go);
				reserveParticlePool.Add(go);

				go.SetActive(false);
			}

			for(int i = 0; i < enemySpawner.enemyList.Count; i++)
			{
				enemySpawner.enemyList[i].Respawn();
			}
		}
		else if(allDead)
		{
			for(int i = 0; i < liveParticlePool.Count; i++)
			{
				GameObject go = liveParticlePool[i];
				
				liveParticlePool.Remove(go);
				reserveParticlePool.Add(go);

				go.SetActive(false);
			}

			for(int i = 0; i < enemySpawner.enemyList.Count; i++)
			{
				enemySpawner.enemyList[i].Respawn();
			}
		}
	}

	void UpdateNoiseMeter()
	{
		noiseMeter.fillAmount = (player.health / 100f);

		if(noiseMeter.fillAmount <= 0.2f)
		{
			noiseMeter.color = new Color(0f, 0f, 1f);
		}
		else if(noiseMeter.fillAmount <= 0.4f)
		{
			noiseMeter.color = new Color(0f, 1f, 1f);
		}
		else if(noiseMeter.fillAmount <= 0.5f)
		{
			noiseMeter.color = new Color(0f, 1f, 0f);
		}
		else if(noiseMeter.fillAmount <= 0.7f)
		{
			noiseMeter.color = new Color(1f, 1f, 0f);
		}
		else if(noiseMeter.fillAmount <= 0.9f)
		{
			noiseMeter.color = new Color(1f, (165f/225f), 0f);
		}
		else if(noiseMeter.fillAmount <= 1f)
		{
			noiseMeter.color = new Color(1f, 0f, 0f);
		}

		soundIcon.sizeDelta = new Vector2(60f, ((player.health + 1f) / 100f) * 40f);
	}

	public void LoadMainMenu()
	{
		SceneManager.LoadScene(0);

		GameManagerScript.instance.first = true;

	}
}
