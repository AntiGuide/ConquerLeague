using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the syncronization of the score on the network
/// </summary>
public class GoalNet : MonoBehaviour {
    /// <summary>
    /// Send a new score to the enemy player
    /// </summary>
    /// <param name="leftSide">Points on the left side</param>
    /// <param name="rightSide">Points on the right side</param>
    public void UpdateScore(uint leftSide, uint rightSide) {
        try {
            CommunicationNet.FakeStatic.SendNewScore(leftSide, rightSide);
        } catch (Exception) {
            Debug.Log("CommunicationNet.FakeStatic.SendPlayerMovement(transform, rigidbodyPlayer); produced an error!");
            throw;
        }
    }
}
