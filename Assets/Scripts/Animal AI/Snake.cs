using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Predator {

	void Start () {

		energyCost = 2;

		animalType = ANIMALTYPE.SNAKE;
		homeType = global::Home.HomeType.Rocks;

		prey.Add (ANIMALTYPE.SQUIRREL);

		health = 10;
		speed = 2f;
		homeRoamingDistance = 4f;

		attackDamage = 3;
		attackDistance = 1.05f;
		attackSpeed = 0.8f;
		sightDistance = 5;

		SyncAttackDistanceAndStoppingDistance (attackDistance);

		//Set Starting State
		ChangeState (CurrentState);
	}

	new protected void Update () {
		base.Update ();
	}

	protected override IEnumerator Wander ()
	{
		// The snake doesnt not wander
		ChangeState (AISTATE.HUNT);
		yield return null;
	}

	protected override GameObject GetHomeMember ()
	{
		return GetHomeMemberFromGroup ();
	}

	protected override float GetHomeRoamingDistance ()
	{
		return homeRoamingDistance / 2;
	}

	protected override IEnumerator Hunt() {
		// We should only attempt to kill 1 animal during hunt
		sightDistance = sightDistance * 2; // See further for prey
		GameObject preyAnimal = GetPrey ();
		sightDistance = sightDistance / 2; // Return to normal sight


		while (CurrentState == AISTATE.HUNT) {
			if (CanSeeEnemy()) {
				ChangeState (AISTATE.ATTACK);
				yield break;
			}

			if (preyAnimal == null) { // If prey has been killed
				ChangeState(AISTATE.HOME);
				yield break;
			}

			bool successfulAttack = MoveToAndAttemptAttack(preyAnimal);
			if (successfulAttack) {
				yield return new WaitForSeconds (attackSpeed);
			}

			yield return null;
		}
	}
}
