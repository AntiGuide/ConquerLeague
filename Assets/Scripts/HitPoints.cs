﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This units HitPoints, gets destroyed when its reduced to zero
/// </summary>
public class HitPoints : MonoBehaviour {
    /// <summary>The units current hitpoints</summary>
    private byte aktHp;

    /// <summary>Saves the units hp so that it can reset it if a tower gets destroyed</summary>
    [SerializeField]
    private byte saveHp;

    /// <summary>References the Team Handler script</summary>
    [SerializeField]
    private TeamHandler teamHandler;

    [SerializeField] private HealthBar healthBar;

    /// <summary>References the MoneyManagement script</summary>
    private MoneyManagement moneyManagement;

    /// <summary>The amount of money you get for killing</summary>
    [SerializeField, Tooltip("From top to bottom: Player, tower, minion")]
    private short[] moneyValue = new short[3];

    public byte AktHp {
        get {
            return aktHp;
        }

        set {
            aktHp = value;
            if (aktHp <= 0) {
                aktHp = 0;
                OnDeath(gameObject.tag);
            }
            healthBar?.UpdateBar();
        }
    }

    public HealthBar HealthBar { get; set; }

    public byte SaveHp { get; set; }

    public void SetFull() {
        AktHp = saveHp;
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        SetFull();
        moneyManagement = GameObject.Find("Currency").GetComponent<MoneyManagement>();
    }

    /// <summary>
    /// If the unit is a turret, its teamstate resets to neutral, if not it gets destroyed
    /// </summary>
    /// <param name="tag"></param>
    void OnDeath(string tag) {
        switch (tag) {
            case "Minion":
                moneyManagement.AddMoney(moneyValue[2]);
                Destroy(gameObject);
                break;
            case "Turret":
                moneyManagement.AddMoney(moneyValue[1]);
                teamHandler.TeamID = TeamHandler.TeamState.NEUTRAL;
                AktHp = saveHp;
                break;
            case "Player":
                moneyManagement.AddMoney(moneyValue[0]);
                GetComponent<PlayerNet>().OnDeath();
                break;
            default:
                Destroy(gameObject);
                break;
        }
    }
}