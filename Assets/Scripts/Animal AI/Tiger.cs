using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiger : Predator {

	void Start () {

		energyCost = 15;

		animalType = ANIMALTYPE.TIGER;
		homeType = global::Home.HomeType.Trees;

		prey.Add (ANIMALTYPE.DEER);

		health = 25;
		speed = 3.5f;
		homeRoamingDistance = 4f;

		attackDamage = 7;
		attackDistance = 1.3f;
		attackSpeed = 1f;
		sightDistance = 7.5f;

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
}
