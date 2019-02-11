using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TeamHandler.TeamState;

public class RamCollider : MonoBehaviour {
    [SerializeField]
    private TeamHandler teamHandler;

    private void OnTriggerEnter(Collider other) {
        var hitPoints = other.gameObject.GetComponent<HitPoints>();
        var otherTeam = other.gameObject.GetComponent<TeamHandler>();
        if (hitPoints == null || otherTeam == null || otherTeam.TeamID == teamHandler.TeamID || otherTeam.TeamID == NEUTRAL) {
            return;
        }

        hitPoints.LastDamager = HitPoints.Damager.PLAYER_RAM;
        CommunicationNet.FakeStatic.SendPlayerDamage(hitPoints.AktHp);
        hitPoints.AktHp = 0;
    }
}