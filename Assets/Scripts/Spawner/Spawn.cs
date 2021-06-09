using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Spawn : MonoBehaviour
{

	public Texture iconUI;

	public GameInformation gameI;
		
	public abstract int getListLength();

	public abstract void removeFromList(GameObject go);
	
	public abstract void spawn(int i);

	public abstract Sprite GetsSprite();



	public abstract int getCost();








}
