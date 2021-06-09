using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

	[SerializeField]
	Transform lumberjack;
	[SerializeField]
	Transform hunter;
	[SerializeField]
	Transform torchman;

	private Vector3 spawnPosition;
	private bool haveSpawnedEnemies = false;

	List<GameObject> lumberjacksList = new List<GameObject>();
	List<GameObject> huntersList = new List<GameObject>();
	List<GameObject> torchmenList = new List<GameObject>();

	// Info regarding what enemies spawn and when
	int[,] daySpawnInfo = new int[20,3] {
		// lumberjack, hunter, torchman
		{1,0,0},
		{2,0,0},
		{0,1,0},
		{0,2,0},
		{1,1,0}, // 5
		{1,2,0},
		{2,2,0},
		{3,2,0},
		{3,3,0},
		{0,0,1}, // 10
		{4,3,0},
		{3,3,1},
		{4,3,1},
		{4,4,0},
		{3,3,2}, // 15
		{4,3,2},
		{4,3,3},
		{4,4,4},
		{5,4,5},
		{6,5,6}  // 20
	};

	// Use this for initialization
	void Start () {
		spawnPosition = GameObject.Find("SpawnPlane").transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateLists (); 
	}

	// PUBLIC FUNCTIONS -------------------------------------

	public void SpawnWave(int day) {
		haveSpawnedEnemies = false;
		int lumberjackCount = daySpawnInfo [day - 1, 0];
		int hunterCount = daySpawnInfo [day - 1, 1];
		int torchmenCount = daySpawnInfo [day - 1, 2];
		spawnEnemies (lumberjackCount, hunterCount, torchmenCount);
	}

	public bool AllSpawnedEnemiesDead() {
		if (!haveSpawnedEnemies) {
			return false;
		} else {
			return (lumberjacksList.Count <= 0 && huntersList.Count <= 0 && torchmenList.Count <= 0);
		}
	}

	public int GetNumberOfLevels() {
		return daySpawnInfo.Length;
	}

	// PRIVATE FUNCTIONS ------------------------------------------

	private void spawnEnemies(int lumberjacks, int hunters, int torchmen) {
		for (int i = 0; i < lumberjacks; i++) {
			SpawnEntity (lumberjack);
		}

		for (int i = 0; i < hunters; i++) {
			SpawnEntity (hunter);
		}

		for (int i = 0; i < torchmen; i++) {
			SpawnEntity (torchman);
		}

		haveSpawnedEnemies = true;
	}

	private void SpawnEntity(Transform entityToSpawn) {
		// Spawn the entity
		var entity = Instantiate (entityToSpawn, spawnPosition, Quaternion.identity);

		// Add spawned entity to its necessary list
		if (entityToSpawn == lumberjack) {
			lumberjacksList.Add (entity.gameObject);
		} else if (entityToSpawn == hunter) {
			huntersList.Add (entity.gameObject);
		} else if (entityToSpawn == torchman) {
			torchmenList.Add(entity.gameObject);
		} else {
			Debug.LogError ("Attempt to spawn non identified entity");
		}

		// Set location again 
		// (this is a bug fix for entity moving to random position on instantiate)
		entity.GetComponent<Creature>().Warp(spawnPosition);
	}

	// Check if any entities have died, if so update the list by removing dead entities
	private void UpdateLists() {
		if (lumberjacksList.Count > 0) {
			foreach (GameObject entity in lumberjacksList) {
				if (entity == null) {
					lumberjacksList.Remove (entity);
				}
			}
		}

		if (huntersList.Count > 0) {
			foreach (GameObject entity in huntersList) {
				if (entity == null) {
					huntersList.Remove (entity);
				}
			}
		}
			
		if (torchmenList.Count > 0) {
			foreach (GameObject entity in torchmenList) {
				if (entity == null) {
					torchmenList.Remove (entity);
				}
			}
		}
	}
}
