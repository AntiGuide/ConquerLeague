using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The turrets bullet, inflicts damage on minions and player
/// </summary>
public class TurretBullet : MonoBehaviour {
    /// <summary>The damage of the bullet</summary>
    [SerializeField]
    private int damage = 15;

    /// <summary>References the Bullets attached Teamhandler script</summary>
    [SerializeField]
    private TeamHandler teamHandler;

    /// <summary>
    /// Checks if the colliding gameobject has hitpoints and deals damage if it has
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other) {
        if (other.GetComponent<HitPoints>() != null) {
            if (other.gameObject.GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.ENEMY) {
                other.GetComponent<HitPoints>().AktHp -= damage;
                Destroy(gameObject);
            }
        }
    }
}
