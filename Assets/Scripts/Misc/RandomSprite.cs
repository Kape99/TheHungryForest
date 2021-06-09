using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour {

	public bool RandomSprites = true;
	public Sprite[] sprites;
	public bool RandomRotation = true;

	// Use this for initialization
	void Start () {

		if (RandomSprites) {
			SpriteRenderer renderer = this.gameObject.GetComponent<SpriteRenderer> ();

			if (renderer == null) {
				Debug.LogError("Random sprite attempted to get renderered but there was none on + " + this.gameObject.name);
			}

			renderer.sprite = sprites[Random.Range(0,sprites.Length)];
		}
			
		if (RandomRotation) {
			Vector3 euler = transform.eulerAngles;
			euler.y = Random.Range(0f, 360f);
			transform.eulerAngles = euler;
		}
	}
}
