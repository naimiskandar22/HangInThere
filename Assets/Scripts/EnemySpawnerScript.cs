using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour {

	public List<EnemyScript> enemyList = new List<EnemyScript>();

	public GameObject enemyPrefab;
	public GameObject explodeParticlePrefab;

	// Use this for initialization
	void Start () 
	{
		GameObject[] temp = GameObject.FindGameObjectsWithTag("Enemy");

		for(int i = 0; i < temp.Length; i++)
		{
			enemyList.Add(temp[i].GetComponent<EnemyScript>());
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
