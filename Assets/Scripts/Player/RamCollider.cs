using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamCollider : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<HitPoints>() != null) {
            other.gameObject.GetComponent<HitPoints>().AktHp -= other.gameObject.GetComponent<HitPoints>().AktHp;
        }
    }
}
