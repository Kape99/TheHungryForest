using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	
public class TreeManager : MonoBehaviour
{
	public GameObject[] trees;
	public TreePlacement treePlacement;

	// Use this for initialization
	void Start ()
	{
		treePlacement = GetComponent<TreePlacement>();
	}

	void OnGUI()
	{
		for (int i = 0; i < trees.Length; i++)
		{
			if (GUI.Button(new Rect(Screen.width / 20, Screen.height / 15 + Screen.height / 12 * i, 100, 30), trees[i].name))
			{
				treePlacement.SetItem(trees[i]);
			}
				
		}
	}
}
