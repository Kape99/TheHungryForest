using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : Creature {

	public enum AISTATE {IDLE,ATTACK,HOME,WANDER,HUNT,FLEE,EAT, ONFIRE};
	public AISTATE CurrentState = AISTATE.HOME;

	public enum ANIMALTYPE {SQUIRREL,HAWK,SNAKE,DEER,TIGER};
	protected ANIMALTYPE animalType;

	protected int idleMinTime = 5;
	protected int idleMaxTime = 10;

	protected Home.HomeType homeType;
	protected GameObject home;
	protected float homeRoamingDistance;

	private GameObject[] quadrants = new GameObject[4+1]; // 4 quadrants but +1 so we can access from 1-4 instead of 0-3

	protected bool fleeFromHumans = false;

	protected float sightDistance; // How far we can see an enemy or prey

	private Animator animator;

	private Flammable flammableComponent;
	private bool onFire = false;

	new protected void Awake () {
		base.Awake ();

		// On our birth, let the game manager know we are alive
		//GameManager.instance.IncreaseFauna();

		for (int i = 1; i <= 4; i++) {
			quadrants [i] = GameObject.Find ("QuadrantPoints/Quadrant" + i);
		}

		animator = this.transform.GetComponentInChildren<Animator> ();

		if (animator == null) {
			Debug.LogError ("No animator in children of " + gameObject.name);
		}

		flammableComponent = this.GetComponent<Flammable> ();
	}

	new protected void Update () {
		base.Update ();

		if (isMoving ()) {
			animator.SetBool ("isMoving", true);
		} else {
			animator.SetBool ("isMoving", false);
		}

		// Check if we are alight
		if (flammableComponent != null) {
			onFire = flammableComponent.IsOnFire ();
			if (onFire) {
				ChangeState (AISTATE.ONFIRE);
			}
		}
	}

	// ------------------------STATE CODE-------------------------------

	protected virtual IEnumerator Idle() 
	{
		float randomTimeToWait = Time.time + Random.Range (idleMinTime, idleMaxTime);

		while (CurrentState == AISTATE.IDLE) {

			WatchForEnemiesAndPredators ();

			if (!IsHome ()) {
				ChangeState (AISTATE.HOME);
				yield break;
			}
				
			MoveTo (RandomPointOnNavMeshNear(home, homeRoamingDistance));
			while (Time.time < randomTimeToWait) {
				WatchForEnemiesAndPredators ();
				yield return null;
			}

			// Update the wait time again
			randomTimeToWait = Time.time + Random.Range (idleMinTime, idleMaxTime);
			yield return null;
		}
	}

	protected IEnumerator Home()
	{
		while (CurrentState == AISTATE.HOME) {

			animator.SetBool ("isAttacking", false);

			WatchForEnemiesAndPredators ();

			if (home == null) {
				home = FindHome (homeType);

				if (home == null) {
					Debug.Log ("No home for " + this.gameObject.name);
					yield return null;
				} 
			}

			if (!IsHome ()) {
				MoveTo (GetHomeMember());
			} else {
				ChangeState (AISTATE.IDLE);
			}
			yield return null;
		}

 		}

	protected IEnumerator Attack()
	{
		while (CurrentState == AISTATE.ATTACK) {
			if (!CanSeeEnemy()) {
				ChangeState (AISTATE.HOME);
				yield return null;

			} else {
				GameObject closestEnemy = this.gameObject.GetClosestObjectWithTag ("Enemy");

				bool successfulAttack = MoveToAndAttemptAttack (closestEnemy);

				if (successfulAttack) {
					animator.SetBool ("isAttacking", true);
					yield return new WaitForSeconds (attackSpeed);
				}
			}

			yield return null;
		}
	}

	protected IEnumerator Flee()
	{
		while (CurrentState == AISTATE.FLEE) {

			if (!CanSeePredator ()) {
				ChangeState (AISTATE.HOME);
			}

			GameObject predator = GetClosestPredator ();

			if (predator != null) {
				Vector3 predator2DPos = new Vector3 (predator.transform.position.x, this.transform.position.y, predator.transform.position.z);
				Vector3 oppositeDirection = (this.transform.position - predator2DPos).normalized;

				MoveTo (this.gameObject.transform.position + oppositeDirection * 2);
			} 
			yield return null;
		}
	}

	protected IEnumerator OnFire()
	{
		while (CurrentState == AISTATE.ONFIRE) {

			Vector3 randomPoint = RandomPointOnNavMeshNear (this.gameObject, sightDistance);
			MoveTo (randomPoint);

			// TODO: Make animal search for water if nearby
			while (!PathComplete ()) {
				if(IsInWater ()) {
					// Reset fire component
					float tempChance = flammableComponent.chanceToAlightNearby;
					flammableComponent.DestroyFire ();
					Destroy (flammableComponent);
					Flammable newFlammableComp = this.gameObject.AddComponent(typeof(Flammable)) as Flammable;
					newFlammableComp.chanceToAlightNearby = tempChance;

					ChangeState (AISTATE.HOME);
				}
				yield return null;
			}
			yield return null;
		}
	}

	protected virtual void ChangeState(AISTATE NewState)
	{
		StopAllCoroutines ();
		CurrentState = NewState;

		switch (NewState) {
		case AISTATE.IDLE:
			StartCoroutine (Idle ());
			break;

		case AISTATE.HOME:
			StartCoroutine (Home ());
			break;

		case AISTATE.ATTACK:
			StartCoroutine (Attack ());
			break;

		case AISTATE.FLEE:
			StartCoroutine (Flee ());
			break;

		case AISTATE.ONFIRE:
			StartCoroutine (OnFire ());
			break;
		}
	}

	// ----------------------------------------------------------------

	// --------------------------FUNCTIONS-----------------------------

	protected virtual GameObject GetHomeMember() {
		return home;
	}

	protected virtual float GetHomeRoamingDistance() {
		return homeRoamingDistance;
	}

	protected GameObject GetHomeMemberFromGroup() {
		if (home == null) {
			return null;
		}

		// Get a random gameobject from the home group of rocks
		GameObject[] homeMembers = home.GetComponent<GroupManager> ().GetGroupMembers ();
		GameObject randomHomeMember = homeMembers [Random.Range (0, homeMembers.Length)];

		// All rocks have a navigation object, so use that as its more accurate for the navmesh agent
		return randomHomeMember.transform.Find ("Navigation Object").gameObject;
	}

	protected void WatchForEnemiesAndPredators() {
		if (CanSeeEnemy ()) {
			ChangeState (AISTATE.ATTACK);
		}

		if (CanSeePredator ()) {
			ChangeState (AISTATE.FLEE);
		}
	}

	protected bool IsHome() 
	{
		if (home == null || !IsInForest()) {
			return false;
		}
		float distanceToHome =  this.transform.position.DistanceToIn2D(home.transform.position);
		if (distanceToHome <= homeRoamingDistance) {
			return true;
		} else {
			return false;
		}
	}

	public GameObject GetHome() {
		return home;
	}

	/// <summary>
	/// Get which tag the given home uses
	/// </summary>
	/// <returns>The type of the home.</returns>
	/// <param name="type">Type.</param>
	public string GetHomeTag(Home.HomeType type) {
		switch (type) {
		case global::Home.HomeType.Tree:
			return "Flora";
		case global::Home.HomeType.NutTree:
			return "Flora";
		case global::Home.HomeType.Rocks:
			return "Group";
		case global::Home.HomeType.Trees:
			return "Group";
		default:
			Debug.LogError ("Attempt to get home tag of undefined home type");
			return null;
		}
	}

	public ANIMALTYPE GetAnimalType() {
		return animalType;
	}

	/// <summary>
	/// Find a home with the given plant type, where
	/// the home has available inhabitant space.
	/// </summary>
	/// <returns>The home.</returns>
	/// <param name="plantHomeType">The animals plant home type.</param>
	protected GameObject FindHome(Home.HomeType homeType) {
		// Get flora tags
		GameObject[] homes = GameObject.FindGameObjectsWithTag (GetHomeTag(homeType));

		// Get plant script with animal home type only or return null if none
		List<GameObject> possibleHomes = new List<GameObject>();
		foreach (GameObject home in homes) {
			var homeComponent = home.GetComponent<Home> ();

			if (homeComponent == null) {
				continue;
			}

			if (homeComponent.homeType == homeType) {
				// If there is room for one more inhabitant
				if (homeComponent.IsFull() == false) {
					if (homeComponent.GetInhabitantsCount() <= 0) {
						possibleHomes.Add (home);
					} else {
						if (homeComponent.GetInhabitantsType () == animalType) {
							possibleHomes.Add (home);
						}
					}
				}
			}
		}

		if (possibleHomes.Count == 0) {
			return null;
		} else {
			// Find nearest possible home
			var newHome = gameObject.GetClosestObjectIn2D(possibleHomes.ToArray());
			bool joinHome = newHome.GetComponent<Home> ().AttemptAddInhabitant (this.gameObject);

			if (joinHome) {
				return newHome;
			} else {
				Debug.LogError ("Attempt to join home failed");
				return null;
			}
		}

	}
		
	protected Vector3 RandomPointOnNavMeshNear(GameObject point, float Radius)
	{
		Vector3 Point = point.transform.position + Random.insideUnitSphere * Radius;
		NavMeshHit NH;
		NavMesh.SamplePosition (Point, out NH, Radius, 1 << NavMesh.GetAreaFromName("Walkable"));
		//Debug.Log ("The mask cost is: " + NavMesh.GetAreaCost (NH.mask));
		return NH.position;
	}

	private Vector3 RandomPointOnWaterNear(GameObject point, float radius) {
		Vector3 Point = point.transform.position + Random.insideUnitSphere * radius;
		NavMeshHit NH;
		NavMesh.SamplePosition (Point, out NH, radius, 1 << NavMesh.GetAreaFromName("Water"));
		//Debug.Log ("The mask cost is: " + NavMesh.GetAreaCost (NH.mask));
		return NH.position;
	}

	protected bool CanSeeEnemy() {
		GameObject closestEnemy = this.gameObject.GetClosestObjectWithTag("Enemy");

		if (closestEnemy == null) {
			return false;
		} else {
			float distanceToClosestEnemy = this.transform.position.DistanceToIn2D(closestEnemy.transform.position);

			if (distanceToClosestEnemy <= sightDistance) {
				return true;
			} else {
				return false;
			}
		}
	}

	/// <summary>
	/// If we can see a predator nearby who is hunting
	/// </summary>
	/// <returns><c>true</c> if this instance can see predator; otherwise, <c>false</c>.</returns>
	protected bool CanSeePredator() {
		GameObject[] closestPredators = GetNearbyPredators ();

		return (closestPredators.Length > 0); // If more than 0 then true
	}

	protected GameObject GetClosestPredator() {
		return this.gameObject.GetClosestObjectIn2D (GetNearbyPredators ());
	}

	/// <summary>
	/// Gets nearby predators that are hunting
	/// </summary>
	/// <returns>The nearby predators.</returns>
	private GameObject[] GetNearbyPredators() {
		// Look at the nearby animals
		GameObject[] nearbyAnimals = this.gameObject.GetObjectsInRangeWithTag ("Fauna", sightDistance/2); // Don't see as far

		// Get a list of only predators that hunt us
		List<GameObject> nearbyPredators = new List<GameObject>();
		foreach (GameObject animal in nearbyAnimals) {
			Predator predator = animal.GetComponent<Predator> ();
			if (predator != null) {
				if (predator.IsPrey(animalType) && predator.IsHunting()) {
					nearbyPredators.Add(animal);
				}
			}
		}

		if (fleeFromHumans) {
			GameObject[] nearbyHumans = this.gameObject.GetObjectsInRangeWithTag ("Enemy", sightDistance);

			foreach (GameObject human in nearbyHumans) {
				nearbyPredators.Add (human);
			}
		}

		return nearbyPredators.ToArray ();
	}

	// WANDERING -------
	/// <summary>
	/// Get's a "wander path" around each quadrant of the map that the creature
	/// isn't currently in. E.g. if calling this function and the creature is in quadrant 2,
	/// it may return a sequence of random points in quadrant 3, 4, and 1.
	/// </summary>
	/// <returns>The random wander path of all other quadrants of the map.</returns>
	protected Vector3[] GetRandomFullMapWanderPath() {
		List<Vector3> returnVectors = new List<Vector3>();

		int startingQuadrant = GetCurrentQuadrant();
		int direction = Random.Range (0, 2); // between 0 and 1

		if (direction == 1) { // Go clockwise
			for (int i = 1; i < 4; i++) { // Only use 3 quadrants
				int quadrantNum = (startingQuadrant + i) % (4+1);
				if (quadrantNum == 0) {
					startingQuadrant = startingQuadrant + 1;
					quadrantNum = 1;
				}
				returnVectors.Add (GetRandomQuadrantPosition (quadrantNum));
			}
		} else { // Go counter clockwise
			for (int i = -1; i > -4; i--) { // Only use 3 quadrants
				int quadrantNum = Mathf.Abs((startingQuadrant + i) % (4+1));
				if (quadrantNum == 0) {
					startingQuadrant = startingQuadrant + 4;
					quadrantNum = 4;
				}
				returnVectors.Add (GetRandomQuadrantPosition (quadrantNum));
			}
		}

		foreach (Vector3 vector in returnVectors) {
			//Debug.Log (vector.ToString ());
		}
		//Debug.Log (this.gameObject.name + " position: " + this.gameObject.transform.position.ToString ());

		return returnVectors.ToArray();
	}

	/// <summary>
	/// Get a random position in a given quadrant
	/// </summary>
	/// <returns>The random quadrant position.</returns>
	/// <param name="quadrantNum">Quadrant number.</param>
	private Vector3 GetRandomQuadrantPosition(int quadrantNum) {
		if (quadrantNum == 0) {
			Debug.LogError ("Attempt to access the zero quadrant in quadrant array");
		}

		return RandomPointOnNavMeshNear (quadrants[quadrantNum], 5f);
	}

	/// <summary>
	/// Find which quadrant the creature is currently in
	/// </summary>
	/// <returns>The current quadrant.</returns>
	protected int GetCurrentQuadrant() {
		float prevDist = float.MaxValue;
		int currentQuadrant = 0;

		for (int i = 1; i < quadrants.Length; i++)
		{
			float dist = quadrants[i].transform.position.DistanceToIn2D (this.transform.position);
			if (dist < prevDist) {
				prevDist = dist;
				currentQuadrant = i;
			}
		}

		return currentQuadrant;
	}
}
