using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleBoost : MonoBehaviour {
    [SerializeField]
    private float boostStrength;

    [SerializeField]
    private float boostTime;

    private void OnTriggerEnter(Collider other) {
        var vc = other.gameObject.GetComponent<VehicleController>();
        if (vc == null) {
            return;
        }

        vc.Boost(boostStrength, boostTime);
    }
}
