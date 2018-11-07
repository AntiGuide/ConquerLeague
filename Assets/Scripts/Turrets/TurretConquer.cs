using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// This class lets players conquer a tower for their team
/// </summary>
public class TurretConquer : MonoBehaviour {
    /// <summary>References the TeamHandler script</summary>
    [SerializeField]
    private TeamHandler teamHandler;

    /// <summary>The towers top renderer</summary>
    [SerializeField]
    private Renderer[] towerRenderer;

    /// <summary>Determines if the turret is conquerable by the player</summary>
    private bool conquerable = true;

    /// <summary>Determines if the turret is conquerable by the player</summary>
    public bool Conquerable {
        get { return conquerable; }
        set { conquerable = value; }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        towerRenderer = gameObject.GetComponentsInChildren<MeshRenderer>();
        for(int i = 0; i < towerRenderer.Length; i++) {
            towerRenderer[i].material.color = Color.gray;
        }
    }

    /// <summary>
    /// Triggered when a player (or something else) enters the towers conquer collider
    /// </summary>
    /// <param name="other">The colliding object</param>
    void OnTriggerStay(Collider other) {
        if (Conquerable && other.gameObject.tag == "Player") {
            for (int i = 0; i < towerRenderer.Length; i++) {
                towerRenderer[i].material.color = Color.white;
            }
        }

        if (other.gameObject.tag == "Player" && CrossPlatformInputManager.GetButtonDown("Action") && other.gameObject.GetComponent<TeamHandler>().TeamID != TeamHandler.TeamState.ENEMY) {
            BuildTurret(other.gameObject.GetComponent<VehicleController>().TeamColor, other.gameObject.GetComponent<TeamHandler>().TeamID);
        }
    }

    /// <summary>
    /// Triggered when a player (or something else) leaves the towers conquer collider
    /// </summary>
    /// <param name="other">The colliding object</param>
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player" && Conquerable) {
            for (int i = 0; i < towerRenderer.Length; i++) {
                towerRenderer[i].material.color = Color.gray;
            }
        }
    }

    /// <summary>
    /// Swap the turrets color and makes it not conquerable, which makes it attack enemy units
    /// </summary>
    /// <param name="teamColor"></param>
    void BuildTurret(Color teamColor, TeamHandler.TeamState teamID) {
        teamHandler.TeamID = teamID;
        for (int i = 0; i < towerRenderer.Length; i++) {
            towerRenderer[i].material.color = teamColor;
        }
        Conquerable = false;
    }
}