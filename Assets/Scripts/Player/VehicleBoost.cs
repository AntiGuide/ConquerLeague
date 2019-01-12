using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleBoost : MonoBehaviour {
    [SerializeField]
    private float boostStrength;

    [SerializeField]
    private float boostTime;

    private void OnCollisionEnter(Collision collision) {
        var vc = collision.gameObject.GetComponent<VehicleController>();
        if (vc == null) {
            return;
        }

        vc.Boost(boostStrength, boostTime);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Trigger");
    }
}
