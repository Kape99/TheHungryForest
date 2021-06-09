using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class SkySpawner : Spawn{
	
	public Transform entityToSpawn;

	public List<GameObject> entityList;
	
	private Vector3[] flyingSpawnPositions = new Vector3[4];

	void Start ()
	{
		gameI.GetComponentInChildren<RawImage>().texture = iconUI;
		gameI.GetComponentInChildren<Text>().text = getListLength().ToString();

		// Find variables
		flyingSpawnPositions[0] = GameObject.Find ("Sky/SkySpawnNorth").transform.position;
		flyingSpawnPositions[1] = GameObject.Find ("Sky/SkySpawnEast").transform.position;
		flyingSpawnPositions[2] = GameObject.Find ("Sky/SkySpawnSouth").transform.position;
		flyingSpawnPositions[3] = GameObject.Find ("Sky/SkySpawnWest").transform.position;
		Random.InitState ((int)Time.time);
	}

	


	public override int getListLength()
	{
		return entityList.Count;

	}

	//Removiing the game object add to your energy counter is energy value
	public override void removeFromList(GameObject go)
	{
		
		entityList.Remove(go);
		gameI.GetComponentInChildren<Text>().text = getListLength().ToString();

//		entityText.text = "Number of " + entityToSpawn.gameObject.name + ": " + entityList.Count;

	}


	public override void spawn(int i)
	{
		for (int j = 0; j < i; j++)
		{
			// Get random spawn location
			Vector3 randomSpawnPosition = flyingSpawnPositions[Random.Range(0,flyingSpawnPositions.Length)];

			// Spawn entity
			var entity = Instantiate(entityToSpawn, randomSpawnPosition, Quaternion.identity);
			entityList.Add(entity.gameObject);
			entity.transform.parent = gameObject.transform;
			entity.GetComponent<Creature>().setSpawn(this);
			
			//entityText.text = "Number of " + entityToSpawn.gameObject.name + ": " + entityList.Count;

			// Ensure we have a NavMeshAgent
			if (entity.GetComponent<NavMeshAgent> () == null) {
				Debug.LogError ("Attempting to spawn something that doesn't have a navmesh agent!");
			}

			// Set location again 
			// (this is a bug fix for entity moving to random position on instantiate)
			entity.GetComponent<NavMeshAgent> ().Warp (randomSpawnPosition); 
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