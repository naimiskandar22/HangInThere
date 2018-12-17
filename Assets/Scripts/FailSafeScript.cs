using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailSafeScript : MonoBehaviour 
{
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.layer == 9)
		{
			GameObject go = other.transform.parent.gameObject;

			LevelManagerScript.instance.enemySpawner.enemyList.Remove(go.GetComponent<EnemyScript>());

			Destroy(go);
		}
	}
}
