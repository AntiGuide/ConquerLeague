using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalManager : MonoBehaviour {
    /// <summary>The Text which displays the current goals</summary>
    [SerializeField]
    private Text goalText;

    [SerializeField]
    private GoalNet goalNet;

    [SerializeField]
    private Text leftScoreText;

    [SerializeField]
    private Text rightScoreText;

    private uint leftGoals;

    private uint rightGoals;

    public uint LeftGoals {
        get {
            return leftGoals;
        }

        set {
            leftGoals = value;
            OutputGoals();
        }
    }

    public uint RightGoals {
        get {
            return rightGoals;
        }

        set {
            rightGoals = value;
            OutputGoals();
        }
    }

    public void AddPoint(TeamHandler.TeamState teamState, uint toAdd = 1) {
        var leftTeam = GameManager.LeftTeam;
        if (leftTeam == null) {
            leftTeam = TeamHandler.TeamState.FRIENDLY;
            //return;
        }

        if (leftTeam == teamState) {
            LeftGoals += toAdd;
        } else {
            RightGoals += toAdd;
        }

        goalNet.UpdateScore(LeftGoals, RightGoals);
    }

    private void OutputGoals() {
        leftScoreText.text = LeftGoals.ToString();
        rightScoreText.text = RightGoals.ToString();
    }
}