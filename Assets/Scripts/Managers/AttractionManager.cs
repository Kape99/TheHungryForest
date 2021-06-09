using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractionManager : MonoBehaviour
{
	// Class that controll the constraint 
	public Attraction[] _attractions;
	[System.Serializable]
	public class Attraction
	{
		//The spawner object that have to be attracted
		public Spawn Spawn;
		public AttractionConstraint[] AttractionConstraints;
		internal bool canBeAttracted;
		internal int many;
	}
	[System.Serializable]
	public class AttractionConstraint
	{
		// Spawner of the constraint
		public Spawn costraintSpawn;
		// how many can be spawned for each perfab
		public int perPrefab;
		// how many prefab  are needed to spawn this
		public int num;
	}
	

	public void checkConstraint()
	{
		foreach (Attraction attraction in _attractions)
		{
			attraction.canBeAttracted = true;
			attraction.many = 0;
			foreach (AttractionConstraint attractionAC in attraction.AttractionConstraints)
			{
				if (attractionAC.costraintSpawn.getListLength() * attractionAC.perPrefab <=
				    attraction.Spawn.getListLength() * attractionAC.num)
				{
					attraction.canBeAttracted = false;
				}
			}
 		}
		updateSpawningValue();
	}

	
	//TODO fix spawning number
	private void updateSpawningValue()
	{
		foreach (Attraction attraction in _attractions)
		{
			if (attraction.canBeAttracted)
			{
				foreach (AttractionConstraint attractionAC in attraction.AttractionConstraints)
				{
					if (attraction.many == 0)
					{
						attraction.many =
							Mathf.FloorToInt((attractionAC.costraintSpawn.getListLength() / attractionAC.num) * attractionAC.perPrefab-attraction.Spawn.getListLength());
						
//					attraction.many = Mathf.Max(attractionAC.costraintSpawn.getListLength() * attractionAC.perPrefab - attraction.Spawn.getListLength() + 1 ,attractionAC.costraintSpawn.getListLength() / attractionAC.num - attraction.Spawn.getListLength());
					}
					attraction.many = Mathf.Min(attraction.many,
						Mathf.FloorToInt((attractionAC.costraintSpawn.getListLength() / attractionAC.num) * attractionAC.perPrefab)-attraction.Spawn.getListLength());
				}
			}
		}
	}

	//TODO add them to a Queue and spawn during the day
	public void spawnEntities()
	{

		foreach (Attraction attraction in _attractions)
		{
			if (attraction.canBeAttracted)
			{
			

				Debug.LogError(attraction.many);
				attraction.Spawn.spawn(Mathf.CeilToInt(attraction.many / 2f));
				attraction.canBeAttracted = false;
			}

		}
	}
}
