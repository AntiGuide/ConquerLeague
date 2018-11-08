using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour {

    public static bool Paused {
        get {
            return paused;
        }

        set {
            paused = value;
            UpdatePausedSetting();
        }
    }

    public static TeamHandler.TeamState? LeftTeam;

    public static TeamHandler.TeamState? RightTeam;

    private static bool paused;

    private void Start() {
        Paused = true;
    }

    // Update is called once per frame
    void Update() {
        if (CrossPlatformInputManager.GetButtonDown("Debug")) {
            Paused = !Paused;
        }
    }

    private static void UpdatePausedSetting() {
        if (Paused) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }
}