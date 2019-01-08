using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// The game manager. Handles the most important info for the state of the game, can pause the game and disable input.
/// </summary>
public class GameManager : MonoBehaviour {
    /// <summary>Static reference to the only occurence of this object. Used because Unity doesn't let you set info for static components</summary>
    public static GameManager FSGameManager;

    /// <summary>Defines wether the left team is the enemy or the friendly team. Null until initialization.</summary>
    public static TeamHandler.TeamState? LeftTeam;

    /// <summary>Defines wether the right team is the enemy or the friendly team. Null until initialization.</summary>
    public static TeamHandler.TeamState? RightTeam;

    /// <summary>References the GoalManager-script</summary>
    private static GoalManager goalManager;

    /// <summary>The Buttons-Canvas</summary>
    private static Canvas MobileSingleStickControl;

    public static TowerNet[] towers = new TowerNet[byte.MaxValue];

    /// <summary>The sprites with numbers from 0-9</summary>
    public Sprite[] SpritesCountdown;

    public Image CountdownImage;

    public Image CountdownBackgroundImage;

    private static Image disableInputFakeStatic;

    private static bool paused;

    /// <summary>References the GameTimer script</summary>
    [SerializeField] private GameTimer gameTimer;

    [SerializeField] private Image disableInput;

    /// <summary> The spawn point of the player on the left side </summary>
    [SerializeField] private Transform startPointLeft;

    /// <summary> The spawn point of the player on the right side </summary>
    [SerializeField] private Transform startPointRight;

    [SerializeField] private GameObject victoryScreen;
                             
    [SerializeField] private GameObject defeatScreen;

    /// <summary>Getter/Setter for paused</summary>
    public static bool Paused {
        get {
            return paused;
        }

        set {
            paused = value;
            UpdatePausedSetting();
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
        Paused = false;
        FSGameManager.CountdownImage.enabled = false;
        FSGameManager.CountdownBackgroundImage.enabled = false;
        yield return null;
    }

    /// <summary>
    /// Disables input trough enabling a transparent image
    /// </summary>
    /// <param name="setTo"></param>
    public static void DisableInput(bool setTo) {
        disableInputFakeStatic.enabled = setTo;
    }

    /// <summary>
    /// Updates settings that are related to the pause state
    /// </summary>
    private static void UpdatePausedSetting() {
        //DisableInput(Paused);
        GameTimer.TimerPaused = Paused;
    }

    /// <summary>
    /// Runs on object initialization
    /// </summary>
    private void Start() {
        if (FSGameManager != null) {
            Application.Quit();
        }

        FSGameManager = this;
        disableInputFakeStatic = disableInput;
        Paused = true;
        LeftTeam = TeamHandler.TeamState.FRIENDLY;
        RightTeam = TeamHandler.TeamState.ENEMY;
    }

    /// <summary>Ends the game, will be called in the GameTimer-script</summary>
    public static void EndGame() {
        Paused = true;

        if(goalManager.LeftGoals > goalManager.RightGoals) {
            Paused = true;
            MobileSingleStickControl.enabled = false;
            FSGameManager.victoryScreen.SetActive(true);
        } else if (goalManager.LeftGoals < goalManager.RightGoals) {
            Paused = true;
            MobileSingleStickControl.enabled = false;
            FSGameManager.defeatScreen.SetActive(true);
        } else if (goalManager.LeftGoals == goalManager.RightGoals) {

        }
    }

    public void BackToMenue() {
        SceneManager.LoadScene("MainMenue");
        CommunicationNet.FakeStatic.client.Disconnect("BackToMenue");
    }
}