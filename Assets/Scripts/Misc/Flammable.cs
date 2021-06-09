using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammable : MonoBehaviour {

	int damage = 1;
	float damageSpeed = 1f; // Every 1 second

	bool isAlight = false;
	bool spawnedFlame = false;
	float distanceToAlightNearby = 3f; // metres
	public float chanceToAlightNearby = 0.7f; // 0-1
	float flameHeightAboveSprite = 0.5f; // Height at which the flame is raised above sprite, for visual reasons

	[SerializeField]
	Transform flamePrefab;

	Transform flames;

	public void AttemptToSetAlight(GameObject ignitor) {
		float distanceToIgnitor = this.transform.position.DistanceToIn2D (ignitor.transform.position);

		float chanceToIgnite = CalculateAlightChance (distanceToIgnitor);

		if (chanceToIgnite > 0) {
			if (chanceToIgnite >= Random.Range(0f,1.0f)) {
				SetAlight();
			}
		}
	}

	public bool IsOnFire() {
		return isAlight;
	}

	public void DestroyFire() {
		Destroy (flames.gameObject);
	}

	private void SetAlight() {

		if (spawnedFlame) {
			return; // Don't spawn more than 1 flame
		}

		// Find the sprite to set alight
		GameObject sprite = this.transform.Find ("Sprite").gameObject;

		if (sprite == null) {
			Debug.LogError ("Attempted to set alight object that doesn't have sprite to light");
			return;
		}

		// Spawn the flames, only once
		flames = Instantiate (flamePrefab, sprite.transform.position, Quaternion.identity);
		spawnedFlame = true;
		isAlight = true;

		// Set the flames as a child
		flames.transform.SetParent(sprite.transform);

		flames.transform.localPosition += new Vector3 (0, 0, -flameHeightAboveSprite);
		flames.transform.localEulerAngles = new Vector3 (0, -180, 180);

		// Apply the damage
		StartCoroutine(ApplyFireDamage());
	}

	private IEnumerator ApplyFireDamage() {
		while (true) {
			Creature creature = this.gameObject.GetComponent<Creature> ();
			if (creature == null) {
				Plant plant = this.gameObject.GetComponent<Plant> ();
				plant.RecieveDamage (damage);
			} else {
				creature.RecieveDamage (damage);
			}

			yield return new WaitForSeconds (damageSpeed);
		}
	}

	float CalculateAlightChance(float distance) {
		return chanceToAlightNearby - (chanceToAlightNearby / distanceToAlightNearby) * distance;
	}
}
