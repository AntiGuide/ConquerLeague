using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// This class lets players conquer a tower for their team
/// </summary>
public class TurretConquer : MonoBehaviour {
    /// <summary>The color of the players team</summary>
    [SerializeField]
    private Color teamColor;

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
        gameObject.GetComponent<Renderer>().material.color = Color.gray;
    }

    /// <summary>
    /// Triggered when a player (or something else) enters the towers conquer collider
    /// </summary>
    /// <param name="other">The colliding object</param>
    void OnTriggerStay(Collider other) {
        if (Conquerable && other.gameObject.tag == "Player") {
            GetComponent<Renderer>().material.color = Color.white;
        }

        if (other.gameObject.tag == "Player" && CrossPlatformInputManager.GetButtonDown("Action")) {
            BuildTurret(teamColor);
        }
    }

    /// <summary>
    /// Triggered when a player (or something else) leaves the towers conquer collider
    /// </summary>
    /// <param name="other">The colliding object</param>
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player" && Conquerable) {
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
        }
    }

    /// <summary>
    /// Swap the turrets color and makes it not conquerable, which makes it attack enemy units
    /// </summary>
    /// <param name="teamColor"></param>
    void BuildTurret(Color teamColor) {
        gameObject.GetComponent<Renderer>().material.color = teamColor;
        Conquerable = false;
    }
}
