using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeSpawner : Spawn
{

	public GameObject entityToSpawn;

	public List<GameObject> entityList;

	public TreePlacement treePlacement;

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
		entityList.Add(entity);
		entity.GetComponent<Home>().setSpawn(this);
//		gameI.GetComponentInChildren<Text>().text = getListLength().ToString();
	}

	public override Sprite GetsSprite()
	{
		return this.Sprite;
	}

	public override int getCost()
	{
		return entityToSpawn.GetComponent<Placeable>().placementCost;
	}
}
