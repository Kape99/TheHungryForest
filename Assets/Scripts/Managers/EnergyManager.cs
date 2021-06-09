using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour {

	public int startingEnergy = 39;
	private int energyPoints;

	public Text energyText;

	// Use this for initialization
	void Start () {
		energyPoints = startingEnergy;
		UpdateEnergyText ();
	}

	// PUBLIC FUNCTIONS -------------------------------------

	public void AddEnergy(int e)
	{
		energyPoints += e;
		UpdateEnergyText ();
	}

	public bool UseEnergy(int e)
	{
		if (e > energyPoints)
		{
			return false;
		}
		energyPoints -= e;
		UpdateEnergyText ();
		return true;
	}

	// PRIVATE FUNCTIONS ------------------------------------

	private void UpdateEnergyText() {
		energyText.text = /*"Energy: " + */energyPoints.ToString();
	}
}
