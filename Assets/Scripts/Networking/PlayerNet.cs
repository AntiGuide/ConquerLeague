using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNet : MonoBehaviour {
    private bool isEnemy;

    // Use this for initialization
    void Start () {
        isEnemy = GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.ENEMY;
	}
	
	// Update is called once per frame
	void Update () {
        if (CommunicationNet.FakeStatic?.ConnectedToServer ?? false) {
            CommunicationNet.FakeStatic.SendPlayerMovement(transform);
        }
	}

    internal void SetNewMovementPack(Vector3 position, Quaternion quaternion, byte hp) {
        transform.position = position;
        transform.rotation = quaternion;
    }
}
