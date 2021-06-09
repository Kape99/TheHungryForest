using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : Plant
{
	public enum HomeType {Tree, NutTree, Rocks, Trees, Bush};
	public HomeType homeType;

	public int MaxInhabitants = 4;
	public int maxInhabitants;
	private List<GameObject> inhabitants = new List<GameObject> ();

	void Awake() {
		SetMaxInhabitants (MaxInhabitants);
	}

    public bool AttemptAddInhabitant(GameObject newInhabitant) {
		if (inhabitants.Count >= maxInhabitants) {
			return false;
		} else {
			inhabitants.Add (newInhabitant);
			return true;
		}
	}

	public void RemoveInhabitant(GameObject oldInhabitant) {
		inhabitants.Remove (oldInhabitant);
	}

	public int GetInhabitantsCount() {
		return inhabitants.Count;
	}

	public bool IsFull() {
		if (inhabitants.Count >= maxInhabitants) {
			return true;
		} else {
			return false;
		}
	}

	public Animal.ANIMALTYPE GetInhabitantsType() {
		return inhabitants [0].GetComponent<Animal> ().GetAnimalType ();
	}

	public void SetMaxInhabitants(int max) {
		maxInhabitants = max;
	}

}