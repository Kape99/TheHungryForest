using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using UnityEngine;

public class Placeable : MonoBehaviour
{
	public int placementCost;
	public float minimumPlacementDistance = 2;

	private bool placeable = false;
	private bool onWater = false;
	private SpriteRenderer sprite;

	void Awake() {
		sprite = this.gameObject.GetComponentInChildren<SpriteRenderer> ();
	}

	void Update() {
		// Construct a list of potential collision objects
		GameObject closestFlora = this.gameObject.GetClosestObjectWithTag ("Flora");
		GameObject closestRock = this.gameObject.GetClosestObjectWithTag ("Rock");

		if (closestFlora == null && closestRock == null) { // If there is nothing on the map yet
			placeable = true;
			return;
		}

		List<GameObject> closestObjects = new List<GameObject> ();
		if (closestFlora != null) {
			closestObjects.Add (closestFlora);
		}
		if (closestRock != null) {
			closestObjects.Add (closestRock);
		}

		// Get the closest
		GameObject closestObject = this.gameObject.GetClosestObjectIn2D(closestObjects.ToArray());

		// If its not within minimum distance, set sprite to dim, disallow placement
		if (this.transform.position.DistanceToIn2D (closestObject.transform.position) > minimumPlacementDistance) {
			placeable = true;
		} else {
			placeable = false;
		}

		UpdateSpriteDim ();
	}

	public bool IsPlaceable()
	{
		return placeable;
	}

	private void UpdateSpriteDim() {
		if (onWater) {
			placeable = false;
			DimSprite ();
		} else if (!onWater && placeable) {
			UndimSprite ();
		} else if (!onWater && !placeable) {
			DimSprite ();
		}
	}

	public void UpdateOnWater(bool onLand) {
		onWater = !onLand;
		UpdateSpriteDim ();
	}

	private void DimSprite() {
		sprite.color = new Color (1f, 1f, 1f, 0.5f);
	}

	private void UndimSprite() {
		sprite.color = new Color (1f, 1f, 1f, 1f);
	}
}
