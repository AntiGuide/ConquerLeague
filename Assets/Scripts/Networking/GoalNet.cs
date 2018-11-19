using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalNet : MonoBehaviour {

    public void UpdateScore(uint leftSide, uint rightSide) {
        try {
            CommunicationNet.FakeStatic.SendNewScore(leftSide, rightSide);
        } catch (Exception) {
            Debug.Log("CommunicationNet.FakeStatic.SendPlayerMovement(transform, rigidbodyPlayer); produced an error!");
            throw;
        }
    }
}
