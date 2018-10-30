using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionCurrency : MonoBehaviour {
    /// <summary>References the CurrencyHandler</summary>
    [SerializeField]
    CurrencyHandler currencyHandler;

    /// <summary>References the Minions Hitpoints script</summary>
    [SerializeField]
    private HitPoints hitPoints;

    /// <summary>how many currency the player gets for killing a minion</summary>
    [SerializeField]
    private byte currencyGain;

    /// <summary>References the minions TeamHandler script</summary>
    [SerializeField]
    TeamHandler teamHandler;

	// Use this for initialization
	void Start () {
        currencyHandler = GameObject.Find("CurrencyHandler").GetComponent<CurrencyHandler>();
    }

    // Update is called once per frame
    void Update () {
        if (teamHandler.TeamID == TeamHandler.TeamState.ENEMY) {
		    if(hitPoints.AktHp < 0) {
                currencyHandler.AktCurrency += currencyGain;
            }
        }
	}
}