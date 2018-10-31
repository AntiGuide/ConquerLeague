using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    private Transform playerVehicleTransform;

    [SerializeField]
    private float camDistance;

    // Update is called once per frame
    void Update () {
	    transform.position = playerVehicleTransform.position + (-transform.forward * camDistance);

	}
}
