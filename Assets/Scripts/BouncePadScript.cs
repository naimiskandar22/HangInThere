using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePadScript : MonoBehaviour {

	Animator anim;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		anim.Play("Bounce_Idle");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void StartBounce()
	{
		anim.Play("Bounce_Hit");
	}
}
