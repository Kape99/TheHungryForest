using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hawk : Predator {

	private GameObject shadow;

	// Use this for initialization
	void Start () {

		energyCost = 5;

		animalType = ANIMALTYPE.HAWK;
		homeType = global::Home.HomeType.Tree;

		prey.Add (ANIMALTYPE.SQUIRREL);
		prey.Add (ANIMALTYPE.SNAKE);

		health = 15;
		speed = 1.7f;
		homeRoamingDistance = 1.25f;

		attackDamage = 3;
		attackDistance = 0.75f;
		attackSpeed = 1;
		sightDistance = 9;

		SyncAttackDistanceAndStoppingDistance (attackDistance);

		//Set Starting State
		ChangeState (CurrentState);

		shadow = this.gameObject.transform.Find ("Sprite/Shadow").gameObject;
	}

	new private void Update() {
		base.Update ();

		float speed = getVelocity ();

		if (speed < 1) {
			shadow.transform.position = Vector3.Lerp (shadow.transform.position, this.transform.position - Vector3.down * -2, Time.deltaTime);
		} else {
			shadow.transform.position = Vector3.Lerp (shadow.transform.position, this.transform.position, Time.deltaTime);
		}

	}

}
