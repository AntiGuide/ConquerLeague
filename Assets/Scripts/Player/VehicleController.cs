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
    [SerializeField]
    private BoxCollider ramCollider;

    [SerializeField]
    private float maxMovementSpeed = 14;

    [SerializeField]
    private float boostFactor = 1f;

    [SerializeField]
    private Transform raycastTrans;

    [SerializeField]
    private float speedUpFactor = 3;

    [SerializeField]
    private float slowDownFactor = 4;

    /// <summary>How many degrees per second the car can turn</summary>
    [SerializeField]
    private float degreePerSecond;

    [SerializeField]
    private GameObject ultimate;

    /// <summary>The vehicles rigidbody</summary>
    private Rigidbody rb;

    /// <summary>The rotation goal</summary>
    private Vector2 goalRotate;

    /// <summary>Defines how fast the vehicle moves</summary>
    private float aktMovementSpeed;

    [SerializeField]
    private VehicleWheelControll[] wheels;

    [SerializeField]
    private ParticleSystem[] particleSystems;

    [SerializeField]
    private TrailRenderer[] trailRenderer;

    [SerializeField]
    private float minRamSpeed;

    [SerializeField]
    private bool safeDriving;

    /// <summary>The color which will be applied to conquered turrets</summary>
    public Color TeamColor { get; set; }

    public Text debugText;

    private PlayerNet playerNet;

    /// <summary>
    /// Use this for initialization
    /// </summary>    
    void Start() {
        playerNet = GetComponent<PlayerNet>();
        ConfigButton.ObjectsToUpdate.Add(this);
        rb = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>    
    void Update() {
        if (safeDriving) {
            SafeDriving(ultimate.activeSelf);
        }

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
        if (rotation != Vector2.zero && tractionModifier < float.Epsilon && rb.velocity.sqrMagnitude < 0.1f) {
            StartCoroutine(playerNet.InitRespawn(true));
        }

        if (rotation == Vector2.zero || tractionModifier < float.Epsilon) {
            VehicleWheelControll.UpdateWheelsTurn(0f, false);
            if (tractionModifier < float.Epsilon) {
                for (int i = 0; i < particleSystems.Length; i++) {
                    var em = particleSystems[i].emission;
                    em.enabled = false;
                }

                for (int i = 0; i < trailRenderer.Length; i++) {
                    trailRenderer[i].enabled = false;
                }
            }

            if(aktMovementSpeed >= 0)
            {
                aktMovementSpeed -= Time.deltaTime * slowDownFactor;
            }

            return;
        } else if(rotation != Vector2.zero && aktMovementSpeed <= maxMovementSpeed) {
            aktMovementSpeed += Time.deltaTime * speedUpFactor;
        }

        for (int i = 0; i < particleSystems.Length; i++) {
            var em = particleSystems[i].emission;
            em.enabled = true;
        }

        for (int i = 0; i < trailRenderer.Length; i++) {
            trailRenderer[i].enabled = true;
        }

        var flatAngle = transform.rotation.eulerAngles;
        goalRotate = rotation;
        var rotate = Vector2.SignedAngle(goalRotate, Vector2.up);
        var flatQuat = new Quaternion();
        flatQuat.eulerAngles = flatAngle;
        var steerValue = rotate - flatAngle.y;
        steerValue += steerValue < -180f ? 360f : 0f;
        steerValue -= steerValue > 180f ? 360f : 0f;
        var maxDegree = 45f;
        steerValue = Mathf.Clamp(steerValue, -maxDegree, maxDegree);
        VehicleWheelControll.UpdateWheelsTurn(steerValue, false);
        //VehicleWheelControll.UpdateWheelsTurn(Mathf.Max(-1f, Mathf.Min(1f, rotate / (degreePerSecond * rotation.magnitude * Time.deltaTime))), false);
        var quat = new Quaternion {
            eulerAngles = new Vector3(0, rotate, 0)
        };
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, quat, degreePerSecond * rotation.magnitude * Time.deltaTime);
        var newVelocity = transform.forward * rotation.magnitude * aktMovementSpeed * boostFactor;
        rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
    }

    public void Boost(float boostStrenght, float boostTime, bool isUltimate) {
        aktMovementSpeed = maxMovementSpeed;
        boostFactor = 1f + boostStrenght > boostFactor ? 1f + boostStrenght : boostFactor;
        PlayerNet.PlayerIsUsingBoost = true;
        if (isUltimate) {
            ultimate.SetActive(true);
            ramCollider.enabled = true;
        }
        StopCoroutine(ResetBoost(boostTime, isUltimate));
        StartCoroutine(ResetBoost(boostTime, isUltimate));
    }

    private IEnumerator ResetBoost(float boostTime, bool isUltimate) {
        yield return new WaitForSeconds(boostTime);
        if (isUltimate) {
            ultimate.SetActive(false);
            ramCollider.enabled = false;
        }
        boostFactor = 1f;
        PlayerNet.PlayerIsUsingBoost = false;
    }

    public void UpdateConfig() {
        aktMovementSpeed = ConfigButton.VehicleMGSpeed;
        degreePerSecond = ConfigButton.VehicleMGTuningSpeed;
    }

    public void OnCollisionEnter(Collision collision) {
        //var go = collision.collider.gameObject;
        //if (!go.CompareTag("Player") || rb.velocity.magnitude < minRamSpeed) {
        //    return;
        //}

        //VehicleWeapon.Kill(go);
    }

    private void SafeDriving(bool ultimateOn) {
        Debug.DrawRay(raycastTrans.position, raycastTrans.forward * 10, Color.red, 1f);
        if (!ultimateOn) {
            if (Physics.Raycast(raycastTrans.position, raycastTrans.forward, 2.5f, LayerMask.GetMask("Wall", "Default", "Turret"))) {
                aktMovementSpeed = 0;
            }
            else if (Physics.Raycast(raycastTrans.position, raycastTrans.forward, 3f, LayerMask.GetMask("Wall", "Default", "Turret"))) {
                aktMovementSpeed = 5;
            }
        } else {
            if (Physics.Raycast(raycastTrans.position, raycastTrans.forward, 2.5f, LayerMask.GetMask("Wall", "Default"))) {
                aktMovementSpeed = 0;
            }
            else if (Physics.Raycast(raycastTrans.position, raycastTrans.forward, 3f, LayerMask.GetMask("Wall", "Default"))) {
                aktMovementSpeed = 5;
            }
        }
    }
}