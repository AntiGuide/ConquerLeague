﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// The players base, which spawns minion
/// </summary>
public class Base : MonoBehaviour, IConfigurable {
    /// <summary>How much currency it costs to build minion</summary>
    [SerializeField]
    private short minionCost = 20;

    /// <summary>Toplane, midlane, botlane minion</summary>
    [SerializeField]
    private GameObject minion;

    /// <summary>The point where minions will spawn</summary>
    [SerializeField]
    private Transform spawnPoint;

    /// <summary>References the MoneyManagement script</summary>
    [SerializeField]
    private MoneyManagement moneyManagement;

    /// <summary>The healthbars canvas trabsform</summary>
    [SerializeField]
    private Transform canvasTrans;
    
    private float spawnTimer = 0;

    private int minionsToSpawn = 0;

    private float timeBetweenMinions = 2f;

    /// <summary>References the Bases attached Teamhandler</summary>
    public TeamHandler TeamHandler { get; set; }

    /// <summary>
    /// Triggered when a minion is initialized over the network
    /// </summary>
    /// <param name="input">The data recieved over the network</param>
    /// <returns></returns>
    public GameObject RecieveMinionInitialize(byte[] input) {
        // 0 = GameMessageType
        // 1 = ID
        return SpawnMinion(minion, spawnPoint.position, minion.transform.rotation, input[1]);
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        ConfigButton.ObjectsToUpdate.Add(this);
        TeamHandler = gameObject.GetComponent<TeamHandler>();
        //startColor = baseRenderer.material.color;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if (minionsToSpawn > 0) {
            spawnTimer += Time.deltaTime;
        } else {
            spawnTimer = 0f;
        }

        if (spawnTimer > timeBetweenMinions) {
            var id = CommunicationNet.FakeStatic.RequestMinionID();
            CommunicationNet.FakeStatic.Minions[id] = SpawnMinion(minion, spawnPoint.position, minion.transform.rotation, id);
            CommunicationNet.FakeStatic.Minions[id].GetComponent<MinionNet>().InitNet();
            minionsToSpawn--;
            spawnTimer -= timeBetweenMinions;
        }
    }

    /// <summary>
    /// Triggered when an other collider enters the collider in this object
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        //if (other.tag == "Player") {
        //    baseRenderer.material.color = Color.green;
        //}
    }

    /// <summary>
    /// When the players enters the trigger-collider he can build minions
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay(Collider other) {
        if (TeamHandler.TeamID == TeamHandler.TeamState.FRIENDLY && other.tag == "Player" && other.gameObject.GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.FRIENDLY && CrossPlatformInputManager.GetButtonDown("Action") && MoneyManagement.HasMoney(minionCost))
        {
            minionsToSpawn++;
            moneyManagement.SubMoney(minionCost);
        }
        else if (TeamHandler.TeamID == TeamHandler.TeamState.FRIENDLY && other.tag == "Player" && other.gameObject.GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.FRIENDLY && CrossPlatformInputManager.GetButtonDown("Action") && !MoneyManagement.HasMoney(minionCost))
        {
            if(!SoundController.FSSoundController.AudioSource1.isPlaying)
            {
                SoundController.FSSoundController.StartSound(SoundController.Sounds.NEED_MORE_GAS);
            }
        }
    }

    /// <summary>
    /// When the players leaves the Base-Trigger change material color back to startcolor
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {
        //if (other.tag == "Player") {
        //    baseRenderer.material.color = startColor;
        //}
    }

    /// <summary>
    /// This method spawns and initialized a minion on call
    /// </summary>
    /// <param name="minionPrefab">The minion prefab to spawn</param>
    /// <param name="spawnPosition">The position to spawn the minion at</param>
    /// <param name="spawnRotation">The rotation to spawn the minion with</param>
    /// <param name="id">The ID of this minion</param>
    /// <returns>New minion object</returns>
    private GameObject SpawnMinion(GameObject minionPrefab, Vector3 spawnPosition, Quaternion spawnRotation, byte? id = null) {
        var spawnedMinion = Instantiate(minion, spawnPoint.position, spawnPoint.transform.rotation);
        spawnedMinion.GetComponent<TeamHandler>().TeamID = TeamHandler.TeamID;
        spawnedMinion.GetComponent<MinionNet>().Id = id;
        spawnedMinion.GetComponent<MinionMovement>().OnInitialize(canvasTrans);
        return spawnedMinion;
    }

    public void UpdateConfig() {
        minionCost = ConfigButton.MinionsBuyValue;
    }
}