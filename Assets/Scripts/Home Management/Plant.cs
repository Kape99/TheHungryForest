using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {

	public int health = 20;
	public int energyCost = 3;

	private Spawn spawn;

	void Update () {

		if (health <= 0) {
			Die ();
		}
	}

	private void Die() {
		if (spawn != null) {
			spawn.removeFromList(this.gameObject);
		}
		GameManager.instance.energyManager.AddEnergy (energyCost);
		GameManager.instance.DecreaseFlora ();
		GameObject.Destroy (this.gameObject);
	}

	public void RecieveDamage(int damage)
	{
		if (health > 0) { // Lets not beat a dead horse
			health -= damage;
		}
	}

	public void setSpawn(Spawn s)
	{
		spawn = s;
	}



	public void despawn()
	{
		spawn.removeFromList(this.gameObject);
		GameObject.Destroy (this.gameObject);
	}
}
