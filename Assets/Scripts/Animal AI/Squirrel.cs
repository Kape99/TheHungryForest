using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squirrel : Herbivore {

	// Use this for initialization
	void Start () {

		energyCost = 1;

		animalType = ANIMALTYPE.SQUIRREL;
		homeType = global::Home.HomeType.NutTree;

		health = 8;
		speed = 1.5f;
		homeRoamingDistance = 3;

		attackDamage = 1;
		attackDistance = 0.75f;
		attackSpeed = 1;
		sightDistance = 7;
		energyCost = 1;
		
		SyncAttackDistanceAndStoppingDistance (attackDistance);

		//Set Starting State
		ChangeState (CurrentState);
	}

	new private void Update() {
		base.Update ();
	}
}
