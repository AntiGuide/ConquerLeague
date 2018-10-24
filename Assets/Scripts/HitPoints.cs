using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This units HitPoints, gets destroyed when its reduced to zero
/// </summary>
public class HitPoints : MonoBehaviour {
    /// <summary>The units current hitpoints</summary>
    public int Hp;

    /// <summary>Saves the units hp so that it can reset it if a tower gets destroyed</summary>
    private int saveHp;

    /// <summary>References the Team Handler script</summary>
    [SerializeField]
    private TeamHandler teamHandler;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        saveHp = Hp;
    }


    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if (Hp <= 0) {
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
            Hp = saveHp;
        }
    }
}