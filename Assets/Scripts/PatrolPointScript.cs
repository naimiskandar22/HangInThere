using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Dir
{
	UP,
	DOWN,
	LEFT,
	RIGHT,
	TOTAL,
}

[System.Serializable]
public struct PatrolPoint
{
	public PatrolPointScript connectedPoint;
	public Dir dir;
}

public class PatrolPointScript : MonoBehaviour {

	public BoxCollider2D coll;
	public PatrolPoint[] points;

	public Dir side;
	public GameObject platformParent;
	public int parentNum;

	// Use this for initialization
	void Start () 
	{
		coll = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
