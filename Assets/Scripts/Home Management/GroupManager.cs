using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour {

	private GameObject[] groupMembers = null;
	int groupSize;
	string groupTag;
	int saveMaxInhabitants;

	Home homeComponent;

	// Type
	public enum GroupType {Rocks, Trees};
	public GroupType groupType;

	// Use this for initialization
	public void Awake () {
		// Add home component as we are a home
		homeComponent = this.gameObject.AddComponent(typeof(Home)) as Home;
	}

	public void Initialise(Home.HomeType _homeType, int _maxInhabitants) {
		homeComponent.homeType = _homeType;
		homeComponent.SetMaxInhabitants(_maxInhabitants);
	}
	
	// Update is called once per frame
	void Update () {
		EnsureGroupIsValid ();
	}

	public void SetGroupMembers(GameObject[] members, int sizeOfGroup, string tagOfGroup, GroupType type, int maxInhabitants) {
		groupMembers = members;
		groupSize = sizeOfGroup + 1;
		groupTag = tagOfGroup;
		saveMaxInhabitants = maxInhabitants;

		foreach (GameObject member in members) {
			// Ensure no groupie scripts attached to members (so they cannot form groups)
			Component groupieScript = member.GetComponent<Groupie> ();
			if (groupieScript != null) { 
				Destroy (groupieScript);
			}

			// Ensure no creatures live in group members
			Home homeScript = member.GetComponent<Home>();
			if (homeScript != null) {
				homeScript.SetMaxInhabitants (0);
			}
		}

		groupType = type;
	}

	private void EnsureGroupIsValid() {
		if (this.transform.childCount < groupSize) { // If our actual group size is less than it's meant to be, destroy ourselves and restart the group
			foreach (GameObject member in groupMembers) {
				if (member == null)
					continue;

				if (member.transform.IsChildOf (this.transform)) { // Must be in the group BUT also as a child to this manager
					member.transform.SetParent (this.transform.parent); // Set as sibling to group manager
					if (member.GetComponent<Groupie> () == null) {
						Groupie groupie = member.AddComponent(typeof(Groupie)) as Groupie;
						groupie.SetGroupTag (groupTag);
						groupie.maxInhabitants = saveMaxInhabitants;
					}
				}
			}
			Destroy (this.gameObject);
		}
	}

	public GameObject[] GetGroupMembers() 
	{
		// TODO I THINK THERE IS A BETTER WAY TO DO THIS
		List<GameObject> existingHomeMembers = new List<GameObject> ();
		foreach (GameObject home in groupMembers) {
			if (home != null) {
				existingHomeMembers.Add (home);
			}
		}
		return existingHomeMembers.ToArray();
	}

	public GroupType GetGroupType() {
		return groupType;
	}
}
