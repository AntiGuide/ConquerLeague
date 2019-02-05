using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimatePickup : MonoBehaviour {
    [SerializeField]
    private byte rewardUltiPoints;

    private void OnTriggerEnter(Collider other) {
        var vc = other.gameObject.GetComponent<VehicleController>();
        if (vc == null) {
            return;
        }

        for (int i = 0; i < rewardUltiPoints; i++) {
            UltimateController.FS.AddCharge();
        }
    }
}