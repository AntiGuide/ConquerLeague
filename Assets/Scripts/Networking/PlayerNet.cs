using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class handles the behaviour of the player object after network packets came in
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerNet : MonoBehaviour {
    /// <summary>Marks if the attached object is an enemy</summary>
    private bool isEnemy;

    /// <summary>The rigidbody of this object</summary>
    private Rigidbody rigidbodyPlayer;

    /// <summary>
    /// Handles new data from the network component
    /// </summary>
    /// <param name="position">The new positon of the player</param>
    /// <param name="quaternion">The new rotation of the player</param>
    /// <param name="velocity">The new velocity of the player</param>
    /// <param name="hp">The new hp of the player</param>
    public void SetNewMovementPack(Vector3 position, Quaternion quaternion, Vector3 velocity, byte hp = 1) {
        transform.position = position;
        transform.rotation = quaternion;
        rigidbodyPlayer.velocity = velocity;
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        isEnemy = GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.ENEMY;
        rigidbodyPlayer = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if (!isEnemy) {
            CommunicationNet.FakeStatic.SendPlayerMovement(transform, rigidbodyPlayer);
        }
    }
}
