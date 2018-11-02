using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {

    /// <summary>The text which displays the current Gametime</summary>
    [SerializeField]
    private Text gameTimeText;

    [SerializeField, Tooltip("The Gameplay-Time in Seconds")]
    private float playTime = 300;

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

        if(playTime < 0) {

        }
	}
}