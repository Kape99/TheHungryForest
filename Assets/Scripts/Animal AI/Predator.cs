using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : Animal {

	protected List<ANIMALTYPE> prey = new List<ANIMALTYPE>(); // What the animal attacks when hunting

	[Range(0,100)]
	protected int chanceToHunt = 50; // Value should be between 0 and 100.
	protected float huntSpeedModifier = 1.5f;
	protected float huntDamageModifier = 3f;
	protected bool huntParametersSet = false;
	protected int wanderMinTime = 1;//10;
	protected int wanderMaxTime = 2;//20;

	new protected void Awake () {
		base.Awake ();
	}
	
	new protected void Update () {
		base.Update ();
	}

	// ------------------------STATE CODE-------------------------------

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
				ChangeState (AISTATE.ATTACK);
				yield break;
			}

			if (wandered == false) {
				var path = GetRandomFullMapWanderPath ();

				foreach (Vector3 node in path) {
					MoveTo (node);
					//Debug.Log (this.gameObject.name + " moving to node " + (System.Array.IndexOf (path, node) + 1) + " of " + path.Length);

					if (node == path [path.Length-1]) { // If last node
						wandered = true;
					}

					while (!PathComplete ()) {
						if (CanSeeEnemy ()) {
							ChangeState (AISTATE.ATTACK);
							yield break;
						}

						if (hunting) {
							if (CanSeePrey ()) {
								ChangeState (AISTATE.HUNT);
							}
						}

						yield return null;
					}
				}
			}

			Debug.Log(this.gameObject.name + " move to home");
			ChangeState (AISTATE.HOME);

			yield return null;
		}

	}

	protected virtual IEnumerator Hunt()
	{
		// We should only attempt to kill 1 animal during hunt
		GameObject preyAnimal = GetPrey ();

		while (CurrentState == AISTATE.HUNT) {
			if (CanSeeEnemy()) {
				ChangeState (AISTATE.ATTACK);
				yield break;
			}

			if (preyAnimal == null) { // If prey has been killed
				ChangeState(AISTATE.HOME);
				yield break;
			}

			//Debug.Log (this.gameObject.transform.position.DistanceToIn2D (preyAnimal.transform.position) + " < " + attackDistance);
			bool successfulAttack = MoveToAndAttemptAttack(preyAnimal);
			if (successfulAttack) {
				yield return new WaitForSeconds (attackSpeed);
			}

			yield return null;
		}
	}

	protected override void ChangeState(AISTATE NewState)
	{
		StopAllCoroutines ();

		if (CurrentState == AISTATE.HUNT) { // Hunt mode off
			UnsetHuntParameters ();
		}

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
			StartCoroutine (Attack());
			break;

		case AISTATE.WANDER:
			StartCoroutine (Wander());
			break;

		case AISTATE.FLEE:
			StartCoroutine (Flee ());
			break;

		case AISTATE.ONFIRE:
			StartCoroutine (OnFire ());
			break;

		case AISTATE.HUNT:
			SetHuntParameters (); // Hunt mode on
			StartCoroutine (Hunt());
			break;
		}
	}

	// ---------------------------------------------------------------

	protected bool CanSeePrey() {
		// Get objects in range of type fauna
		var faunaInRange = this.gameObject.GetObjectsInRangeWithTag("Fauna", sightDistance);

		if (faunaInRange == null) {
			return false;
		}

		// Get closest object that is of prey animal type
		var closestPrey = GetClosestPrey(faunaInRange);

		if (closestPrey != null) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Check if the predator hunts this animal type
	/// </summary>
	/// <returns><c>true</c> if this instance is prey the specified animal; otherwise, <c>false</c>.</returns>
	/// <param name="animal">Animal.</param>
	public bool IsPrey(ANIMALTYPE animal) {
		return prey.Contains (animal);
	}

	/// <summary>
	/// Gets the closest prey within sight distance of animal
	/// </summary>
	/// <returns>The prey.</returns>
	protected GameObject GetPrey() {
		// Get objects in range of type fauna
		var faunaInRange = this.gameObject.GetObjectsInRangeWithTag("Fauna", sightDistance);

		// Get closest object that is of prey animal type
		return GetClosestPrey(faunaInRange);
	}

	/// <summary>
	/// Get's the closest prey from a given list of animals
	/// </summary>
	/// <returns>The closest prey.</returns>
	/// <param name="animals">Animal lists.</param>
	private GameObject GetClosestPrey(GameObject[] animals) {
		GameObject closestPrey = null;
		foreach (GameObject animal in animals) {
			Animal animalComponent = animal.GetComponent<Animal> ();

			if (animalComponent == null) {
				Debug.LogError ("Animal in list of animals does not have animal component");
			}

			if (closestPrey == null) {
				foreach (Animal.ANIMALTYPE preyType in prey) {
					if (animalComponent.GetAnimalType() == preyType) {
						closestPrey = animal;
					}
				}
			} else {
				float distanceToClosestPrey = this.transform.position.DistanceToIn2D (closestPrey.transform.position);
				float distanceToAnimal = this.transform.position.DistanceToIn2D (animal.transform.position);

				if (distanceToAnimal < distanceToClosestPrey) {
					foreach (Animal.ANIMALTYPE preyType in prey) {
						if (animalComponent.GetAnimalType() == preyType) {
							closestPrey = animal;
						}
					}
				}
			}
		}
		return closestPrey;
	}

	public bool IsHunting() {
		return huntParametersSet;
	}

	/// <summary>
	/// Put's the animal into hunt mode where it is especially deadly
	/// </summary>
	protected void SetHuntParameters() {
		if (!huntParametersSet) {
			speed = speed * huntSpeedModifier;
			attackDamage = Mathf.CeilToInt(attackDamage * huntDamageModifier);
			huntParametersSet = true;
		} else {
			Debug.LogError ("Attempt to set hunt parameter modifiers more than once");
		}
	}

	/// <summary>
	/// Turns off hunt mode and resumes regularly animal properties
	/// </summary>
	protected void UnsetHuntParameters() {
		if (huntParametersSet) {
			speed = speed / huntSpeedModifier;
			attackDamage = Mathf.CeilToInt(attackDamage / huntDamageModifier);
			huntParametersSet = false;
		} else {
			Debug.LogError ("Attempt to unset hunt parameter modifiers more than once");
		}
	}
}
