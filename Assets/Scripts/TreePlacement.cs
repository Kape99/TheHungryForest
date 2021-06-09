using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePlacement : MonoBehaviour
{

	private Placeable placeable;
	private GameObject currentTree;
    private bool isPlaced;

	private bool onLand;
	
	
    // Update is called once per frame
    void Update ()
    {
	    
	    RaycastHit[] hits;
	    
	    
	    
        if (currentTree != null && !isPlaced)
		{
            Vector3 m = Input.mousePosition;
            m = new Vector3(m.x,m.y,m.y);
            m = Camera.main.ScreenToWorldPoint(m);
            currentTree.transform.position = new Vector3(m.x,0,m.z);
			hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
	        onLand = false;
	        foreach (RaycastHit hit in hits)
	        {
		    	if (hit.collider.tag == "Ground")
		        {
			        onLand = true;
					placeable.UpdateOnWater (onLand);
		        }
		       
	        }
	      	foreach (RaycastHit hit in hits)
	        {
		    	if (hit.collider.tag == "Water")
		        {
			        onLand = false;
					placeable.UpdateOnWater (onLand);
		        }
		       
	        }
	      
	        if (Input.GetMouseButtonDown(0))
        	{
           		if (IsLegalPosition() && onLand)
				{
					isPlaced = true;

					// Let the game manager know we've added a new flora
					if (currentTree.tag != "Rock") {
						GameManager.instance.IncreaseFlora(); 
					}
						
					// If this is the first tree placed, let the game manager know
					if (GameManager.instance.HasPlayerPlacedFirstFlora () == false) {
						GameManager.instance.PlacedFirstFlora ();
					}

					if (currentTree.GetComponent<Groupie> () != null) {
						AttemptToFormNewGroups(currentTree.tag);
					}
					Destroy (placeable); // Remove the placeable script once placed!
					
					foreach (GroupSpawner groupSpawner in GameObject.FindObjectsOfType<GroupSpawner>()){
						groupSpawner.UpdateGroupList();//Update the spawner list in order to attract animals
					}
					
				}
       		} 
	        
	        if (Input.GetMouseButtonDown(1))
	        {
		        if (currentTree.transform.parent.CompareTag("Spawn"))
			        currentTree.transform.parent.GetComponent<Spawn>().removeFromList(currentTree);
		        Destroy(currentTree);
		        GameManager.instance.energyManager.AddEnergy(currentTree.GetComponent<Placeable>().placementCost);
		        isPlaced = true;
	        }
			
        }
    }

	private bool IsLegalPosition()
	{
		return placeable.IsPlaceable();
	}

	public GameObject SetItem(GameObject t)
    {
        isPlaced = false;
        currentTree = ((GameObject) Instantiate(t));
	    placeable = currentTree.GetComponent<Placeable>();
        
        return currentTree;
    }
	
	public void AttemptToFormNewGroups(string tag) {
		GameObject[] groupieObjects = GameObject.FindGameObjectsWithTag (tag);

		foreach (GameObject groupie in groupieObjects) {
			Groupie groupieComponent = groupie.GetComponent<Groupie> ();
			if (groupieComponent == null) {
				Debug.LogError ("Group tag found member that is not groupable");
			} else {
				groupieComponent.RenewAttemptToFormGroup ();
			}

		}
	}
	 

	public bool getIsPlaced()
	{
		return isPlaced;
	}
}
