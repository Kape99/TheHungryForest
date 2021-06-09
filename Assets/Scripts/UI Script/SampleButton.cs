using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleButton : MonoBehaviour
{

	public Spawn Spawn;
	private EnergyManager sm;
	private Text cost;
	private Image icon;

	public void click()
	{
		if (sm.GetComponent<EnergyManager>().UseEnergy(Spawn.GetComponent<Spawn>().getCost()))
		{
			Spawn.spawn(1);
		}

	
	}
	
	// Use this for initialization
	void Start ()
	{
		this.GetComponent<Image>().sprite = Spawn.GetsSprite();
		this.GetComponentInChildren<Text>().text = Spawn.getCost().ToString();
		sm = GameObject.FindObjectOfType<EnergyManager>().GetComponent<EnergyManager>();

	}
	

}
