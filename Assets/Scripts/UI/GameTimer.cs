using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {

    /// <summary>The text which displays the current Gametime</summary>
    [SerializeField]
    private Text gameTimeText;

    /// <summary>Defines how long one round will last</summary>
    [SerializeField, Tooltip("The Gameplay-Time in Seconds")]
    private float playTime = 180;

    /// <summary>References the GameManager</summary>
    [SerializeField]
    private GameManager gameManager;

    /// <summary>Playtime in minutes </summary>
    private int minutes;

    private int seconds;

    private string actTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        playTime -= Time.deltaTime;

        minutes = Mathf.FloorToInt(playTime / 60f);
        seconds = Mathf.FloorToInt(playTime - minutes * 60);

        actTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        gameTimeText.text = actTime;

        if(playTime <= 0) {
            gameManager.Paused = true;
        }
	}
}   