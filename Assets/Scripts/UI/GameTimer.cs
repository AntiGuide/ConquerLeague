using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays and counts the games current gametime
/// </summary>
public class GameTimer : MonoBehaviour {
    /// <summary>The text which displays the current Gametime</summary>
    [SerializeField]
    private Text gameTimeText;

    /// <summary>Defines how long one round will last</summary>
    [SerializeField, Tooltip("The Gameplay-Time in Seconds")]
    private float playTime = 180;

    public static bool TimerPaused { get; set; }

    /// <summary>Playtime in minutes</summary>
    private int minutes;

    /// <summary>Playtime in seconds</summary>
    private int seconds;
	
	// Update is called once per frame
	void Update () {
        if (!TimerPaused) {
            playTime -= Time.deltaTime;
        }

        if(playTime <= 0) {
            playTime = 0f;
            GameManager.Paused = true;
        }

        minutes = (int)(Mathf.RoundToInt(playTime) / 60f);
        seconds = Mathf.RoundToInt(playTime - minutes * 60);

        gameTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
	}
}   