using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionCloth : MonoBehaviour {
    [SerializeField]
    private TeamHandler teamHandler;

    [SerializeField]
    private Material matBlue;

    [SerializeField]
    private Material matRed;

    private bool isEnemy;

	// Use this for initialization
	void Start () {
        if(teamHandler.TeamID == TeamHandler.TeamState.FRIENDLY) {
            GetComponent<MeshRenderer>().material = matBlue;
        } else if (teamHandler.TeamID == TeamHandler.TeamState.ENEMY) {
            GetComponent<MeshRenderer>().material = matRed;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
