using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the positioning of the camera
/// </summary>
public class CameraController : MonoBehaviour
{
    /// <summary>Refernce to the players transform</summary>
    [SerializeField] private Transform playerVehicleTransform;

    /// <summary>The distance from player to cam</summary>
    [SerializeField] private float camDistance;

    [SerializeField] private Vector3 offset;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if (playerVehicleTransform != null) {
            transform.position = playerVehicleTransform.position + offset + (-transform.forward * camDistance);
        }
    }
}