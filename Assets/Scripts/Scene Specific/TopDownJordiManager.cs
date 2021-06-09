using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using System;

public class TopDownJordiManager : MonoBehaviour {

	[SerializeField] // Makes the field available to edit in editor
	Transform squirrel;
	[SerializeField]
	Transform lumberjack;
	[SerializeField]
	Transform hawk;
	[SerializeField]
	Transform snake;
	[SerializeField]
	Transform deer;
	[SerializeField]
	Transform tiger;
	[SerializeField]
	Transform hunter;
	[SerializeField]
	Transform torchman;

	private Vector3 spawnPosition;
	private Vector3[] flyingSpawnPositions = new Vector3[4];

	// Use this for initialization
	void Start () {
		// Find variables
		spawnPosition = GameObject.Find("SpawnPlane").transform.position;
		flyingSpawnPositions[0] = GameObject.Find ("Sky/SkySpawnNorth").transform.position;
		flyingSpawnPositions[1] = GameObject.Find ("Sky/SkySpawnEast").transform.position;
		flyingSpawnPositions[2] = GameObject.Find ("Sky/SkySpawnSouth").transform.position;
		flyingSpawnPositions[3] = GameObject.Find ("Sky/SkySpawnWest").transform.position;

		// Spawn stuff
		//StartCoroutine(DelayedMultiSpawn(squirrel, 5, 1));
	}

	void Update()
	{
		if (Input.GetKeyDown ("l"))
			SpawnEntity (lumberjack);
		else if (Input.GetKeyDown ("h"))
			SpawnFlyingEntity (hawk);
		else if (Input.GetKeyDown ("s"))
			SpawnEntity (squirrel);
		else if (Input.GetKeyDown ("n"))
			SpawnEntity (snake);
		else if (Input.GetKeyDown ("d"))
			SpawnEntity (deer);
		else if (Input.GetKeyDown ("u"))
			SpawnEntity (hunter);
		else if (Input.GetKeyDown ("t"))
			SpawnEntity (torchman);
		else if (Input.GetKeyDown ("i"))
			SpawnEntity (tiger);

		UnityEngine.Random.InitState ((int)Time.time);
	}
		
	public void SpawnEntity(Transform entityToSpawn) {
		// Spawn the entity
		var entity = Instantiate (entityToSpawn, spawnPosition, Quaternion.identity);

		// Ensure we have a NavMeshAgent
		if (entity.GetComponent<NavMeshAgent> () == null) {
			Debug.LogError ("Attempting to spawn something that doesn't have a navmesh agent!");
		}

		// Set location again 
		// (this is a bug fix for entity moving to random position on instantiate)
		entity.GetComponent<NavMeshAgent> ().Warp (spawnPosition); 
	}

	void SpawnFlyingEntity(Transform entityToSpawn) {
		// Get random spawn location
		Vector3 randomSpawnPosition = flyingSpawnPositions[Random.Range(0,flyingSpawnPositions.Length)];

		// Spawn entity
		var entity = Instantiate(entityToSpawn, randomSpawnPosition, Quaternion.identity);

		// Ensure we have a NavMeshAgent
		if (entity.GetComponent<NavMeshAgent> () == null) {
			Debug.LogError ("Attempting to spawn something that doesn't have a navmesh agent!");
		}

		// Set location again 
		// (this is a bug fix for entity moving to random position on instantiate)
		entity.GetComponent<NavMeshAgent> ().Warp (randomSpawnPosition); 
	}
		
	IEnumerator DelayedMultiSpawn(Transform entityToSpawn, int amount, float timeBetweenSpawns)
	{
		for (int i = 0; i < amount; i++) {
			SpawnEntity (entityToSpawn);
			yield return new WaitForSeconds(timeBetweenSpawns);
		}

		yield break;
	}

	void MoveCreaturesWithTagTo(string tag, GameObject destination) {
		foreach (GameObject entity in GameObject.FindGameObjectsWithTag(tag))
		{
			Creature creature = entity.GetComponent<Creature>();
			creature.MoveTo (destination);
		}
	}

}
