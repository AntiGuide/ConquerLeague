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

    /// <summary>The bases attached renderer</summary>
    private Renderer renderer;

    /// <summary>References the Bases attached Teamhandler</summary>
    private TeamHandler teamHandler;

    /// <summary>The bases normal color</summary>
    private Color startColor;

    /// <summary>Counts to 2, spawns minions if reached</summary>
    private float spawnTimer;

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
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= 2) {
            var spawnedMinion = Instantiate(minion, spawnPoint.position, minion.transform.rotation);
            spawnedMinion.GetComponent<TeamHandler>().TeamID = teamHandler.TeamID;
            spawnTimer -= 2;
        }
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
        if (other.tag == "Player") {
            if (CrossPlatformInputManager.GetButtonDown("Action") && other.GetComponent<CurrencyHandler>().AktCurrency >= minionCost) {
                var spawnedMinion = Instantiate(minion, spawnPoint.position, minion.transform.rotation);
                spawnedMinion.GetComponent<TeamHandler>().TeamID = teamHandler.TeamID;
                other.GetComponent<CurrencyHandler>().AktCurrency -= minionCost;
            }
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
}
