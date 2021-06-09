using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torchman : Human {

	// Update is called once per frame
	new protected void Update () {
		base.Update ();
	}

	// Use this for initialization
	void Start () {
		Awake ();

		energyCost = 15;

		health = 30;
		speed = 1.2f;
		attackDamage = 2;
		attackDistanceFauna = 1;
		attackSpeed = 0.5f;

		canAlight = true; // Can set things on fire

		//Set Starting State
		ChangeState (CurrentState);
	}
}
