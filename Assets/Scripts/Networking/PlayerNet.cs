using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerNet : MonoBehaviour {
    private bool isEnemy;
    private Rigidbody rigidbody;

    // Use this for initialization
    void Start () {
        isEnemy = GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.ENEMY;
        rigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!isEnemy) {
            CommunicationNet.FakeStatic.SendPlayerMovement(transform, rigidbody);
        }
	}

    public void SetNewMovementPack(Vector3 position, Quaternion quaternion, Vector3 velocity, byte hp) {
        transform.position = position;
        transform.rotation = quaternion;
        rigidbody.velocity = velocity;
    }
}
