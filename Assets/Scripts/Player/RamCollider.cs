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
        if (other.gameObject.GetComponent<MinionNet>() != null) {
            //Minion
            var id = other.gameObject.GetComponent<MinionNet>().Id ?? 0;
            CommunicationNet.FakeStatic.SendMinionHP(id, hitPoints.AktHp);
        } else if (other.gameObject.GetComponent<PlayerNet>() != null) {
            //Player
            CommunicationNet.FakeStatic.SendPlayerDamage(hitPoints.AktHp);
        } else if (other.gameObject.GetComponent<TowerNet>() != null) {
            var id = other.gameObject.GetComponent<TowerNet>().ID;
            CommunicationNet.FakeStatic.SendTowerDamage(id, hitPoints.AktHp);
        }
        hitPoints.AktHp = 0;
    }
}