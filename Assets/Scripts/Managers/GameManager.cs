using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;
	public EnergyManager energyManager;


	public Canvas MainMenu;

	public GameObject pausePanel;
	
	public GameObject attractionsPanel;
	
	public GameObject scorePanel;

	public Text resultText;

	public Text scoreText;
	
	private Vector3 spawnPosition;
	
	public GameObject[] spawnObjects;

	// Game win/lose variables
	bool playerHasPlacedFirstFlora = false;
	bool gameOver = false;
	int floraCount = 0;
	int faunaCount = 0;
	
	// Use this for initialization
	private void Awake()
	{
		
		Time.timeScale = 1;

		// Setup singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);    
		
		DontDestroyOnLoad(gameObject);

		energyManager = this.gameObject.GetComponent<EnergyManager>();
	}

	void Update() {
		CheckForGameOver ();
	}

	// GAME OVER/WON CODE ----------------------------------
	private void CheckForGameOver() {
		if (playerHasPlacedFirstFlora) {
			UpdateFaunaCount ();
			//Debug.Log ("Flora: " + floraCount + " Fauna: " + faunaCount);
			if (floraCount <= 0 && faunaCount <= 0) {
				GameOver ();
			}
			if (this.gameObject.GetComponent<DayNightManager>().GetCurrentDay() > 25)
			{
				GameWon();
			}
		}
	}

	public void GameOver() {
		Debug.Log("Game over, you lose!");
		gameOver = true;
		// Insert game won code here
		Time.timeScale = 0;
		resultText.text = "You Lose! Your Forest is no more...";
		scoreText.text = "Your survived " + GameObject.FindObjectOfType<DayNightManager>().GetCurrentDay()+" days";
		scorePanel.SetActive(true);
	}

	public void GameWon() {
		Debug.Log("You win!");
		gameOver = true;
		// Insert game won code here
		Time.timeScale = 0;
		resultText.text = "You Win! Your Forest Survived!!!";
		scoreText.text = "Your survived " + GameObject.FindObjectOfType<DayNightManager>().GetCurrentDay()+" days";
		scorePanel.SetActive(true);

	}

	public bool IsGameFinished() {
		return gameOver;
	}

	public void IncreaseFlora() {
		floraCount++;
		CheckForGameOver ();
	}

	public void DecreaseFlora() {
		floraCount--;
		CheckForGameOver ();
	}

//	public void IncreaseFauna() {
//		faunaCount++;
//		CheckForGameOver ();
//	}

//	public void DecreaseFauna()  {
//		faunaCount--;
//		CheckForGameOver ();
//	}

	private void UpdateFaunaCount() {
		faunaCount = 0;
		foreach (GameObject spawn in spawnObjects) {
			faunaCount += spawn.transform.childCount;
		}
	}

	public void PlacedFirstFlora() {
		playerHasPlacedFirstFlora = true;
		CheckForGameOver ();
	}

	public bool HasPlayerPlacedFirstFlora() {
		return playerHasPlacedFirstFlora;
	}
	// ------------------------------------------------------
	public void PauseGame()
	{
		Time.timeScale = 0;
		pausePanel.SetActive(true);
		//Disable scripts that still work while timescale is set to 0
	} 
	public void ShowAttractions()
	{
		Time.timeScale = 0;
		attractionsPanel.SetActive(true);
		//Disable scripts that still work while timescale is set to 0
	} 
	public void CloseAttractions()
	{
		Time.timeScale = 1;
		attractionsPanel.SetActive(false);
		//Disable scripts that still work while timescale is set to 0
	} 
	
	public void ContinueGame()
	{
		Time.timeScale = 1;
		pausePanel.SetActive(false);
		//enable the scripts again
	}

	public void SaveGame()
	{
		PlayerPrefs.SetInt ("Level", SceneManager.GetActiveScene ().buildIndex);
		PlayerPrefs.Save ();
	}
	
	public void LoadGame() {

		SceneManager.LoadScene ( PlayerPrefs.GetInt("Level") );
		Destroy(gameObject);

	}

	public void NewGame()
	{
		Application.LoadLevel(Application.loadedLevel);
		Destroy(gameObject);

	}
	
	public void ExitGame()
	{
		SceneManager.LoadScene(0);
		Destroy(gameObject);
	}
	
	
}


