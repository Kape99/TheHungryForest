using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumberjack : Human {

	// Update is called once per frame
	new protected void Update () {
		base.Update ();
	}

	// Use this for initialization
	void Start () {
		Awake ();

		energyCost = 4;

		health = 25;
		speed = 1.2f;
		attackDamage = 2;
		attackDistanceFauna = 1;

		//Set Starting State
		ChangeState (CurrentState);
	}
}
