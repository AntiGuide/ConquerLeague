using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionNet : MonoBehaviour {

    /// <summary>Marks if the attached object is an enemy</summary>
    private bool isEnemy;

    // Use this for initialization
    void Start () {
        isEnemy = GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.ENEMY;
        if (!isEnemy) {
            InitNet();
        }
	}

    private void InitNet() {
        CommunicationNet.FakeStatic.SendMinionInitialization();
    }

    // Update is called once per frame
    void Update () {
        if (!isEnemy) {
            //CommunicationNet.FakeStatic.SendMinionMovement(transform, rigidbodyMinion, id);
        }
    }
}
