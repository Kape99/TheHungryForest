using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyManager))]
public class DayNightManager : MonoBehaviour {

	private int day = 0;
	private bool skipCurrentDay = false;

	public Text dayText;
	public Text countdownText;

	public GameObject skipButtonObject;
	private Image skipButtonImage;
	private Button skipButtonButton;
	private Text skipButtonText;

	public GameObject placementUIObject;

	public GameObject sun;
	Coroutine dayCoroutine;
	Coroutine nightCoroutine;
	bool sunUp = true;
	public float sunUpRotation = 112;
	public float sunDownRotation = 184;

	public float dayTimeDuration = 20; // seconds

	private bool attracted = false;

	EnemyManager enemyManager;
	AttractionManager attractionManager;

	// Use this for initialization
	void Start () {
		// Get components
		enemyManager = this.GetComponent<EnemyManager>(); 
		attractionManager = this.GetComponent<AttractionManager> ();
		skipButtonImage = skipButtonObject.GetComponent<Image> ();
		skipButtonButton = skipButtonObject.GetComponent<Button> ();
		skipButtonText = skipButtonObject.GetComponentInChildren<Text> ();

		// Start first day
		dayCoroutine = StartCoroutine (Day ());
	}
	
	// Update is called once per frame
	void Update () {
		if (sunUp) {
			SlerpSunUp ();
		} else {
			SlerpSunDown ();
		}

//		if (Input.GetKeyDown ("1")) {
//			day = 10;
//			StopCoroutine (nightCoroutine);
//			StopCoroutine (dayCoroutine);
//			dayCoroutine = StartCoroutine (Day ());
//		}
			
		
	}

	// COROUTINES ---------------------------------------------

	IEnumerator Day() {
		// Setup day stuff
		attracted = false;
		SendSunUp();
		if (GameManager.instance.IsGameFinished ()) {
			StopCoroutine (nightCoroutine);
			StopCoroutine (dayCoroutine);
			yield break;
		}
		UpdateDayText ();
		EnableSkipButton ();

		// Enable flora placement UI
		EnablePlayerPlacementAbility();

		// Enable attraction of animals

		// While loop
		float counter = 0;
		while (counter <= dayTimeDuration) { // WHEN DAY TIME IS OVER
			if (GameManager.instance.HasPlayerPlacedFirstFlora ()) {
				counter += Time.deltaTime;
				UpdateCountDownText (counter, dayTimeDuration);
			}

			if (skipCurrentDay) {
				counter = dayTimeDuration; // Move to end of day
				skipCurrentDay = false;
			}

			// TODO: Update this, currently attract animals near end of the day
			// TODO Attracted is use to attract the animals just once a day
			
			if (counter >= (dayTimeDuration * 0.75) && !attracted) {
				ActivateAnimalAttraction ();
				Debug.Log ("Attract animals now!");
				attracted = true;
			}


			yield return null;
		}

		nightCoroutine = StartCoroutine (Night ());
	}

	IEnumerator Night() {
		// Set night stuff
		SendSunDown();
		DisableSkipButton ();

		// Disable flora placement
		DisablePlayerPlacementAbility();

		// Disable attraction

		// Spawn enemies
		enemyManager.SpawnWave(day);

		// While loop
		while (!enemyManager.AllSpawnedEnemiesDead()) {

			yield return null;
		}

		dayCoroutine = StartCoroutine (Day ());
	}

	// PRIVATE FUNCTIONS ----------------------------------------

	private void SlerpSunUp() {
		sun.transform.rotation = Quaternion.Slerp (sun.transform.rotation, Quaternion.Euler (sunUpRotation, 319, 0), Time.deltaTime * 0.5f);
	}

	private void SlerpSunDown() {
		sun.transform.rotation = Quaternion.Slerp (sun.transform.rotation, Quaternion.Euler (sunDownRotation, 319, 0), Time.deltaTime);
	}

	private void SendSunDown() {
		sunUp = false;
	}

	private void SendSunUp() {
		sunUp = true;
		day++;

		// If the day is above the number of levels available
		if (day > enemyManager.GetNumberOfLevels ()) {
			GameManager.instance.GameWon ();
		}
	}

	private void UpdateDayText() {
		dayText.text = "Day " + day;
	}

	private void EnableSkipButton() {
		skipButtonButton.enabled = true;
		skipButtonImage.enabled = true;
		skipButtonText.enabled = true;
	}

	private void DisableSkipButton() {
		skipButtonButton.enabled = false;
		skipButtonImage.enabled = false;
		skipButtonText.enabled = false;
	}

	private void UpdateCountDownText(float counter, float dayLength) {
		if (Mathf.Round (counter) >= dayLength) {
			countdownText.text = "";
		} else {
			countdownText.text = "Night comes in " + (Mathf.Round(dayLength) - Mathf.Round (counter));
		}
	}

	private void DisablePlayerPlacementAbility() {
		placementUIObject.SetActive (false);
	}

	private void EnablePlayerPlacementAbility() {
		placementUIObject.SetActive (true);
	}

	private void ActivateAnimalAttraction() {
		attractionManager.checkConstraint ();
		attractionManager.spawnEntities ();
	}

	// PUBLIC FUNCTIONS ------------------------------------------

	public int GetCurrentDay() {
		return day;
	}

	public void SkipCurrentDay() {
		if (IsDay ()) {
			skipCurrentDay = true;
		} else {
			Debug.LogError ("Attempt to skip day when it was not day");
		}
	}

	public bool IsDay() {
		return sunUp;
	}

	public bool IsNight() {
		return !sunUp;
	}

	// DEBUG FUNCTIONS -------------------------------------------

	private void DebugDayAndCounter(float counter) {
		Debug.Log ("Day " + day + " Seconds " + Mathf.Round(counter));
	}
}
