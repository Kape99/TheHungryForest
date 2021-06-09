using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Base class for all animals and humans.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Creature : MonoBehaviour
{

	[Header("Values Shown For Debug Only")]
	public int health;

	protected float speed;

	protected int attackDamage;
	protected float attackDistance;
	protected float attackSpeed;

	protected bool canAlight = false;

	protected int energyCost;

	NavMeshAgent navMeshAgent;
	bool setAreaMask = false; // used to tell when we are in the forest

	private Spawn spawn; // The object that spawned this
	
	protected void Awake () 
	{
		navMeshAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent> ();
		navMeshAgent.speed = speed;
	}

	protected void Update() 
	{
		if (health <= 0) {
			Die ();
		}
			
		navMeshAgent.speed = GetSpeedOnAreaType (); // Adjust speed on water

		// Once we enter the forest only allow us to walk in the forest (not in the spawn area)
		if (!setAreaMask && IsInForest ()) {
			navMeshAgent.areaMask = 1 << NavMesh.GetAreaFromName ("Walkable") | 1 << NavMesh.GetAreaFromName ("Water");
			setAreaMask = true;
		}
	}

	/// <summary>
	/// Syncs the attack distance with the navmesh agent stopping distance
	/// </summary>
	/// <param name="newAttackDistance">New attack distance.</param>
	protected void SyncAttackDistanceAndStoppingDistance(float newAttackDistance) {
		navMeshAgent.stoppingDistance = newAttackDistance - 0.1f;
	}

	public void MoveTo(GameObject destination)
	{
		if (destination == null) {
			return;
		}

		destination = GetNavigationObjectIfPossible (destination);
		ActualMoveTo (destination.transform.position);
	}

	public void MoveTo(Transform destination)
	{
		if (destination == null) {
			return;
		}

		destination = GetNavigationObjectIfPossible (destination.gameObject).transform;

		ActualMoveTo (destination.transform.position);
	}

	public void MoveTo(Vector3 destination)
	{
		ActualMoveTo (destination);
	}

	private void ActualMoveTo (Vector3 destination) {
		Vector3 Point = destination + Random.insideUnitSphere;
		NavMeshHit NH;
		NavMesh.SamplePosition (Point, out NH, 0.5f, (1 << NavMesh.GetAreaFromName ("Walkable") | 1 << NavMesh.GetAreaFromName ("Water")));
		 
		Vector3 targetVector = NH.position;
		if (navMeshAgent != null) {
			navMeshAgent.isStopped = false;
			navMeshAgent.SetDestination (targetVector);
		}
	}

	public void Warp(Vector3 newPosition) {
		navMeshAgent.Warp (newPosition);
	}

	private GameObject GetNavigationObjectIfPossible(GameObject destination) {
		Transform navObjectTransform = destination.transform.Find ("Navigation Object");

		if (navObjectTransform != null) {
			return navObjectTransform.gameObject;
		} else {
			return destination;
		}
	}

	protected bool isMoving() {
		return (navMeshAgent.velocity.magnitude > 0.01);
	}

	protected float getVelocity() {
		return navMeshAgent.velocity.magnitude;
	}

	/// <summary>
	/// Query whether the agent has essentially reached it's path destination in 2D
	/// </summary>
	/// <returns><c>true</c>, If close to path destination, <c>false</c> otherwise.</returns>
	protected bool PathComplete() {
		if (navMeshAgent.pathPending) {
			return false;
		}

		Vector3 destination = navMeshAgent.destination;

		if (this.gameObject.transform.position.DistanceToIn2D(destination) < navMeshAgent.stoppingDistance + 0.2) {
			//Debug.Log (this.gameObject.name + "'s destination is " + destination.ToString ());
			//Debug.Log (this.gameObject.name + ": " + this.gameObject.transform.position.DistanceToIn2D(destination) + " < " + attackDistance / 2);
			return true;
		} else {
			return false;
		}
	}

	private void Die() 
	{
		StopAllCoroutines ();

		// If we are an animal with a home, remove us from that homes list
		Animal animalComponent = this.GetComponent<Animal> ();
		if (animalComponent != null) {
			if (animalComponent.GetHome() != null) { // If we had a home
				animalComponent.GetHome ().GetComponent<Home> ().RemoveInhabitant (this.gameObject);
			}
			// If we are an animal, let the game manager know an animal has died
			//GameManager.instance.DecreaseFauna();
		}

		// Remove us from the spawner object too
		if (spawn != null) {
			spawn.removeFromList(this.gameObject);
		}

		// Add energy for death
		GameManager.instance.energyManager.AddEnergy(this.energyCost);

		// Destroy us
		GameObject.Destroy (this.gameObject);
	}

	public void RecieveDamage(int damage)
	{
		if (health > 0) { // Lets not beat a dead horse
			health -= damage;
		}
	}

	/// <summary>
	/// Move to the target and attempt to deal damage to it when close enough
	/// </summary>
	/// <returns><c>true</c>, if a successful attack was made, <c>false</c> otherwise.</returns>
	/// <param name="attackTarget">Attack target.</param>
	protected bool MoveToAndAttemptAttack(GameObject attackTarget)
	{
		float distanceToAttackTarget = this.transform.position.DistanceToIn2D(attackTarget.transform.position);
		//Debug.Log (this.gameObject.name + ": " + distanceToAttackTarget + " <= " + attackDistance);
		if (distanceToAttackTarget <= attackDistance) {

			if (navMeshAgent != null) {
				navMeshAgent.isStopped = true;
			}

			//RotateTowards (attackTarget.transform); // TODO: Fix this so it actually looks at the object

			if (attackTarget.tag == "Fauna" || attackTarget.tag == "Enemy") {
				attackTarget.GetComponent<Creature> ().RecieveDamage (attackDamage);
				if (canAlight) {
					AttemptToSetOnFire (attackTarget);
				}
				return true;
			} else if (attackTarget.tag == "Flora") {
				attackTarget.GetComponent<Plant> ().RecieveDamage (attackDamage);
				if (canAlight) {
					AttemptToSetOnFire (attackTarget);
				}
				return true;
			} else {
				return false;
			}
		} else { // If not close enough to attack, get closer
			MoveTo (attackTarget);
			return false;
		}
	}

	private void AttemptToSetOnFire(GameObject target) {
		Flammable flammableComponent = target.GetComponent<Flammable> ();
		if (flammableComponent == null) {
			Debug.LogError ("Attempted to set non flammable object on fire");
			return;
		}

		flammableComponent.AttemptToSetAlight (this.gameObject);
	}

	/// <summary>
	/// A glitchy attempt to rotate the agent towards the target, however 
	/// results in the agent "headbutting" the target instead
	/// </summary>
	/// <param name="target">Target.</param>
	private void RotateTowards (Transform target) 
	{
		Vector3 targetVector = new Vector3 (target.transform.position.x, 0f, target.transform.position.z);
		Vector3 direction = (targetVector - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * attackSpeed);
	}

	/// <summary>
	/// Discerns when we are on water and returns the adjusted speed that the
	/// navmesh agent should have
	/// </summary>
	/// <returns>The speed on the area type.</returns>
	private float GetSpeedOnAreaType() 
	{
		float newSpeed;

		// Code found online
		int waterMask = 1 << NavMesh.GetAreaFromName("Water");
		NavMeshHit hit;

		if (navMeshAgent != null) {

			if (this.gameObject != null) {
				navMeshAgent.SamplePathPosition (-1, 0.0f, out hit);
			} else {
				return speed;
			}

			if (hit.mask == waterMask) {
				newSpeed = speed / 2;
			} else {
				newSpeed = speed;
			}
			return newSpeed;
		} else {
			return speed;
		}
	}

	public bool IsInForest() 
	{
		RaycastHit hit;
		Ray groundDetectRay = new Ray (this.transform.position, Vector3.down);
		float rayLength = 1f;

		if (Physics.Raycast(groundDetectRay, out hit, rayLength)) {
			return (hit.collider.name == "Walkable"); // True if walkable
		} else {
			return false;
		}
	}

	public bool IsInWater() 
	{
		RaycastHit hit;
		Ray groundDetectRay = new Ray (this.transform.position, Vector3.down);
		float rayLength = 1f;

		if (Physics.Raycast(groundDetectRay, out hit, rayLength)) {
			return (hit.collider.tag == "Water"); // True if walkable
		} else {
			return false;
		}
	}

	public void setSpawn(Spawn s)
	{
		spawn = s;
	}

	public int getEnergyPoint()
	{
		return energyCost;
	}
	
}
