using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionNet : MonoBehaviour {

    /// <summary>Marks if the attached object is an enemy</summary>
    private bool isEnemy;

    private Rigidbody rigidbodyMinion;

    private byte? id;

    public byte? Id {
        get { return id; }
        set { id = value; }
    }

    // Use this for initialization
    void Start () {
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

    private void InitNet() {
        CommunicationNet.FakeStatic.SendMinionInitialization((byte)id);
    }

    public void DeInitNet() {
        CommunicationNet.FakeStatic.SendMinionDeinitialization((byte)id);
    }

    // Update is called once per frame
    void Update () {
        if (!isEnemy && id != null) {
            CommunicationNet.FakeStatic.SendMinionMovement(transform, rigidbodyMinion, (byte)id);
        }
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
}
