﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class TurretConquer : MonoBehaviour {

    /// <summary>The color of the players team</summary>
    [SerializeField]
    private Color teamColor;

    /// <summary>Determines if the turret is conquerable by the player</summary>
    [HideInInspector]
    public bool conquerable = true;

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Renderer>().material.color = Color.gray;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider other) {
        if(conquerable && other.gameObject.tag == "Player") {
            GetComponent<Renderer>().material.color = Color.white;
        }

        if(other.gameObject.tag == "Player" && CrossPlatformInputManager.GetButtonDown("Action")) {
            BuildTurret(teamColor);
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.gameObject.tag == "Player" && conquerable) {
            gameObject.GetComponent<Renderer>().material.color = Color.gray;
        }
    }

    /// <summary>
    /// Swap the turrets color and makes it not conquerable, which makes it attack enemy units
    /// </summary>
    /// <param name="teamColor"></param>
    void BuildTurret(Color teamColor) {
        gameObject.GetComponent<Renderer>().material.color = teamColor;
        conquerable = false;
    }
}