using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This units HitPoints, gets destroyed when its reduced to zero
/// </summary>
public class HitPoints : MonoBehaviour {
    /// <summary>The units current hitpoints</summary>
    private int aktHp;

    /// <summary>Saves the units hp so that it can reset it if a tower gets destroyed</summary>
    [SerializeField]
    private int saveHp;

    /// <summary>References the Team Handler script</summary>
    [SerializeField]
    private TeamHandler teamHandler;

    /// <summary>References the MoneyManagement script</summary>
    [SerializeField]
    private MoneyManagement moneyManagement;

    /// <summary>The amount of money you get for killing</summary>
    [SerializeField, Tooltip("From top to bottom: Player, tower, minion")]
    private short[] moneyValue = new short[3];

    /// <summary>References the Turretcontroller</summary>
    TurretCurrency turretCurrency;

    public int AktHp { get; set; }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        //if(gameObject.tag == "Turret") {
        //    turretCurrency = gameObject.GetComponent<TurretCurrency>();
        //}

        moneyManagement = GameObject.Find("Currency").GetComponent<MoneyManagement>();
        AktHp = saveHp;
    }


    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if (AktHp < 0) {
            OnDeath(gameObject.tag);
        }
    }

    /// <summary>
    /// If the unit is a turret, its teamstate resets to neutral, if not it gets destroyed
    /// </summary>
    /// <param name="tag"></param>
    void OnDeath(string tag) {
        switch (tag) {
            case "Minion":
                moneyManagement.AddMoney(moneyValue[2]);
                break;
            case "Turret":
                moneyManagement.AddMoney(moneyValue[1]);
                break;
            case "Player":
                moneyManagement.AddMoney(moneyValue[0]);
                break;
        }

        if (tag != "Turret") {
            Destroy(gameObject);
        } else {
            teamHandler.TeamID = TeamHandler.TeamState.NEUTRAL;
            AktHp = saveHp;
            turretCurrency.CurrencyGained = false;
        }
    }
}