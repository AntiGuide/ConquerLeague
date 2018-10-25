using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Base : MonoBehaviour {
    /// <summary>How much currency it costs to build minion</summary>
    [SerializeField]
    private short minionCost = 20;

    /// <summary>The bases attached renderer</summary>
    [SerializeField]
    private Renderer renderer;

    /// <summary>Toplane, midlane, botlane minion</summary>
    [SerializeField]
    private GameObject minion;

    /// <summary>The point where minions will spawn</summary>
    [SerializeField]
    private Transform spawnPoint;

    private TeamHandler teamHandler;

    /// <summary>The bases normal color</summary>
    private Color startColor;

    /// <summary>Decides which minion will spawn</summary>
    private int random;

    private float spawnTimer;

	// Use this for initialization
	void Start () {
        teamHandler = gameObject.GetComponent<TeamHandler>();
        startColor = renderer.material.color;
	}
	
	// Update is called once per frame
	void Update () {
        spawnTimer += Time.deltaTime;
        if(spawnTimer >= 2) {
            var spawnedMinion = Instantiate(minion, spawnPoint.position, minion.transform.rotation);
            spawnedMinion.GetComponent<TeamHandler>().TeamID = teamHandler.TeamID;
            spawnTimer -= 2;
        }
	}

    /// <summary>When the players enters the trigger-collider he can build minions</summary>
    /// <param name="other"></param>
    void OnTriggerStay(Collider other) {
        if(other.tag == "Player") {
            renderer.material.color =  Color.green;

            if (CrossPlatformInputManager.GetButtonDown("Action") && other.GetComponent<CurrencyHandler>().AktCurrency >= minionCost) {
                var spawnedMinion = Instantiate(minion, spawnPoint.position, minion.transform.rotation);
                spawnedMinion.GetComponent<TeamHandler>().TeamID = teamHandler.TeamID;
                other.GetComponent<CurrencyHandler>().AktCurrency -= minionCost;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player") {
            renderer.material.color = startColor;
        }
    }
}
