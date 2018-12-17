using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

	public float health;

	bool canMove = true;
	float moveSpeed = 5f;
	public Dir currDir;

	bool willStumble = false;
	float minFall = 15f;

	float stumbleTimer = 2.5f;

	[Range(1, 10)]
	public float jumpForce;
	float fallMultiplier = 2f;
	float lowJumpMultiplier = 2.5f;
	bool grounded;
	public Transform downCheck;
	float groundCheckRadius = 0.1f;
	public LayerMask groundCheckMask;
	public LayerMask stompMask;

	GameObject lastBouncy;

	//Score Check
	public int currScore = 0;

	//Damage Flash
	public bool damageFlash = false;
	public float flashTime;


	Rigidbody2D rb;
	SpriteRenderer rend;
	Animator anim;
	BoxCollider2D coll;


	// Use this for initialization
	void Start () 
	{
		currDir = Dir.RIGHT;

		rb = GetComponent<Rigidbody2D>();
		rend = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
		coll = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(LevelManagerScript.instance.isPaused) return;

		if(health > 0f)
		{
			health -= Time.deltaTime;
		}
		else
		{
			health = 0f;
		}


		if(!canMove)
		{
			stumbleTimer -= Time.deltaTime;

			if(stumbleTimer <= 0f)
			{
				willStumble = false;
				canMove = true;
				stumbleTimer = 2.5f;
			}
		}

		Move();
		Jump();

		GroundSlam();
		DamageFlash();
	}

	void FixedUpdate()
	{
		if(LevelManagerScript.instance.isPaused) return;

		grounded = Physics2D.OverlapCircle(downCheck.position, groundCheckRadius, groundCheckMask);

//		if(grounded)
//		{
//			if(willStumble)
//			{
//				canMove = false;
//			}
//		}

		Stomp();
	}

	void DamageFlash()
	{
		if(damageFlash)
		{
			flashTime -= Time.deltaTime;

			if(rend.color != Color.red)
			{
				rend.color = Color.red;
			}

			if(flashTime <= 0f)
			{
				rend.color = Color.white;
				damageFlash = false;
				LevelManagerScript.instance.noiseMeterBorder.color = Color.black;

			}
		}
	}

	void Move()
	{
		if(canMove)
		{
			if(!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
			{
				if(grounded)
				{
					anim.StopPlayback();
					anim.Play("Player_Idle");
				}
				else
				{
					anim.Play("Player_Jump");
				}

			}
			else if(Input.GetKey(KeyCode.RightArrow))
			{
				if(rend.flipX)
				{
					rend.flipX = false;
				}

				transform.Translate(Vector2.right * Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime);

				if(grounded)
				{
					if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Running"))
					{
						anim.Play("Player_Running");
						//anim.StartPlayback();
					}
				}
				else
				{
					if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Jump"))
					{
						anim.Play("Player_Jump");
						//anim.StartPlayback();
					}
				}
			}
			else if(Input.GetKey(KeyCode.LeftArrow))
			{
				if(!rend.flipX)
				{
					rend.flipX = true;
				}

				transform.Translate(Vector2.right * Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime);

				if(grounded)
				{
					if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Running"))
					{
						anim.Play("Player_Running");
						//anim.StartPlayback();
					}
				}
				else
				{
					if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Jump"))
					{
						anim.Play("Player_Jump");
						//anim.StartPlayback();
					}
				}
			}
		}
	}

	void Jump()
	{
		if(canMove)
		{
			if(Input.GetKeyDown(KeyCode.UpArrow))
			{
				if(grounded)
				{
					rb.velocity = Vector2.up * jumpForce;

					anim.Play("Player_Jump");
				}
			}
		}

		if(rb.velocity.y < 0)
		{
			rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
		}
		else if(rb.velocity.y > 0 && !Input.GetKeyDown(KeyCode.UpArrow))
		{
			rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
		}

		if(rb.velocity.y <= -minFall)
		{
			if(!willStumble)
			{
				willStumble = true;
			}
		}
		else
		{
			if(willStumble)
			{
				willStumble = false;
			}
		}
	}

	void Stomp()
	{
		if(rb.velocity.y < 0)
		{
			Collider2D[] collider = Physics2D.OverlapCircleAll(downCheck.position, 0.4f, stompMask);

			if(collider.Length > 0)
			{
				Debug.Log(collider.Length);
			}

			foreach(Collider2D go in collider)
			{
				EnemyScript enemy = go.transform.parent.GetComponent<EnemyScript>();

				if(enemy != null)
				{
					Debug.Log("Enemy");

					if(enemy.isAlive)
					{
						if(!enemy.expTriggered)
						{
							Debug.Log("Stomp");

							enemy.canMove = false;
							enemy.StartCoroutine(enemy.TriggerExplosion(false));

							rb.AddForce(Vector2.up * 8f, ForceMode2D.Impulse);

							currScore += 100;

							LevelManagerScript.instance.scoreText.text = "Score Points : " + currScore.ToString();
							LevelManagerScript.instance.scorePointText.text = "Score Points : " + currScore.ToString();

							LevelManagerScript.instance.scoreSet = true;
							LevelManagerScript.instance.scoreNotifierText.text = "Germ Kill\t" + currScore.ToString();

							GameManagerScript.instance.soundManager.PlaySFX(AudioClipID.SFX_UI_BUTTON);
						}
					}
				}
			}
		}
	}

	void GroundSlam()
	{
		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			if(!grounded)
			{
				rb.velocity = Vector2.zero;

				Invoke("Slam", 0.1f);
			}
		}
	}

	void Slam()
	{
		rb.AddForce(Vector2.down * 20f, ForceMode2D.Impulse);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Bouncepad"))
		{
			Debug.Log("Touch Bouncy");

			if(rb.velocity.y < 0)
			{
				rb.AddForce(Vector2.up * (-rb.velocity.y * 2.25f), ForceMode2D.Impulse);

				other.GetComponent<BouncePadScript>().StartBounce();

				int points;

				if(lastBouncy == null)
				{
					points = 10;

					LevelManagerScript.instance.scoreNotifierText.text = "Bouncy!\t" + points.ToString();
				}
				else if(lastBouncy != other.gameObject)
				{
					points = 25;

					LevelManagerScript.instance.scoreNotifierText.text = "More Bouncy!\t" + points.ToString();
				}
				else
				{
					points = 10;

					LevelManagerScript.instance.scoreNotifierText.text = "Bouncy!\t" + points.ToString();
				}

				currScore += points;

				LevelManagerScript.instance.scoreText.text = "Score Points : " + currScore.ToString();
				LevelManagerScript.instance.scorePointText.text = "Score Points : " + currScore.ToString();

				lastBouncy = other.gameObject;

				LevelManagerScript.instance.scoreSet = true;
			}
		}
		else if(other.CompareTag("Platform"))
		{
			rb.velocity = Vector3.zero;
		}
	}
}
