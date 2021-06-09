using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : MonoBehaviour {

	[SerializeField]
	Transform destination;

	UnityEngine.AI.NavMeshAgent navMeshAgent;

	// Use this for initialization
	void Start () {
		navMeshAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent> ();

		if (navMeshAgent == null) {
			Debug.LogError ("Nav Mesh Agent component is not attacthed to " + gameObject.name);
		} else {
			SetDestination ();
		}
	}

	private void SetDestination()
	{
		if (destination != null) {
			Vector3 targetVector = destination.transform.position;
			navMeshAgent.SetDestination (targetVector);
		}
	}
}
