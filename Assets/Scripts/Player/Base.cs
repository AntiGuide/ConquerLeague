using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.AI;
using TMPro;

/// <summary>
/// The players base, which spawns minion
/// </summary>
[RequireComponent(typeof(TeamHandler))]
public class Base : MonoBehaviour, IConfigurable
{
    [SerializeField]
    private float healFactor = 1;

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
    private Renderer[] rendererMaterialStraps;

    private Material[] strapMaterial;

    [SerializeField]
    private TextMeshProUGUI minionBuyFeedback;

    private Color startStrapColor;

    private float spawnTimer = 0;

    private int minionsToSpawn = 0;

    private float timeBetweenMinions = 2f;

    private float saveHeal;

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
        startStrapColor = Color.yellow;//strapMaterial.color;
        strapMaterial = new Material[rendererMaterialStraps.Length];
        for (int i = 0; i < rendererMaterialStraps.Length; i++) {
            strapMaterial[i] = rendererMaterialStraps[i].material;
            strapMaterial[i].color = startStrapColor;
        }
        
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
            VehicleAim.AllShootables.Add(CommunicationNet.FakeStatic.Minions[id]);
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
            ButtonChanger.FSButtonChanger.ChangeButton(ButtonChanger.ActionButtonState.BUY_WARTRUCKS);
            for (int i = 0; i < strapMaterial.Length; i++) {
                strapMaterial[i].color = Color.blue;
            }
        }
    }

    /// <summary>
    /// When the players enters the trigger-collider he can build minions
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay(Collider other) {
        if (other.tag != "Player" || other.gameObject.GetComponent<TeamHandler>().TeamID != TeamHandler.TeamID) {
            return;
        }

        var hp = other.GetComponent<HitPoints>();
        HealPlayer(hp);

        if (TeamHandler.TeamID != TeamHandler.TeamState.FRIENDLY) {
            return;
        }

        if (MoneyManagement.HasMoney(minionCost)) {
            ButtonChanger.FSButtonChanger.SetTransparent(false, ButtonChanger.Buttons.ACTION_BUTTON);
            if (CrossPlatformInputManager.GetButtonDown("Action")) {
                SoundController.FSSoundController.StartSound(SoundController.Sounds.BUY_WARTRUCKS, 1);
                FloatUpSpawner.GenerateFloatUp(-minionCost, FloatUp.ResourceType.GAS, Camera.main.WorldToScreenPoint(other.transform.position), 30);
                minionBuyFeedback.text = "";
                minionsToSpawn++;
                moneyManagement.SubMoney(minionCost);
            }
        } else {
            ButtonChanger.FSButtonChanger.SetTransparent(true, ButtonChanger.Buttons.ACTION_BUTTON);
            if(CrossPlatformInputManager.GetButtonDown("Action")) {
                if (!SoundController.FSSoundController.AudioSource1.isPlaying) {
                    SoundController.FSSoundController.StartSound(SoundController.Sounds.CANTBUY_WARTRUCKS, 1);
                }
                StopCoroutine(ClearText(minionBuyFeedback));
                minionBuyFeedback.text = "Not enough currency";
                StartCoroutine(ClearText(minionBuyFeedback));
            }
        }
    }

    private IEnumerator ClearText(TextMeshProUGUI minionBuyFeedback) {
        yield return new WaitForSeconds(3f);
        minionBuyFeedback.text = "";
    }

    /// <summary>
    /// When the players leaves the Base-Trigger change material color back to startcolor
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player" && TeamHandler.TeamID == TeamHandler.TeamState.FRIENDLY) {
            for (int i = 0; i < strapMaterial.Length; i++) {
                strapMaterial[i].color = startStrapColor;
            }

            ButtonChanger.FSButtonChanger.SetTransparent(true, ButtonChanger.Buttons.ACTION_BUTTON);
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
        spawnedMinion.GetComponent<MinionMovement>().OnInitialize();
        if (remoteSpawn) {
            spawnedMinion.GetComponent<NavMeshAgent>().enabled = false;
        }

        return spawnedMinion;
    }

    public void UpdateConfig() {
        minionCost = ConfigButton.MinionsBuyValue;
    }

    private void HealPlayer(HitPoints hp) {
        if (hp.AktHp < hp.SaveHp && hp.AktHp > 0) {
            var heal = Time.deltaTime * healFactor;
            saveHeal += heal;
            heal = Mathf.Min(saveHeal, hp.SaveHp - hp.AktHp);
            hp.AktHp += (byte)heal;
            saveHeal -= (byte)heal;
        }
    }
}