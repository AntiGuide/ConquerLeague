using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// This class lets players conquer a tower for their team
/// </summary>
public class TurretConquer : MonoBehaviour
{
    [SerializeField]
    private GameObject turret;

    /// <summary>References the TeamHandler script</summary>
    [SerializeField]
    private TeamHandler teamHandler;

    /// <summary>All of the turrets renderer</summary>
    [SerializeField]
    private Renderer[] myRenderers;

    [SerializeField]
    private Renderer topRenderer;

    /// <summary>References the hitpoint script</summary>
    [SerializeField]
    private HitPoints hitPoints;

    [SerializeField]
    private TowerNet towerNet;

    /// <summary>The tops materials which will get switched upon conquering, 0 = neutral, 1 = blue, 2 = red</summary>
    [SerializeField]
    private Material[] topMaterials = new Material[3];

    [SerializeField]
    private Flare[] flare;

    /// <summary>References the MoneyManagement script</summary>
    private MoneyManagement moneyManagement;

    /// <summary>The currency-value which will be aquired upon tower conquering</summary>
    private byte conquerValue;

    /// <summary>Determines if the turret is conquerable by the player</summary>
    //[HideInInspector]
    public bool Conquerable = true;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        towerNet.TurretC = this;
        moneyManagement = GameObject.Find("Currency").GetComponent<MoneyManagement>();
        myRenderers = turret.GetComponentsInChildren<MeshRenderer>();
        //for (int i = 1; i < myRenderers.Length; i++) {
        //    myRenderers[i].material.color = Color.gray;
        //}
        flare = turret.GetComponentsInChildren<Flare>();
    }

    /// <summary>
    /// Triggered when a player (or something else) enters the towers conquer collider
    /// </summary>
    /// <param name="other">The colliding object</param>
    void OnTriggerEnter(Collider other) {
        if (Conquerable && other.gameObject.tag == "Player") {
            for (int i = 1; i < myRenderers.Length; i++) {
                myRenderers[i].material.color = Color.white;
            }
        }
    }

    /// <summary>
    /// Triggered when a player (or something else) stays in the towers conquer collider
    /// </summary>
    /// <param name="other">The colliding object</param>
    void OnTriggerStay(Collider other) {
        if (Conquerable && other.gameObject.tag == "Player" && other.gameObject.GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.FRIENDLY) {
            if (CrossPlatformInputManager.GetButtonDown("Action")) {
                BuildTurret(other.gameObject.GetComponent<TeamHandler>().TeamID);
            }
        }
    }

    /// <summary>
    /// Triggered when a player (or something else) leaves the towers conquer collider
    /// </summary>
    /// <param name="other">The colliding object</param>
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player" && Conquerable) {
            for (int i = 1; i < myRenderers.Length; i++) {
                myRenderers[i].material.color = Color.gray;
            }
        }
    }

    /// <summary>
    /// Swap the turrets color and makes it not conquerable, which makes it attack enemy units
    /// </summary>
    /// <param name="teamColor"></param>
    public void BuildTurret(TeamHandler.TeamState teamID) {
        if (teamID == TeamHandler.TeamState.FRIENDLY) {
            topRenderer.material = topMaterials[1];
            CommunicationNet.FakeStatic.SendTowerConquered(towerNet.ID);
            moneyManagement.AddMoney(30);
        } else {
            topRenderer.material = topMaterials[2];
        }

        for (int i = 0; i < flare.Length; i++) {
            flare[i].UpdateColor(teamID);
        }
        teamHandler.TeamID = teamID;

        Conquerable = false;
    }

    public void ResetTowerNeutral() {
        topRenderer.material = topMaterials[0];
        teamHandler.TeamID = TeamHandler.TeamState.NEUTRAL;
        Conquerable = true;
    }
}