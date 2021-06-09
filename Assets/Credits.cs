using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour {

    public GameObject creditsPanel;

  

    public void ExitCredits()
    {
        
        creditsPanel.SetActive(false);
    }
}
