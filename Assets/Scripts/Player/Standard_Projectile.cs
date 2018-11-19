using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The players standard-shot projectile
/// </summary>
public class Standard_Projectile : MonoBehaviour {
    /// <summary>This projectiles damage</summary>
    [SerializeField]
    private int damage = 2;

    /// <summary>References the bullets attached Teamhandler</summary>
    [SerializeField]
    private TeamHandler teamHandler;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        Destroy(this.gameObject, 2f);
    }

    /// <summary>
    /// Deals damage if gameobject has hitpoints script, gets destroyed if not
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other) {
        if (other.GetComponent<HitPoints>() != null) {
            if (other.gameObject.GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.ENEMY) {
                other.gameObject.GetComponent<HitPoints>().AktHp -= damage;
            }
            Destroy(gameObject);
        }
    }
}