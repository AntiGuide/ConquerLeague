using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// The vehicles controller, which defines how fast its moving and rotating
/// </summary>
public class VehicleController : MonoBehaviour, IConfigurable {
    /// <summary>Defines how fast the vehicle moves</summary>
    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float boostFactor = 1f;

    /// <summary>How many degrees per second the car can turn</summary>
    [SerializeField]
    private float degreePerSecond;

    /// <summary>The vehicles rigidbody</summary>
    private Rigidbody rb;

    /// <summary>The rotation goal</summary>
    private Vector2 goalRotate;

    [SerializeField]
    private VehicleWheelControll[] wheels;

    [SerializeField]
    private ParticleSystem[] particleSystems;

    [SerializeField]
    private TrailRenderer[] trailRenderer;

    /// <summary>The color which will be applied to conquered turrets</summary>
    public Color TeamColor { get; set; }

    public Text debugText;

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
        if (rotation != Vector2.zero && tractionModifier < float.Epsilon && rb.velocity.sqrMagnitude < float.Epsilon) {
            Debug.Log("Stuck");
        }

        if (rotation == Vector2.zero || tractionModifier < float.Epsilon) {
            VehicleWheelControll.UpdateWheelsTurn(0f, false);
            for (int i = 0; i < particleSystems.Length; i++) {
                var em = particleSystems[i].emission;
                em.enabled = false;
            }

            for (int i = 0; i < trailRenderer.Length; i++) {
                trailRenderer[i].enabled = false;
            }
            

            return;
        }

        for (int i = 0; i < particleSystems.Length; i++) {
            var em = particleSystems[i].emission;
            em.enabled = true;
        }

        for (int i = 0; i < trailRenderer.Length; i++) {
            trailRenderer[i].enabled = true;
        }

        goalRotate = rotation;
        var rotate = Vector2.SignedAngle(goalRotate, Vector2.up);
        VehicleWheelControll.UpdateWheelsTurn(Mathf.Max(-1f, Mathf.Min(1f, rotate / (degreePerSecond * rotation.magnitude * Time.deltaTime))), false);
        var quat = new Quaternion {
            eulerAngles = new Vector3(0, rotate, 0)
        };

        transform.rotation = Quaternion.RotateTowards(transform.rotation, quat, degreePerSecond * rotation.magnitude * Time.deltaTime);
        var newVelocity = transform.forward * rotation.magnitude * movementSpeed * boostFactor;
        rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);

        var flatAngle = transform.rotation.eulerAngles;
        flatAngle.x = 0;
        flatAngle.z = 0;
        var flatQuat = new Quaternion();
        flatQuat.eulerAngles = flatAngle;

        var steerValue = rotate - flatAngle.y;
        steerValue += steerValue < -180f ? 360f : 0f;
        steerValue -= steerValue > 180f ? 360f : 0f;
        var maxDegree = 45f;
        steerValue = Mathf.Clamp(steerValue, -maxDegree, maxDegree);
        steerValue /= maxDegree;
        var speedValue = rotation.magnitude;
    }

    public void Boost(float boostStrenght, float boostTime) {
        boostFactor = 1f + boostStrenght > boostFactor ? 1f + boostStrenght : boostFactor;
        StopCoroutine(ResetBoost(boostTime));
        StartCoroutine(ResetBoost(boostTime));
    }

    private IEnumerator ResetBoost(float boostTime) {
        yield return new WaitForSeconds(boostTime);
        boostFactor = 1f;
    }

    public void UpdateConfig() {
        movementSpeed = ConfigButton.VehicleMGSpeed;
        degreePerSecond = ConfigButton.VehicleMGTuningSpeed;
    }
}