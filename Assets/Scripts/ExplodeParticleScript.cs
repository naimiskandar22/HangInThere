using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeParticleScript : MonoBehaviour {

	bool grounded = false;
	BoxCollider2D coll;
	Rigidbody2D rb;
	public SpriteRenderer rend;

	public Transform downCheck;
	float groundCheckRadius = 0.2f;
	public LayerMask groundCheckMask;

	float lifetimeTimer = 3.0f;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody2D>();
		coll = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		lifetimeTimer -= Time.deltaTime;

		if(lifetimeTimer <= (3f / 6f) * 1f)
		{
			rend.color = Color.yellow;
		}
		else if(lifetimeTimer <= (3f / 6f) * 2f)
		{
			rend.color = Color.green;
		}
		else if(lifetimeTimer <= (3f / 6f) * 3f)
		{
			rend.color = Color.yellow;
		}
		else if(lifetimeTimer <= (3f / 6f) * 4f)
		{
			rend.color = Color.green;
		}
		else if(lifetimeTimer <= (3f / 6f) * 5f)
		{
			rend.color = Color.yellow;
		}
		else if(lifetimeTimer <= (3f / 6f) * 6f)
		{
			rend.color = Color.green;
		}

		if(lifetimeTimer <= 0f)
		{
			lifetimeTimer = 3.0f;

			LevelManagerScript.instance.liveParticlePool.Remove(gameObject);
			LevelManagerScript.instance.reserveParticlePool.Add(gameObject);

			gameObject.SetActive(false);
		}
	}

	void FixedUpdate()
	{
		grounded = Physics2D.OverlapCircle(downCheck.position, groundCheckRadius, groundCheckMask);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Enemy"))
		{
			EnemyScript enemy = other.GetComponent<EnemyScript>();

			if(enemy != null)
			{
				if(enemy.isAlive)
				{
					if(!enemy.expTriggered)
					{
						enemy.canMove = false;
						enemy.anim.speed = 0f;

						enemy.StartCoroutine(enemy.TriggerExplosion(true));
					}
				}
			}
		}
		else if(other.CompareTag("Platform"))
		{
			if(other != null)
			{
				if(rb != null)
				{
					if(rb.bodyType != RigidbodyType2D.Kinematic)
					{
						rb.velocity = Vector2.zero;
						rb.bodyType = RigidbodyType2D.Kinematic;
					}
				}
			}
		}
		else if(other.CompareTag("Player"))
		{
			if(LevelManagerScript.instance.player.health < 100f)
			{
				if(!LevelManagerScript.instance.player.damageFlash)
				{
					LevelManagerScript.instance.player.health += 10f;
					LevelManagerScript.instance.player.damageFlash = true;
					LevelManagerScript.instance.player.flashTime = 0.25f;
					LevelManagerScript.instance.noiseMeterBorder.color = Color.red;

					int rand = Random.Range(4, 8);

					if(!GameManagerScript.instance.soundManager.sfxAudioSource.isPlaying)
					{
						GameManagerScript.instance.soundManager.PlaySFX(SoundManagerScript.Instance.audioClipInfoList[rand].audioClipID);
					}
				}
			}

			LevelManagerScript.instance.liveParticlePool.Remove(gameObject);
			LevelManagerScript.instance.reserveParticlePool.Add(gameObject);

			gameObject.SetActive(false);
		}
	}
}
