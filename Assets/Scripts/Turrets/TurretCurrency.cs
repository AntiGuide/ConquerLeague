using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretCurrency : MonoBehaviour {
    /// <summary>References the CurrencyHandler</summary>
    CurrencyHandler currencyHandler;

    /// <summary>References the Minions Hitpoints script</summary>
    [SerializeField]
    private HitPoints hitPoints;

    /// <summary>Defines how much currency the player gets for killing a turret</summary>
    [SerializeField]
    private int currencyKillGain;

    /// <summary>Defines how much currency the player gets for capturing a turret</summary>
    [SerializeField]
    private int currencyCaptureGain;

    /// <summary>References the TurretConquer script</summary>
    [SerializeField]
    private TurretConquer turretConquer;

    /// <summary>References the turrets TeamHandler script</summary>
    [SerializeField]
    private TeamHandler teamHandler;

    /// <summary>Checks if the player has gained currency</summary>
    private bool currencyGained = false;

    public bool CurrencyGained { get; set; }

    // Use this for initialization
    void Start() {
        currencyHandler = GameObject.Find("CurrencyHandler").GetComponent<CurrencyHandler>();
    }

    // Update is called once per frame
    void Update() {
        if (teamHandler.TeamID == TeamHandler.TeamState.ENEMY) {
            if (hitPoints.AktHp <= 0) {
                currencyHandler.AktCurrency += currencyKillGain;
            } else if (!turretConquer.Conquerable && !CurrencyGained) {
                currencyHandler.AktCurrency += currencyCaptureGain;
                CurrencyGained = true;
            }
        }
    }
}