using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour {
    
    [SerializeField] private Image disableInput;

    [SerializeField] private Text countdownText;

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

    private static Image disableInputFakeStatic;

    private static bool paused;

    private static Text countdownTextFakeStatic;


    private void Start() {
        disableInputFakeStatic = disableInput;
        countdownTextFakeStatic = countdownText;
        Paused = true;
    }

    // Update is called once per frame
    void Update() {
        if (CrossPlatformInputManager.GetButtonDown("Debug")) {
            Paused = !Paused;
        }
    }

    public static IEnumerator StartGame() {
        countdownTextFakeStatic.text = "3";
        yield return new WaitForSeconds(1f);
        countdownTextFakeStatic.text = "2";
        yield return new WaitForSeconds(1f);
        countdownTextFakeStatic.text = "1";
        yield return new WaitForSeconds(1f);
        countdownTextFakeStatic.text = "GO!";
        Paused = false;
        yield return new WaitForSeconds(1f);
        countdownTextFakeStatic.text = "";
        yield return null;
    }

    private static void UpdatePausedSetting() {
        DisableInput(Paused);
        GameTimer.TimerPaused = Paused;
    }

    public static void DisableInput(bool setTo) {
        disableInputFakeStatic.enabled = setTo;
    }
}