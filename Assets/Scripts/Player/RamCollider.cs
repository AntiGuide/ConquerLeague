using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamCollider : MonoBehaviour {
    [SerializeField]
    private TeamHandler teamHandler;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<HitPoints>() != null && other.gameObject.GetComponent<TeamHandler>()?.TeamID != teamHandler.TeamID && other.gameObject.GetComponent<TeamHandler>()?.TeamID != TeamHandler.TeamState.NEUTRAL) {
            other.gameObject.GetComponent<HitPoints>().AktHp -= other.gameObject.GetComponent<HitPoints>().AktHp;
        }
    }
}