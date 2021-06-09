 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject videoBack;
    public GameObject videoplayer;
    
    public GameObject creditsPanel;

    public void NewGame()
    {
        videoplayer.SetActive(true);
        videoBack.SetActive(true);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {    
        Application.Quit();
    }

    public void Credits()
    {
        
        creditsPanel.SetActive(true);
    }
}
