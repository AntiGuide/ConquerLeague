using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static bool Paused { get; set; }

    public static TeamHandler.TeamState? LeftTeam;

    public static TeamHandler.TeamState? RightTeam;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Paused) {
            Time.timeScale = 0;
        }
    }
}