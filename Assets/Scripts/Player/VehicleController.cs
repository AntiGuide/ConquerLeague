﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// The vehicles controller, which defines how fast its moving and rotating
/// </summary>
public class VehicleController : MonoBehaviour, IConfigurable {
    /// <summary>Defines how fast the vehicle moves</summary>
    [SerializeField]
    private float movementSpeed;

    /// <summary>How many degrees per second the car can turn</summary>
    [SerializeField]
    private float degreePerSecond;

    /// <summary>The vehicles rigidbody</summary>
    private Rigidbody rb;

    /// <summary>The rotation goal</summary>
    private Vector2 goalRotate;

    /// <summary>The color which will be applied to conquered turrets</summary>
    [SerializeField, Tooltip("The color which will be applied to conquered turrets")]
    private Color teamColor;

    [SerializeField]
    private VehicleWheelControll[] wheels;

    /// <summary>The color which will be applied to conquered turrets</summary>
    public Color TeamColor { get; set; }

    /// <summary>
    /// Use this for initialization
    /// </summary>    
    void Start() {
        ConfigButton.ObjectsToUpdate.Add(this);
        rb = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>    
    void Update() {
        Movement(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
        VehicleWheelControll.UpdateWheelsSpin(rb, false);
    }

    /// <summary>
    /// Uses CrossplatformInput to move and rotate player vehicle
    /// </summary>
    void Movement(float horizontalAxis, float verticalAxis) {
        var tractionModifier = 0f;
        for (int i = 0; i < wheels.Length; i++) {
            tractionModifier += wheels[i].WheelHasTraction ? 0.25f : 0f;
        }

        var rotation = new Vector2(horizontalAxis, verticalAxis);
        if (rotation == Vector2.zero || tractionModifier < float.Epsilon) {
            VehicleWheelControll.UpdateWheelsTurn(0f, false);
            return;
        }

        goalRotate = rotation;
        var rotate = Vector2.SignedAngle(goalRotate, Vector2.up);
        VehicleWheelControll.UpdateWheelsTurn(Mathf.Max(-1f, Mathf.Min(1f, rotate / (degreePerSecond * rotation.magnitude * Time.deltaTime))), false);
        var quat = new Quaternion {
            eulerAngles = new Vector3(0, rotate, 0)
        };

        transform.rotation = Quaternion.RotateTowards(transform.rotation, quat, degreePerSecond * rotation.magnitude * Time.deltaTime);
        var newVelocity = transform.forward * rotation.magnitude * movementSpeed;
        rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
    }

    public void UpdateConfig() {
        movementSpeed = ConfigButton.VehicleMGSpeed;
        degreePerSecond = ConfigButton.VehicleMGTuningSpeed;
    }
}