using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour {

    public static GameManager FSGameManager;

    [SerializeField] private Image disableInput;

    /// <summary>References the Countdown-Text</summary>
    [SerializeField] private Text countdownText;

    /// <summary> The spawn point of the player on the left side </summary>
    [SerializeField] private Transform startPointLeft;

    /// <summary> The spawn point of the player on the right side </summary>
    [SerializeField] private Transform startPointRight;

    public Sprite[] SpritesCountdown;

    public Image CountdownImage;

    public Image CountdownBackgroundImage;



    public static bool Paused {
        get {
            return paused;
        }

        set {
            paused = value;
            UpdatePausedSetting();
        }
    }

    public static Text CountdownTextFakeStatic {
        get {
            return countdownTextFakeStatic;
        }

        set {
            countdownTextFakeStatic = value;
        }
    }

    public static TeamHandler.TeamState? LeftTeam;

    public static TeamHandler.TeamState? RightTeam;

    private static Image disableInputFakeStatic;

    private static bool paused;

    private static Text countdownTextFakeStatic;


    private void Start() {
        if (FSGameManager != null) {
            Application.Quit();
        }

        FSGameManager = this;
        disableInputFakeStatic = disableInput;
        countdownTextFakeStatic = countdownText;
        Paused = true;
        LeftTeam = TeamHandler.TeamState.FRIENDLY;
        RightTeam = TeamHandler.TeamState.ENEMY;
        DisableInput(false);
    }

    // Update is called once per frame
    void Update() {
        if (CrossPlatformInputManager.GetButtonDown("Debug")) {
            Paused = !Paused;
        }
    }

    /// <summary>
    /// The games startup countdown
    /// </summary>
    /// <returns></returns>
    public static IEnumerator StartGame() {
        FSGameManager.CountdownBackgroundImage.enabled = true;
        FSGameManager.CountdownImage.enabled = true;
        FSGameManager.CountdownImage.sprite = FSGameManager.SpritesCountdown[3];
        yield return new WaitForSeconds(1f);
        FSGameManager.CountdownImage.sprite = FSGameManager.SpritesCountdown[2];
        yield return new WaitForSeconds(1f);
        FSGameManager.CountdownImage.sprite = FSGameManager.SpritesCountdown[1];
        yield return new WaitForSeconds(1f);
        //FSGameManager.countdownImage.sprite = FSGameManager.spritesCountdown[0];
        Paused = false;
        //yield return new WaitForSeconds(1f);
        FSGameManager.CountdownImage.enabled = false;
        FSGameManager.CountdownBackgroundImage.enabled = false;
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