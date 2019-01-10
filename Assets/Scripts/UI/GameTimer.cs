﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays and counts the games current gametime
/// </summary>
public class GameTimer : MonoBehaviour, IConfigurable {
    /// <summary>The text which displays the current Gametime</summary>
    [SerializeField]
    private Text gameTimeText;

    /// <summary>Defines how long one round will last</summary>
    [SerializeField, Tooltip("The Gameplay-Time in Seconds")]
    private float playTime = 180;

    private float timeElapsed;

    private bool gameFinished = false;

    public static bool TimerPaused { get; set; }

    /// <summary>Playtime in minutes</summary>
    private int minutes;

    /// <summary>Playtime in seconds</summary>
    private int seconds;

    private void Start() {
        ConfigButton.ObjectsToUpdate.Add(this);
    }

    // Update is called once per frame
    void Update () {
        if (!TimerPaused) {
            timeElapsed += Time.deltaTime;
            playTime -= Time.deltaTime;
        }

        if(playTime - timeElapsed <= 0 && !gameFinished) {
            timeElapsed = playTime;
            GameManager.EndGame();
            gameFinished = true;
        }

        minutes = (int)(Mathf.RoundToInt(playTime - timeElapsed) / 60f);
        seconds = Mathf.RoundToInt((playTime - timeElapsed) - minutes * 60);

        gameTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
	}

    public void UpdateConfig() {
        playTime = ConfigButton.GameTime;
    }
}   