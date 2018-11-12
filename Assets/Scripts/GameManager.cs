using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour {
    
    /// <summary></summary>
    [SerializeField] private Image disableInput;

    /// <summary></summary>
    [SerializeField] private Text countdownText;

    /// <summary></summary>
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
        LeftTeam = TeamHandler.TeamState.FRIENDLY;
        RightTeam = TeamHandler.TeamState.ENEMY;
    }

    // Update is called once per frame
    void Update() {
        if (CrossPlatformInputManager.GetButtonDown("Debug")) {
            Paused = !Paused;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    private static void UpdatePausedSetting() {
        DisableInput(Paused);
        GameTimer.TimerPaused = Paused;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="setTo"></param>
    public static void DisableInput(bool setTo) {
        disableInputFakeStatic.enabled = setTo;
    }
}