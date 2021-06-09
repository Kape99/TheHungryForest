using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Groupie : MonoBehaviour {

	// Note: Set the game objects tag in unity to control which objects will group together

	public float groupRadius = 4f; // How far we check to see if a nearby rock can be grouped
	public int groupSize = 4; // Size of a group of rocks (including ourself)
	public int maxInhabitants = 4; // For the total group
	public Home.HomeType homeType;
	bool inGroup = false;
	string groupTag;
	GroupManager.GroupType groupType;

	// Use this for initialization
	void Start () {
		groupSize--; // Minus 1 to account for ourself not being in the group

		// Set tags
		if (groupTag == "Untagged") {
			Debug.LogError ("No group tag set for " + gameObject.name);
		} else {
			groupTag = this.transform.tag;
		}

		// Set group type
		SetupGroupType();

		// Ensure we cannot be used as home
		Home home = this.GetComponent<Home>();
		if (home != null) {
			home.SetMaxInhabitants (0);
		}

		StartCoroutine(AttemptToFormGroup ());

//		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//		sphere.transform.position = this.transform.position;
//		sphere.transform.localScale = new Vector3 (groupRadius, groupRadius, groupRadius);
	}

	public void SetupGroupType() {
		switch (groupTag) {
		case ("Rock"):
			groupType = GroupManager.GroupType.Rocks;
			break;
		case ("Flora"):
			groupType = GroupManager.GroupType.Trees;
			break;
		case ("Untagged"):
			break;
		default:
			Debug.LogError ("Undefined group type not handled");
			break;
		}
	}

	public void SetGroupTag(string tag) {
		groupTag = tag;
		this.transform.tag = tag;
		SetupGroupType ();
	}

	public void RenewAttemptToFormGroup() {
		StartCoroutine(AttemptToFormGroup ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator AttemptToFormGroup() {
		while (!inGroup) {
			// Find the closest groupies in range
			GameObject[] closestMembers = this.gameObject.GetObjectsInRangeWithTag (groupTag, groupRadius);

			closestMembers = FilterTagObjectsWithoutGroupieScript (closestMembers);

			// Get the closest within groupsize amount only
			if (closestMembers.Length > groupSize) { 
				closestMembers = this.gameObject.GetMostClosest (closestMembers, groupSize);
			} else if (closestMembers.Length < groupSize) {
				yield break;
			}

			// We now have a correctly sized group of rocks
			inGroup = true;

			// Check to see if any of the others in group have formed the grouping parent
			// if they have already figured out they are in a group, then we won't do anything
			// as they will ultimately form the group for us anyway
			foreach (GameObject groupMember in closestMembers) {
				if (groupMember.GetComponent<Groupie>().IsInGroup ()) {
					yield break;
				}
			}

			// Debug only to show connections
			foreach (GameObject member in closestMembers) {
				Debug.DrawLine (member.transform.transform.position, this.transform.position, Color.red, 60);
			}

			// Add my grouping object
			GameObject groupParent = new GameObject ();
			groupParent.name = groupTag + " Group";
			groupParent.tag = "Group";
			groupParent.transform.position = GetMidpointOfObjects (closestMembers);
			if (this.transform.parent != null) { // If we have a parent already
				groupParent.transform.SetParent (this.transform.parent.transform); // set group manager as my sibling (to keep hierachy)
			}
			this.transform.SetParent (groupParent.transform); // then set myself as group managers child

			// set everyone in the group as child
			foreach (GameObject member in closestMembers) {
				member.transform.SetParent (groupParent.transform);
			}

			// Create group array including myself
			List<GameObject> groupList = new List<GameObject>();
			foreach (GameObject member in closestMembers) {
				groupList.Add (member);
			}
			groupList.Add (this.gameObject);
			GameObject[] groupArray = groupList.ToArray ();

			// Setup the parent group manager
			GroupManager groupManager = groupParent.AddComponent(typeof(GroupManager)) as GroupManager;
			groupManager.Initialise (homeType, maxInhabitants); // Tell it the home type we want to be
			groupManager.SetGroupMembers (groupArray, groupSize, groupTag, groupType ,maxInhabitants);

			// Remove this script as its now in a group
			Destroy(this.GetComponent<Groupie>());

			yield return null;
		}
	}

	public bool IsInGroup() {
		return inGroup;
	}

	private GameObject[] RemoveThoseAlreadyInGroup(GameObject[] inputs) {
		List<GameObject> outputs = new List<GameObject> ();
		foreach (GameObject input in inputs) {
			if (input.GetComponent<Groupie> ().IsInGroup ()) {
				continue;
			}
			outputs.Add (input);
		}
		return outputs.ToArray ();
	}

	private GameObject[] FilterTagObjectsWithoutGroupieScript(GameObject[] inputs) {
		List<GameObject> output = new List<GameObject> ();
		foreach (GameObject input in inputs) {
			if (input.GetComponent<Groupie> () != null) { // If we have a groupie script
				output.Add (input);
			}
		}
		return output.ToArray();
	}

	private Vector3 GetMidpointOfObjects(GameObject[] inputs) {
		float sumX = 0;
		float sumY = 0;
		float sumZ = 0;

		foreach (GameObject input in inputs) {
			sumX += input.transform.position.x;
			sumY += input.transform.position.y;
			sumZ += input.transform.position.z;
		}

		float avgX = sumX / inputs.Length;
		float avgY = sumY / inputs.Length;
		float avgZ = sumZ / inputs.Length;

		return new Vector3 (avgX, avgY, avgZ);
	}
}
