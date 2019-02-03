using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.AI;

/// <summary>
/// The players base, which spawns minion
/// </summary>
[RequireComponent(typeof(TeamHandler))]
public class Base : MonoBehaviour, IConfigurable {
    [SerializeField]
    private float healFactor = 2;

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

    /// <summary></summary>
    [SerializeField]
    private Material strapMaterial;

    private Color startStrapColor;
    
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
        CommunicationNet.FakeStatic.RequestMinionID();
        return SpawnMinion(minion, spawnPoint.position, minion.transform.rotation, input[1], true);
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        startStrapColor = Color.white;//strapMaterial.color;
        strapMaterial.color = startStrapColor;
        ConfigButton.ObjectsToUpdate.Add(this);
        TeamHandler = GetComponent<TeamHandler>();
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
        if (other.tag == "Player" && TeamHandler.TeamID == TeamHandler.TeamState.FRIENDLY) {
            strapMaterial.color = Color.green;
        }
    }

    /// <summary>
    /// When the players enters the trigger-collider he can build minions
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay(Collider other) {
        if(other.tag == "Player" && other.gameObject.GetComponent<TeamHandler>().TeamID == TeamHandler.TeamID)
        {
            if(other.GetComponent<HitPoints>().AktHp < other.GetComponent<HitPoints>().maxHp && other.GetComponent<HitPoints>().AktHp > 0)
            {
                other.GetComponent<HitPoints>().AktHp += (byte)(Time.deltaTime * healFactor);
            }
        }

        if (TeamHandler.TeamID == TeamHandler.TeamState.FRIENDLY && other.tag == "Player" && other.gameObject.GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.FRIENDLY && CrossPlatformInputManager.GetButtonDown("Action") && MoneyManagement.HasMoney(minionCost))
        {
            SoundController.FSSoundController.StartSound(SoundController.Sounds.BUY_WARTRUCKS, 1);
            minionsToSpawn++;
            moneyManagement.SubMoney(minionCost);
        }
        else if (TeamHandler.TeamID == TeamHandler.TeamState.FRIENDLY && other.tag == "Player" && other.gameObject.GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.FRIENDLY && CrossPlatformInputManager.GetButtonDown("Action") && !MoneyManagement.HasMoney(minionCost))
        {
            if(!SoundController.FSSoundController.AudioSource1.isPlaying)
            {
                SoundController.FSSoundController.StartSound(SoundController.Sounds.CANTBUY_WARTRUCKS, 1);
            }
        }
    }

    /// <summary>
    /// When the players leaves the Base-Trigger change material color back to startcolor
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player" && TeamHandler.TeamID == TeamHandler.TeamState.FRIENDLY) {
            strapMaterial.color = startStrapColor;
        }
    }

    /// <summary>
    /// This method spawns and initialized a minion on call
    /// </summary>
    /// <param name="minionPrefab">The minion prefab to spawn</param>
    /// <param name="spawnPosition">The position to spawn the minion at</param>
    /// <param name="spawnRotation">The rotation to spawn the minion with</param>
    /// <param name="id">The ID of this minion</param>
    /// <returns>New minion object</returns>
    private GameObject SpawnMinion(GameObject minionPrefab, Vector3 spawnPosition, Quaternion spawnRotation, byte? id = null, bool remoteSpawn = false) {
        var spawnedMinion = Instantiate(minion, spawnPoint.position, spawnPoint.transform.rotation);
        spawnedMinion.GetComponent<TeamHandler>().TeamID = TeamHandler.TeamID;
        spawnedMinion.GetComponent<MinionNet>().Id = id;
        spawnedMinion.GetComponent<MinionMovement>().OnInitialize(canvasTrans);
        if (remoteSpawn) {
            spawnedMinion.GetComponent<NavMeshAgent>().enabled = false;
        }

        return spawnedMinion;
    }

    public void UpdateConfig() {
        minionCost = ConfigButton.MinionsBuyValue;
    }
}