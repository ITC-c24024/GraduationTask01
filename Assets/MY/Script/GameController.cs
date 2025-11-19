using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    WaveManager waveManager;
    StartUIScript startUIScript;

    void Start()
    {
        waveManager = gameObject.GetComponent<WaveManager>();
        startUIScript = gameObject.GetComponent<StartUIScript>();

        StartCoroutine(startUIScript.SetUI(0));
    }

    void Update()
    {
        
    }
}
