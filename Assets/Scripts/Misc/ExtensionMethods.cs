using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {

	/// <summary>
	/// Gets the closest object with tag.
	/// </summary>
	/// <returns>The closest object with tag.</returns>
	/// <param name="thisObject">This object.</param>
	/// <param name="tag">Tag.</param>
	public static GameObject GetClosestObjectWithTag(this GameObject thisObject, string tag) {
		GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag (tag);

		objectsWithTag = RemoveSelfFromArray (thisObject, objectsWithTag);

		// Only allow selection of creatures in the forest
		GameObject[] objectsWithTagInForest = CreaturesInForest(objectsWithTag);

		if (objectsWithTagInForest == null) {
			return null;
		}

		return GetClosestObjectIn2D(thisObject, objectsWithTagInForest);
	}

	/// <summary>
	/// Gets the objects in range of us with a certain tag.
	/// </summary>
	/// <returns>The objects in range with tag.</returns>
	/// <param name="thisObject">This object.</param>
	/// <param name="tag">Tag.</param>
	/// <param name="range">Range to look for objects.</param>
	public static GameObject[] GetObjectsInRangeWithTag(this GameObject thisObject, string tag, float range) {
		GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag (tag);
		objectsWithTag = RemoveSelfFromArray (thisObject, objectsWithTag);

		// Only allow selection of creatures in the forest
		GameObject[] objectsWithTagInForest = CreaturesInForest(objectsWithTag);

		if (objectsWithTagInForest == null) {
			return null;
		}

		return GetObjectsInRangeIn2D(thisObject, objectsWithTagInForest, range);
	}

	/// <summary>
	/// Removes us from the array
	/// </summary>
	/// <returns>The new array which we are no longer in.</returns>
	/// <param name="thisObject">This object.</param>
	/// <param name="inputs">Array to check.</param>
	private static GameObject[] RemoveSelfFromArray(GameObject thisObject, GameObject[] inputs) {
		List<GameObject> returnList = new List<GameObject> ();
		foreach (GameObject input in inputs) {
			if (input != thisObject.gameObject) {
				returnList.Add (input);
			}
		}
		return returnList.ToArray ();
	}

	/// <summary>
	/// Get's the distance between 2 objects in 2D/Topdown
	/// </summary>
	/// <returns>The to in2 d.</returns>
	/// <param name="thisVector3">This vector3.</param>
	/// <param name="end">End.</param>
	public static float DistanceToIn2D (this Vector3 thisVector3, Vector3 end) {

		Vector3 start2D = new Vector3 (thisVector3.x, 0f, thisVector3.z);
		Vector3 end2D = new Vector3 (end.x, 0f, end.z);

		//Debug.Log ("2D distance: " + Vector3.Distance (start2D, end2D));
		//Debug.Log("Destination: " + end.ToString());
		return Vector3.Distance(start2D, end2D);
	}

	/// <summary>
	/// Culls any creatures that aren't in the forest at the moment
	/// </summary>
	/// <returns>An array of only the creatures in the forest at the moment.</returns>
	/// <param name="creatures">An array of creatures.</param>
	private static GameObject[] CreaturesInForest(GameObject[] creatures) {
		List<GameObject> creaturesInForest = new List<GameObject>();
		foreach (GameObject creature in creatures) {
			var creatureComponent = creature.GetComponent<Creature> ();

			if (creatureComponent == null) {
				creaturesInForest.Add (creature);
			} else {
				if (creatureComponent.IsInForest ()) {
					creaturesInForest.Add (creature);
				}
			}
		}
			
		return creaturesInForest.ToArray();
	}

	/// <summary>
	/// Gets the closest object using distance measured from top down
	/// </summary>
	/// <returns>The closest object.</returns>
	/// <param name="thisObject">This object.</param>
	/// <param name="inputs">An array of objects.</param>
	public static GameObject GetClosestObjectIn2D(this GameObject thisObject, GameObject[] inputs) {
		GameObject closestObject = null;
		foreach (GameObject tagObject in inputs) {
			// If first object
			if (closestObject == null) {
				closestObject = tagObject;
			}

			float distanceToTagObject = thisObject.transform.position.DistanceToIn2D(tagObject.transform.position);
			float distanceToClosestObject = thisObject.transform.position.DistanceToIn2D(closestObject.transform.position);

			if (distanceToTagObject <= distanceToClosestObject) {
				closestObject = tagObject;
			}
		}

		return closestObject;
	}

	/// <summary>
	/// Gets all objects nearby in 2D topdown that are within a specified range
	/// </summary>
	/// <returns>The objects in range in2 d.</returns>
	/// <param name="thisObject">This object.</param>
	/// <param name="inputs">Inputs.</param>
	/// <param name="range">Range.</param>
	public static GameObject[] GetObjectsInRangeIn2D(GameObject thisObject, GameObject[] inputs, float range) {
		List<GameObject> objectsInRange = new List<GameObject> ();
		foreach (GameObject input in inputs) {
			float distanceToTagObject = thisObject.transform.position.DistanceToIn2D(input.transform.position);

			if (distanceToTagObject <= range) {
				objectsInRange.Add (input);
			}
		}

		return objectsInRange.ToArray();
	}

	/// <summary>
	/// Get the select number of closest objects in 2D topdown
	/// </summary>
	/// <returns>The most closest.</returns>
	/// <param name="thisObject">This object.</param>
	/// <param name="inputs">An array of objects.</param>
	/// <param name="amount">How many close objects to get.</param>
	public static GameObject[] GetMostClosest(this GameObject thisObject, GameObject[] inputs, int amount)
	{
		// Error check
		if (amount >= inputs.Length) {
			Debug.LogError ("Attempt to get 'closer' objects but there aren't enough objects to cull");
		}

		// Create a list to store the top amount of closest objects
		List<GameObject> closest = new List<GameObject> ();

		// NOTE: Could potentially do this with List.Sort()

		for (int i = 0; i < amount; i++) {
			// Store the closest object for this iteration
			GameObject closestObject = null;

			// Search through all inputted objects
			foreach (GameObject inputObject in inputs) {
				if (closest.Contains (inputObject)) {
					continue; // skip remainder of this iteration
				}

				if (closestObject == null) { // If we don't yet have a closest object
					closestObject = inputObject;
				} else { // If we do have a closest object
					float distToClosest = thisObject.transform.position.DistanceToIn2D (closestObject.transform.position);
					float distToInputObject = thisObject.transform.position.DistanceToIn2D (inputObject.transform.position);

					// If the object for this iteration is closer and we haven't already used it
					if (distToInputObject < distToClosest) {
						closestObject = inputObject;
					}
				}
			}
				
			closest.Add(closestObject); // Add the closest object for this iteration
		}

		return closest.ToArray ();
	}
}
