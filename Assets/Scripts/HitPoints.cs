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

    /// <summary>References the Turretcontroller</summary>
    TurretCurrency turretCurrency;

    public int AktHp { get; set; }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        if(gameObject.tag == "Turret") {
            turretCurrency = gameObject.GetComponent<TurretCurrency>();
        }
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
        if (tag != "Turret") {
            Destroy(gameObject);
        } else {
            teamHandler.TeamID = TeamHandler.TeamState.NEUTRAL;
            AktHp = saveHp;
            turretCurrency.CurrencyGained = false;
        }
    }
}