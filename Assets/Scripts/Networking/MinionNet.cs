using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles networking for minions
/// </summary>
[RequireComponent(typeof(HitPoints))]
public class MinionNet : MonoBehaviour {
    /// <summary>Marks if the attached object is an enemy</summary>
    private bool isEnemy;

    /// <summary>The rigidbody of the attached minion</summary>
    private Rigidbody rigidbodyMinion;

    /// <summary>The ID of this minion</summary>
    private byte? id;

    private HitPoints hitPoints;

    /// <summary>Getter/Setter for id</summary>
    public byte? Id {
        get { return id; }
        set { id = value; }
    }

    /// <summary>
    /// Deinitializes this minion on the network
    /// </summary>
    public void DeInitNet() {
        CommunicationNet.FakeStatic.SendMinionDeinitialization((byte)id);
    }

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
        if (rigidbodyMinion == null) {
            Debug.Log("No rigidbody on minion at SetNewMovementPack with id " + id);
        } else {
            rigidbodyMinion.velocity = velocity;
        }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        hitPoints = GetComponent<HitPoints>();
        isEnemy = GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.ENEMY;
        if (!isEnemy) {
            id = CommunicationNet.FakeStatic.RequestMinionID();
            InitNet();
        }

        rigidbodyMinion = GetComponent<Rigidbody>();
        if (rigidbodyMinion == null) {
            Debug.Log("No rigidbody on minion with id " + id);
        }
    }

    /// <summary>
    /// Initializes this minion on the network
    /// </summary>
    private void InitNet() {
        CommunicationNet.FakeStatic.SendMinionInitialization((byte)id);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if (!isEnemy && id != null) {
            CommunicationNet.FakeStatic.SendMinionMovement(transform, rigidbodyMinion, (byte)id, hitPoints.AktHp);
        }
    }
}
