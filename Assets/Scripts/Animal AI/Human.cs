using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Creature {

	protected float attackDistanceFauna; // Distance it prefers to attack animals at

	public enum AISTATE {ATTACK=0};
	public AISTATE CurrentState = AISTATE.ATTACK; // Kill kill kill

	private Animator animator;
	protected bool isAttacking = false;

	// Use this for initialization
	new protected void Awake () {
		base.Awake ();

		animator = this.transform.GetComponentInChildren<Animator> ();

		// Set defaults
		attackDistance = 1.2f;
		attackSpeed = 2;
	}

	// Update is called once per frame
	new protected void Update () {
		base.Update ();

		if (isMoving ()) {
			animator.SetBool ("isMoving", true);
		} else {
			animator.SetBool ("isMoving", false);
		}

		if (isAttacking) {
			animator.SetBool ("isAttacking", true);
		} else {
			animator.SetBool ("isAttacking", false);
		}
	}

	protected IEnumerator Attack()
	{
		while (CurrentState == AISTATE.ATTACK) {
			isAttacking = false;

			GameObject closestFlora = null;
			GameObject closestFauna = null;

			if (canAlight) {
				// Get the closest object not on fire. Doing this a cheap way that looks within 100m
				closestFauna = this.gameObject.GetClosestObjectIn2D(GetOnlyObjectsNotOnFire (this.gameObject.GetObjectsInRangeWithTag("Fauna", 100)));
				closestFlora = this.gameObject.GetClosestObjectIn2D(GetOnlyObjectsNotOnFire (this.gameObject.GetObjectsInRangeWithTag("Flora", 100)));
			} else {
				closestFlora = this.gameObject.GetClosestObjectWithTag("Flora");
				closestFauna = this.gameObject.GetClosestObjectWithTag("Fauna");
			}
			// Attack fauna if nearby otherwise attack the nearest flora where ever it is


			// Remove flora that is on fire


			if (closestFauna == null && closestFlora == null) {
				yield return null;
			} else {
				GameObject attackTarget;

				// Check if there are any animals on the map to attack
				float distanceToClosestFauna;
				if (closestFauna != null) { // Are animals
					distanceToClosestFauna = this.transform.position.DistanceToIn2D(closestFauna.transform.position); // Get the closest
				} else { // No animals
					distanceToClosestFauna = float.MaxValue; // Essentially always look for flora then
				}

				// If there is an animal nearby then prefer to attack it over flora
				if (distanceToClosestFauna <= attackDistanceFauna) {
					attackTarget = closestFauna;
				} else {
					if (closestFlora == null) { // But if there is no flora, then attack closest fauna
						attackTarget = closestFauna;
					} else {
						attackTarget = closestFlora;
					}
				}

				// Only attack if we are in the forest
				if (this.IsInForest ()) {
					bool successfulAttack = MoveToAndAttemptAttack (attackTarget);

					if (successfulAttack) {
						isAttacking = true;
						yield return new WaitForSeconds (attackSpeed);
					}
				} else { // otherwise move towards the target (and thus into the forest)
					MoveTo (attackTarget);
				}


			}
			yield return null;
		}
	}

	protected void ChangeState(AISTATE NewState)
	{
		StopAllCoroutines ();
		CurrentState = NewState;

		switch(NewState)
		{
		case AISTATE.ATTACK:
			StartCoroutine (Attack());
			break;
		}
	}

	private GameObject[] GetOnlyObjectsNotOnFire(GameObject[] inputs) {
		List<GameObject> outputs = new List<GameObject> ();
		foreach (GameObject input in inputs) {
			Flammable flammableComponent = input.GetComponent<Flammable> ();
			if (flammableComponent == null) {
				// If it's not flammable, it cannot be on fire
				outputs.Add(input);
			} else {
				if (!flammableComponent.IsOnFire ()) { // If not on fire
					outputs.Add (input); 
				}
			}
		}
		return outputs.ToArray();
	}
}
