using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// The players base, which spawns minion
/// </summary>
public class Base : MonoBehaviour {
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

    /// <summary>The bases attached renderer</summary>
    private Renderer renderer;

    /// <summary>References the Bases attached Teamhandler</summary>
    private TeamHandler teamHandler;

    /// <summary>The bases normal color</summary>
    private Color startColor;

    /// <summary>Counts to 2, spawns minions if reached</summary>
    private float spawnTimer;

    private GameObject[] minions = new GameObject[byte.MaxValue];

    /// <summary>References the Bases attached Teamhandler</summary>
    public TeamHandler TeamHandler {
        get { return teamHandler; }
        set { teamHandler = value; }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        renderer = gameObject.GetComponent<MeshRenderer>();
        teamHandler = gameObject.GetComponent<TeamHandler>();
        startColor = renderer.material.color;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if (teamHandler.TeamID == TeamHandler.TeamState.FRIENDLY && CrossPlatformInputManager.GetButtonDown("Minion")) {
            SpawnMinion(minion, spawnPoint.position, minion.transform.rotation);
        }

        //spawnTimer += Time.deltaTime;

        // Let minions spawn every 10 seconds on enemy base, used for testing purposes
        //if(teamHandler.TeamID == TeamHandler.TeamState.ENEMY) {
        //    if (spawnTimer >= 10) {
        //        SpawnMinion(minion, spawnPoint.position, minion.transform.rotation);
        //        spawnTimer -= 10;
        //    }
        //}
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            renderer.material.color = Color.green;
        }
    }

    /// <summary>
    /// When the players enters the trigger-collider he can build minions
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay(Collider other) {
        if (other.tag == "Player" && other.gameObject.GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.FRIENDLY && CrossPlatformInputManager.GetButtonDown("Action") && MoneyManagement.HasMoney(minionCost)) {
            SpawnMinion(minion, spawnPoint.position, minion.transform.rotation);
            moneyManagement.SubMoney(minionCost);
        }
    }

    /// <summary>
    /// When the players leaves the Base-Trigger change material color back to startcolor
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            renderer.material.color = startColor;
        }
    }

    private GameObject SpawnMinion(GameObject minionPrefab, Vector3 spawnPosition, Quaternion spawnRotation, byte? id = null) {
        var spawnedMinion = Instantiate(minion, spawnPoint.position, minion.transform.rotation);
        spawnedMinion.GetComponent<TeamHandler>().TeamID = teamHandler.TeamID;
        spawnedMinion.GetComponent<MinionNet>().Id = id;
        return spawnedMinion;
    }

    public GameObject RecieveMinionInitialize(byte[] input) {
        // 0 = GameMessageType
        // 1 = ID
        return SpawnMinion(minion, spawnPoint.position, minion.transform.rotation, input[1]);
    }
}