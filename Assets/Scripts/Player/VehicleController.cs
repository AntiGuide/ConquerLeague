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

    [SerializeField]
    private Transform raycastTrans;

    /// <summary>How many degrees per second the car can turn</summary>
    [SerializeField]
    private float degreePerSecond;

    /// <summary>The vehicles rigidbody</summary>
    private Rigidbody rb;

    /// <summary>The rotation goal</summary>
    private Vector2 goalRotate;

    private float startMovementSpeed;

    [SerializeField]
    private VehicleWheelControll[] wheels;

    [SerializeField]
    private ParticleSystem[] particleSystems;

    [SerializeField]
    private TrailRenderer[] trailRenderer;

    private Ray ray;
    private RaycastHit hit;

    /// <summary>The color which will be applied to conquered turrets</summary>
    public Color TeamColor { get; set; }

    public Text debugText;

    /// <summary>
    /// Use this for initialization
    /// </summary>    
    void Start() {
        startMovementSpeed = movementSpeed;
        ConfigButton.ObjectsToUpdate.Add(this);
        rb = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>    
    void Update() {
        if (Physics.Raycast(ray, out hit, 3f, LayerMask.GetMask("Wall", "Default"))) {
            movementSpeed = 0;
            print("stuck");
        } else if (Physics.Raycast(ray, out hit, 5, LayerMask.GetMask("Wall", "Default"))) {
            print(hit.transform.gameObject.name);
            movementSpeed = 5;
        } else {
            movementSpeed = startMovementSpeed;
        }
        Movement(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
        VehicleWheelControll.UpdateWheelsSpin(rb, false);
        ray.origin = raycastTrans.position;
        ray.direction = raycastTrans.forward;
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 1f);
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