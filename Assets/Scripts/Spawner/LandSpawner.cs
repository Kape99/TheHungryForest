using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LandSpawner : Spawn
{
	public Transform entityToSpawn;

	public List<GameObject> entityList;

	public Text entityText;

	private EnergyManager sm;

	private Vector3 spawnPosition;

	void Start()
	{
		gameI.GetComponentInChildren<RawImage>().texture = iconUI;
		gameI.GetComponentInChildren<Text>().text = getListLength().ToString();
		// Find variables
		spawnPosition = GameObject.Find("SpawnPlane").transform.position;
	}





	public override int getListLength()
	{
		//gameI.GetComponentInChildren<Text>().text = getListLength().ToString();
		return entityList.Count;
	}

	//Removiing the game object add to your energy counter is energy value
	public override void removeFromList(GameObject go)
	{
		entityList.Remove(go);
		gameI.GetComponentInChildren<Text>().text = getListLength().ToString();

	}

	public override void spawn(int i)
	{
		for (int j = 0; j < i; j++)
		{
		// Spawn the entity
		var entity = Instantiate (entityToSpawn, spawnPosition, Quaternion.identity);
		entityList.Add(entity.gameObject);
		entity.transform.parent = gameObject.transform;
		entity.GetComponent<Creature>().setSpawn(this);
			
		// Ensure we have a NavMeshAgent
		if (entity.GetComponent<NavMeshAgent> () == null) {
			Debug.LogError ("Attempting to spawn something that doesn't have a navmesh agent!");
		}

		// Set location again 
		// (this is a bug fix for entity moving to random position on instantiate)
		entity.GetComponent<NavMeshAgent> ().Warp (spawnPosition); 	
		}
		gameI.GetComponentInChildren<Text>().text = getListLength().ToString();

	}

	public override Sprite GetsSprite()
	{
		throw new System.NotImplementedException();
	}


	public override int getCost()
	{
		return 0;
	}
}
