using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour {

    /// <summary>The damage of the bullet</summary>
    [SerializeField]
    private int damage = 15;

    /// <summary>References the Bullets attached Teamhandler script</summary>
    [SerializeField]
    private TeamHandler teamHandler;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<TeamHandler>().TeamID != teamHandler.TeamID) {
            other.GetComponent<HitPoints>().Hp -= damage;
        }
        Destroy(gameObject);
    }
}
