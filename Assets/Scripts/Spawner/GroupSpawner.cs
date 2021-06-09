using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupSpawner : Spawn {

	public GameObject entityToSpawn;

	public List<GameObject> entityList;

	public TreePlacement treePlacement;
	
	public enum GroupType {Rocks, Trees};

	public GroupType type;

	public Sprite Sprite;

	private void Start()
	{
//		gameI.GetComponentInChildren<RawImage>().texture = iconUI;
//		gameI.GetComponentInChildren<Text>().text = getListLength().ToString();
	}

	public override int getListLength()
	{
		return entityList.Count;
	}

	public override void removeFromList(GameObject go)
	{
		entityList.Remove(go);
//		gameI.GetComponentInChildren<Text>().text = getListLength().ToString();

	}

	public override void spawn(int i)
	{
		GameObject entity = treePlacement.SetItem(entityToSpawn);
		entity.transform.parent = gameObject.transform;

		UpdateGroupList();
		entity.GetComponent<Home>().setSpawn(this);
	}

	public override Sprite GetsSprite()
	{
		return Sprite;
	}

	public void UpdateGroupList()
	{
		GameObject[] groupObject = GameObject.FindGameObjectsWithTag("Group");
		entityList.Clear();
		foreach (GameObject groupie in groupObject)
		{
			if (groupie.GetComponent<GroupManager>().GetGroupType() == (GroupManager.GroupType) type)
			{
				entityList.Add(groupie);
				groupie.transform.parent = gameObject.transform;

			}
		}
//		gameI.GetComponentInChildren<Text>().text = getListLength().ToString();

	}

	public override int getCost()
	{
		
		return entityToSpawn.GetComponent<Placeable>().placementCost;
	}

	
	
//	public void AttemptToFormNewGroups(string tag) {
//		GameObject[] groupieObjects = GameObject.FindGameObjectsWithTag (tag);
//
//		foreach (GameObject groupie in groupieObjects) {
//			Groupie groupieComponent = groupie.GetComponent<Groupie> ();
//			if (groupieComponent == null) {
//				Debug.LogError ("Group tag found member that is not groupable");
//			} else {
//				groupieComponent.RenewAttemptToFormGroup ();
//			}
//
//		}
//	}
}
