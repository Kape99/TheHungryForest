using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : Herbivore {

	protected List<Home.HomeType> foodTypes = new List<Home.HomeType>();

	protected int chanceToHunt = 50; // Value should be between 0 and 100.
	protected int wanderMinTime = 10;
	protected int wanderMaxTime = 20;

	private float eatingTime = 2;

	void Start () {

		energyCost = 8;

		animalType = ANIMALTYPE.DEER;
		homeType = global::Home.HomeType.Trees;
		foodTypes.Add (global::Home.HomeType.Bush);

		health = 20;
		speed = 4f;
		homeRoamingDistance = 4f;

		attackDamage = 2;
		attackDistance = 0.75f;
		attackSpeed = 0.8f;
		sightDistance = 5;

		fleeFromHumans = true; // Deers don't attack much

		SyncAttackDistanceAndStoppingDistance (attackDistance);

		//Set Starting State
		ChangeState (CurrentState);
	}

	new protected void Update () {
		base.Update ();
	}

	protected override GameObject GetHomeMember ()
	{
		return GetHomeMemberFromGroup ();
	}

	protected override float GetHomeRoamingDistance ()
	{
		return homeRoamingDistance / 2;
	}

	protected override IEnumerator Idle() 
	{
		float randomTimeTillWander = Time.time + Random.Range (wanderMinTime, wanderMaxTime);

		float randomTimeToWait = Time.time + Random.Range (idleMinTime, idleMaxTime);
		while (CurrentState == AISTATE.IDLE) {

			WatchForEnemiesAndPredators ();

			if (!IsHome ()) {
				ChangeState (AISTATE.HOME);
				yield break;
			}

			// If we've waited enough time to wander
			float currentTime = Time.time;
			if (currentTime > randomTimeTillWander) {
				Debug.Log ("Wandering...");
				ChangeState (AISTATE.WANDER);
				yield break;
			}

			MoveTo (RandomPointOnNavMeshNear(GetHomeMember(), GetHomeRoamingDistance()));
			while (Time.time < randomTimeToWait) {
				WatchForEnemiesAndPredators ();
				yield return null;
			}

			randomTimeToWait = Time.time + Random.Range (idleMinTime, idleMaxTime);
			yield return null;
		}
	}

	protected virtual IEnumerator Eat()
	{
		// We should only attempt to kill 1 animal during hunt
		GameObject foundFood = GetFood ();

		while (CurrentState == AISTATE.EAT) {
			if (CanSeeEnemy()) {
				ChangeState (AISTATE.FLEE);
				yield break;
			}

			if (foundFood == null) { // If prey has been killed
				ChangeState(AISTATE.HOME);
				yield break;
			}


			MoveTo (foundFood);

			float counter = 0;
			while (this.transform.position.DistanceToIn2D (foundFood.transform.position) <= attackDistance) {
				counter += Time.deltaTime;

				if (counter > eatingTime) {
					ChangeState (AISTATE.HOME);
				}
				yield return null;
			}

			yield return null;
		}
	}

	protected virtual IEnumerator Wander()
	{
		bool wandered = false;

		// Each time we wander we have a chance of going hunting too
		bool hunting;
		if (Random.Range (0, 101) < chanceToHunt) {
			hunting = true;
		} else {
			hunting = false;
		}

		while (CurrentState == AISTATE.WANDER) {

			if (CanSeeEnemy ()) {
				ChangeState (AISTATE.FLEE);
				yield break;
			}

			if (wandered == false) {
				var path = GetRandomFullMapWanderPath ();

				foreach (Vector3 node in path) {
					MoveTo (node);

					if (node == path [path.Length-1]) { // If last node
						wandered = true;
					}

					while (!PathComplete ()) {
						if (CanSeeEnemy ()) {
							ChangeState (AISTATE.FLEE);
							yield break;
						}

						if (hunting) {
							if (CanSeeFood ()) {
								Debug.Log ("Found food!");
								ChangeState (AISTATE.EAT);
							}
						}

						yield return null;
					}
				}
			}


			ChangeState (AISTATE.HOME);

			yield return null;
		}

	}

	protected override void ChangeState(AISTATE NewState)
	{
		StopAllCoroutines ();

		CurrentState = NewState;

		switch(NewState)
		{
		case AISTATE.IDLE:
			StartCoroutine (Idle());
			break;

		case AISTATE.HOME:
			StartCoroutine (Home());
			break;

		case AISTATE.ATTACK:
			Debug.LogError ("Deer went into attack mode, deers do not attack");
			StartCoroutine (Attack());
			break;

		case AISTATE.WANDER:
			StartCoroutine (Wander());
			break;

		case AISTATE.FLEE:
			StartCoroutine (Flee ());
			break;

		case AISTATE.EAT:
			StartCoroutine (Eat());
			break;
		}
	}

	protected bool CanSeeFood() {
		// Get objects in range of type fauna
		var floraInRange = this.gameObject.GetObjectsInRangeWithTag("Flora", sightDistance/2);

		if (floraInRange == null) {
			return false;
		}

		// Get closest object that is of prey animal type
		var closestFood = GetClosestFood(floraInRange);

		if (closestFood != null) {
			return true;
		} else {
			return false;
		}
	}
		
	protected GameObject GetFood() {
		// Get objects in range of type fauna
		var floraInRange = this.gameObject.GetObjectsInRangeWithTag("Flora", sightDistance/2);

		// Get closest object that is of prey animal type
		return GetClosestFood(floraInRange);
	}
		
	private GameObject GetClosestFood(GameObject[] foods) {
		GameObject closestFood = null;
		foreach (GameObject food in foods) {
			Home plantComponent = food.GetComponent<Home> ();

			if (plantComponent == null) {
				continue;
			}

			if (closestFood == null) {
				foreach (Home.HomeType plantType in foodTypes) {
					if (plantComponent.homeType == plantType) {
						closestFood = food;
					}
				}
			} else {
				float distanceToClosestFood = this.transform.position.DistanceToIn2D (closestFood.transform.position);
				float distanceToFood = this.transform.position.DistanceToIn2D (food.transform.position);

				if (distanceToFood < distanceToClosestFood) {
					foreach (Home.HomeType plantType in foodTypes) {
						if (plantComponent.homeType == plantType) {
							closestFood = food;
						}
					}
				}
			}
		}
		return closestFood;
	}
}
