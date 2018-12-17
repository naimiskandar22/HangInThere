using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

	public bool isAlive = true;

	public bool canMove = true;
	float moveSpeed = 1f;
	bool isHopping = false;

	public SpriteRenderer rend;

	public bool isHunting;
	float huntTimer = 5f;
	PlayerScript player;

	float huntRange = 5f;
	public Dir currDir;

	float expRange = 2f;

	public Animator anim;
	public BoxCollider2D coll;
	public Rigidbody2D rb;

	public bool expTriggered = false;
	bool deadDone = false;
	bool grounded;
	public Transform downCheck;
	float groundCheckRadius = 0.2f;
	public LayerMask groundCheckMask;

	public GameObject currGround;
	float failSafeTimer = 2.0f;

	// Use this for initialization
	void Start () 
	{
		player = LevelManagerScript.instance.player;

		anim = GetComponent<Animator>();
		coll = GetComponent<BoxCollider2D>();
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(LevelManagerScript.instance.isPaused) return;

		if(isAlive)
		{
			Hunt();
		}

		if(!grounded)
		{
			failSafeTimer -= Time.deltaTime;

			if(failSafeTimer <= 0f)
			{
				LevelManagerScript.instance.enemySpawner.enemyList.Remove(this);

				for(int i = 0; i < LevelManagerScript.instance.platformers.Count; i++)
				{
					LevelManagerScript.instance.platformers[i].walkers.Remove(this);
				}

				Destroy(gameObject);
			}
		}
	}

	void FixedUpdate()
	{
		CheckGround();

		if(!isAlive && deadDone)
		{
			DeadZeroVelocity();
		}
	}

	void CheckGround()
	{
		grounded = Physics2D.OverlapCircle(downCheck.position, groundCheckRadius, groundCheckMask);

		if(grounded)
		{
			if(failSafeTimer != 2.0f)
			{
				failSafeTimer = 2.0f;
			}
		}
	}

	void MoveRight()
	{
		if(canMove)
		{
			if(!isHopping)
			{
				if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Running"))
				{
					anim.Play("Player_Running");
					//anim.StartPlayback();
				}
			}

			if(currDir != Dir.RIGHT)
			{
				currDir = Dir.RIGHT;
			}

			if(rend.flipX)
			{
				rend.flipX = false;
			}

			transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
		}
	}

	void MoveLeft()
	{
		if(canMove)
		{
			if(!isHopping)
			{
				if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Running"))
				{
					anim.Play("Player_Running");
				}
			}


			if(currDir != Dir.RIGHT)
			{
				currDir = Dir.LEFT;
			}

			if(!rend.flipX)
			{
				rend.flipX = true;
			}

			transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
		}
	}

	void Hunt()
	{
		Vector2 compare = transform.position - player.transform.position;

		if(compare.x >= 0 && currDir == Dir.LEFT)
		{
			if(Vector2.Distance(transform.position, player.transform.position) <= huntRange)
			{
				huntRange = 5f;

				isHunting = true;
			}
		}
		else if(compare.x <= 0 && currDir == Dir.RIGHT)
		{
			if(Vector2.Distance(transform.position, player.transform.position) <= huntRange)
			{
				huntRange = 5f;

				isHunting = true;
			}
		}

		//Movement when hunting
		if(isHunting)
		{
			if(isAlive)
			{
				if(canMove)
				{
					if(Vector2.Distance(transform.position, player.transform.position) <= expRange)
					{
						if(compare.x >= 0 && compare.y >= -0.5f && compare.y <= 0.5f && currDir == Dir.LEFT)
						{
							if(!expTriggered)
							{
								canMove = false;
								anim.speed = 0f;

								StartCoroutine(TriggerExplosion(true));
							}
						}
						else if(compare.x <= 0 && compare.y >= -0.5f && compare.y <= 0.5f && currDir == Dir.RIGHT)
						{
							if(!expTriggered)
							{
								canMove = false;
								anim.speed = 0f;

								StartCoroutine(TriggerExplosion(true));
							}
						}
					}
				}
			}

			if(compare.x >= 0 && compare.y >= -0.5f && compare.y <= 0.5f)
			{
				MoveLeft();
			}
			else if(compare.x <= 0 && compare.y >= -0.5f && compare.y <= 0.5f)
			{
				MoveRight();
			}
			else
			{
				if(currDir == Dir.RIGHT)
				{
					MoveRight();
				}
				else if(currDir == Dir.LEFT)
				{
					MoveLeft();
				}
			}
		}
		else
		{
			if(currDir == Dir.RIGHT)
			{
				MoveRight();
			}
			else if(currDir == Dir.LEFT)
			{
				MoveLeft();
			}
		}

		//Timer when hunting
		if(isHunting)
		{
			if(Vector2.Distance(transform.position, player.transform.position) > huntRange)
			{
				huntTimer -= Time.deltaTime;

				if(huntTimer <= 0f)
				{
					isHunting = false;
					huntRange = 5f;
				}
			}
			else
			{
				huntTimer = 5f;
			}
		}

	}

	public IEnumerator TriggerExplosion(bool live)
	{
		if(live)
		{
			expTriggered = true;

			int loopNum = 1;

			float colorFlashTimer = 2f;
			float colorFlashDuration = 2f;

			while(loopNum <= 5)
			{
				yield return new WaitForSeconds(0.0005f);

				colorFlashTimer -= Time.deltaTime * 2f *loopNum;

				if(colorFlashTimer <= colorFlashDuration / 2)
				{
					rend.color = Color.green;

					if(colorFlashTimer <= 0f)
					{
						colorFlashTimer = colorFlashDuration;

						loopNum++;

						yield return new WaitForSeconds(0.1f);
					}
				}
				else if(colorFlashTimer <= colorFlashDuration)
				{
					rend.color = Color.red;
				}
			}

			anim.speed = 1f;
			anim.Play("Player_Explode");

			yield return new WaitForSeconds(1f);

			if(LevelManagerScript.instance.enemySpawner.enemyList.Count <= 20)
			{
				GameObject go = Instantiate(LevelManagerScript.instance.enemySpawner.enemyPrefab, transform.position, Quaternion.identity);

				LevelManagerScript.instance.enemySpawner.enemyList.Add(go.GetComponent<EnemyScript>());

				EnemyScript goscript = go.GetComponent<EnemyScript>();

				for(int i = 0; i < LevelManagerScript.instance.platformers.Count; i++)
				{
					for(int j = 0; j < LevelManagerScript.instance.platformers[i].walkers.Count; j++)
					{
						if(LevelManagerScript.instance.platformers[i].walkers[j] == this)
						{
							LevelManagerScript.instance.platformers[i].walkers.Add(goscript);
						}
					}
				}

				goscript.currGround = currGround;
				goscript.SetDead();

				float rand = Random.Range(1f, 5f);

				goscript.rb.AddForce(Vector2.right * (rand + 10f), ForceMode2D.Impulse);
				rb.AddForce(Vector2.right * -(rand + 10f), ForceMode2D.Impulse);

				goscript.Invoke("DeadZeroVelocity", 0.25f);
			}

			Invoke("DeadZeroVelocity", 0.25f);
			SpawnParticles(true, 5);

			yield return new WaitForSeconds(1f);

			deadDone = true;
			isAlive = false;
		}
		else
		{
			expTriggered = true;

			anim.Play("Player_Explode");

			DeadZeroVelocity();
			SpawnParticles(false, 2);
			Invoke("DeadZeroVelocity", 0.25f);

			yield return new WaitForSeconds(1f);

			isAlive = false;
			deadDone = true;
		}

		yield return null;
	}

	public void SpawnParticles(bool live, int num)
	{
		for(int i = 0; i < num; i++)
		{
			GameObject go = LevelManagerScript.instance.enemySpawner.explodeParticlePrefab;

			if(LevelManagerScript.instance.reserveParticlePool.Count == 0)
			{
				go = Instantiate(LevelManagerScript.instance.enemySpawner.explodeParticlePrefab, transform.position, Quaternion.identity);

				LevelManagerScript.instance.liveParticlePool.Add(go);
			}
			else
			{
				for(int j = 0; j < LevelManagerScript.instance.reserveParticlePool.Count; j++)
				{
					if(!LevelManagerScript.instance.reserveParticlePool[j].activeSelf)
					{
						go = LevelManagerScript.instance.reserveParticlePool[j];

						go.gameObject.SetActive(true);

						go.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

						go.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

						LevelManagerScript.instance.reserveParticlePool.Remove(go);
						LevelManagerScript.instance.liveParticlePool.Add(go);

						break;
					}
				}

			}

			if(go != null)
			{
				Rigidbody2D gorb = go.GetComponent<Rigidbody2D>();

				//gorb.AddForce(Vector2.up* 50f + Vector2.right* 50f, ForceMode2D.Impulse);

				if(live)
				{
					gorb.AddForce(Vector2.up * Mathf.Sin((180f / num - 90f) * i * Mathf.Deg2Rad) * 10f + Vector2.right * Mathf.Cos((180f / num - 90f) * i * Mathf.Deg2Rad) * 2f, ForceMode2D.Impulse);
				}
				else
				{
					gorb.AddForce(Vector2.up * Mathf.Sin((180f / num) * i * Mathf.Deg2Rad) * 1f + Vector2.right * Mathf.Cos((180f / num) * i * Mathf.Deg2Rad) * 1f, ForceMode2D.Impulse);
				}
			}
		}
		
	}

	public void Respawn()
	{
		anim.Play("Player_Respawn");

		Invoke("SetAlive", 1f);
	}

	void SetAlive()
	{
		isAlive = true;
		canMove = true;
		expTriggered = false;

		coll.enabled = true;
		rb.bodyType = RigidbodyType2D.Dynamic;

		deadDone = false;
	}

	void SetDead()
	{
		expTriggered = true;

		anim.Play("Player_Slime");

		isAlive = false;
		canMove = false;
		deadDone = true;
	}

	void DeadZeroVelocity()
	{
		if(grounded)
		{
			rb.velocity = Vector2.zero;
			coll.enabled = false;
			rb.bodyType = RigidbodyType2D.Kinematic;
		}
	}
		

	IEnumerator Hop(Vector3 endPoint, float time, float hopHeight)
	{
		if(!isHopping)
		{
			anim.Play("Player_Jump");

			isHopping = true;

			//yield return new WaitForSeconds(waitTime);

			Vector3 startPos = transform.position;
			float timer = 0.0f;

			while(timer <= 1.0f)
			{
				float height = Mathf.Sin(Mathf.PI * timer) * hopHeight;
				transform.position = Vector3.Lerp(startPos, endPoint, timer) + Vector3.up * height;

				timer += Time.deltaTime / time;
				yield return null;
			}

			isHopping = false;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("PatrolPoint"))
		{
			if(canMove && isAlive)
			{
				PatrolPointScript point = other.GetComponent<PatrolPointScript>();

				Vector2 compare = transform.position - player.transform.position;

				int rand = Random.Range(0, point.points.Length + 1);

				if(rand == 0)
				{
					if(currGround != point.platformParent)
					{
						currGround = point.platformParent;

						for(int i = 0; i < LevelManagerScript.instance.platformers.Count; i++)
						{
							LevelManagerScript.instance.platformers[i].walkers.Remove(this);
						}

						LevelManagerScript.instance.platformers[point.parentNum].walkers.Add(this);
					}

					if(point.side == Dir.RIGHT)
					{
						if(currDir == Dir.RIGHT)
						{
							currDir = Dir.LEFT;
						}
					}
					else if(point.side == Dir.LEFT)
					{
						if(currDir == Dir.LEFT)
						{
							currDir = Dir.RIGHT;
						}
					}
				}
				else
				{
					if(point.points[rand - 1].dir == Dir.DOWN)
					{
						if(currGround != point.points[rand - 1].connectedPoint.platformParent)
						{
							currGround = point.points[rand - 1].connectedPoint.platformParent;

							for(int i = 0; i < LevelManagerScript.instance.platformers.Count; i++)
							{
								LevelManagerScript.instance.platformers[i].walkers.Remove(this);
							}

							LevelManagerScript.instance.platformers[point.points[rand - 1].connectedPoint.parentNum].walkers.Add(this);
						}

						StartCoroutine(Hop(point.points[rand - 1].connectedPoint.transform.position, 1f, transform.position.y + 1.5f));

						if(point.points[rand - 1].connectedPoint.side == Dir.RIGHT)
						{
							if(currDir != Dir.LEFT)
							{
								currDir = Dir.LEFT;
							}
						}
						else if(point.points[rand - 1].connectedPoint.side == Dir.LEFT)
						{
							if(currDir != Dir.RIGHT)
							{
								currDir = Dir.RIGHT;
							}
						}
					}
					else
					{
						if(currGround != point.points[rand - 1].connectedPoint.platformParent)
						{
							currGround = point.points[rand - 1].connectedPoint.platformParent;

							for(int i = 0; i < LevelManagerScript.instance.platformers.Count; i++)
							{
								LevelManagerScript.instance.platformers[i].walkers.Remove(this);
							}

							LevelManagerScript.instance.platformers[point.points[rand - 1].connectedPoint.parentNum].walkers.Add(this);
						}

						StartCoroutine(Hop(point.points[rand - 1].connectedPoint.transform.position, 1f, point.points[rand - 1].connectedPoint.transform.position.y + 1.5f));

					}
				}

				//			if(isHunting)
				//			{
				//				if(compare.y < 0)
				//				{
				//					bool found = false;
				//
				//					if(compare.x >= 0)
				//					{
				//						if(point.side == Dir.LEFT)
				//						{
				//							for(int i = 0; i < point.points.Length; i++)
				//							{
				//								if(point.points[i].dir == Dir.UP)
				//								{
				//									StartCoroutine(Hop(point.points[i].connectedPoint.transform.position, 1f, point.points[i].connectedPoint.transform.position.y + 2.5f));
				//
				//									found = true;
				//								}
				//							}
				//						}
				//					}
				//					else
				//					{
				//						if(point.side == Dir.RIGHT)
				//						{
				//							for(int i = 0; i < point.points.Length; i++)
				//							{
				//								if(point.points[i].dir == Dir.UP)
				//								{
				//									StartCoroutine(Hop(point.points[i].connectedPoint.transform.position, 1f, point.points[i].connectedPoint.transform.position.y + 2.5f));
				//
				//									found = true;
				//								}
				//							}
				//						}
				//					}
				//
				//
				//
				//					if(!found)
				//					{
				//						if(currDir == Dir.RIGHT)
				//						{
				//							currDir = Dir.LEFT;
				//						}
				//						else if(currDir == Dir.LEFT)
				//						{
				//							currDir = Dir.RIGHT;
				//						}
				//					}
				//				}
				//				else
				//				{
				//					bool found = false;
				//
				//					if(compare.x >= 0)
				//					{
				//						if(point.side == Dir.LEFT)
				//						{
				//							for(int i = 0; i < point.points.Length; i++)
				//							{
				//								if(point.points[i].dir == Dir.DOWN)
				//								{
				//									StartCoroutine(Hop(point.points[i].connectedPoint.transform.position, 1f, transform.position.y + 2.5f));
				//
				//									found = true;
				//								}
				//							}
				//						}
				//					}
				//					else
				//					{
				//						if(point.side == Dir.RIGHT)
				//						{
				//							for(int i = 0; i < point.points.Length; i++)
				//							{
				//								if(point.points[i].dir == Dir.DOWN)
				//								{
				//									StartCoroutine(Hop(point.points[i].connectedPoint.transform.position, 1f, transform.position.y + 2.5f));
				//
				//									found = true;
				//								}
				//							}
				//						}
				//					}
				//					
				//
				//					if(!found)
				//					{
				//						if(currDir == Dir.RIGHT)
				//						{
				//							currDir = Dir.LEFT;
				//						}
				//						else if(currDir == Dir.LEFT)
				//						{
				//							currDir = Dir.RIGHT;
				//						}
				//					}
				//				}
				//			}
				//			else
				//			{
				//				int rand = Random.Range(0, point.points.Length);
				//
				//				if(rand == 0)
				//				{
				//					if(point.side == Dir.RIGHT)
				//					{
				//						if(currDir == Dir.RIGHT)
				//						{
				//							currDir = Dir.LEFT;
				//						}
				//					}
				//					else if(point.side == Dir.LEFT)
				//					{
				//						if(currDir == Dir.LEFT)
				//						{
				//							currDir = Dir.RIGHT;
				//						}
				//					}
				//				}
				//				else
				//				{
				//					if(point.points[rand - 1].dir == Dir.DOWN)
				//					{
				//						StartCoroutine(Hop(point.points[rand - 1].connectedPoint.transform.position, 1f, transform.position.y + 2.5f));
				//					}
				//					else
				//					{
				//						StartCoroutine(Hop(point.points[rand - 1].connectedPoint.transform.position, 1f, point.points[rand - 1].connectedPoint.transform.position.y + 2.5f));
				//
				//					}
				//				}
				//
				//			}
			}
		}
	}
}
