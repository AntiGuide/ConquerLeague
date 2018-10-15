using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// The vehicles weapon, which defines its shootfrequency and what weapontype its using
/// </summary>
public class VehicleWeapon : MonoBehaviour {
    /// <summary>The Weapontype the Vehicle is using</summary>
    [SerializeField]
    private GameObject weaponType;

    /// <summary>The position where the bullet spawns</summary>
    [SerializeField]
    private Transform shotSpawn;

    /// <summary>The magnitude of force which will be applied to the bullet</summary>
    [SerializeField]
    private float projectileSpeed;

    /// <summary>The players shootfrequency</summary>
    [SerializeField]
    private float shootingTime = 0.5f;

    /// <summary>Reference to the VehicleAim script on AimRange</summary>
    [SerializeField]
    private VehicleAim vehicleAim;

    /// <summary>The shootingtimer</summary>
    private float aktShootingTime;

    /// <summary>The transform off all GameObjects with a turret tag</summary>
    private Transform[] turrets;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        GameObject[] turretGameObjects;
        aktShootingTime = shootingTime;
        turretGameObjects = GameObject.FindGameObjectsWithTag("Turret");
        turrets = new Transform[turretGameObjects.Length];
        for (int i = 0; i < turretGameObjects.Length; i++) {
            turrets[i] = turretGameObjects[i].transform;
        }

        // Sort turrets by magnitude
        OrderByMagnitude(ref turrets, shotSpawn);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if (vehicleAim.AktAimingAt != null) {
            transform.LookAt(vehicleAim.AktAimingAt.transform);
            Shoot(this.weaponType, vehicleAim.AktAimingAt.transform);
        }
    }

    /// <summary>
    /// Orders given transforms by magnitude to another transform
    /// </summary>
    /// <param name="transforms">Transforms to order</param>
    /// <param name="referenceTransform">Transform for magnitude check</param>
    void OrderByMagnitude(ref Transform[] transforms, Transform referenceTransform) {
        var hasSorted = true;
        Transform switchTransform;
        for (int i = 0; hasSorted; i++) {
            hasSorted = false;
            if (transforms.Length <= 1) {
                return;
            }

            for (int i2 = 1; i2 < transforms.Length; i2++) {
                var dist1 = Vector3.SqrMagnitude(referenceTransform.position - transforms[i2 - 1].position);
                var dist2 = Vector3.SqrMagnitude(referenceTransform.position - transforms[i2].position);
                if (dist1 > dist2) {
                    switchTransform = transforms[i2 - 1];
                    transforms[i2 - 1] = transforms[i2];
                    transforms[i2] = switchTransform;
                    hasSorted = true;
                }
            }
        }
    }

    /// <summary>
    /// Instantiate Bullet and applies force
    /// </summary>
    /// <param name="ammo"></param>
    void Shoot(GameObject ammo, Transform target) {
        if (aktShootingTime <= 0f && CrossPlatformInputManager.GetButton("Shoot")) {
            var shot = Instantiate(ammo, shotSpawn.position, new Quaternion(90, this.transform.rotation.y, 0, 0));
            shot.GetComponent<Rigidbody>().AddForce((target.position - shot.transform.position) * projectileSpeed);
            aktShootingTime += shootingTime;
        } else if (CrossPlatformInputManager.GetButton("Shoot")) {
            aktShootingTime -= Time.deltaTime;
        }
    }
}