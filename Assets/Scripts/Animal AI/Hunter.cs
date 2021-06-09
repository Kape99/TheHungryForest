using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : Human {

	// Update is called once per frame
	new protected void Update () {
		base.Update ();
	}

	// Use this for initialization
	void Start () {
		Awake ();

		energyCost = 4;

		health = 30;
		speed = 1.6f;
		attackDamage = 2;
		attackDistanceFauna = 20; // Large number ensures hunter will prefer to attack trees

		//Set Starting State
		ChangeState (CurrentState);
	}
}
