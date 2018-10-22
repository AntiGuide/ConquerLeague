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
    private float shootingTime = 0.1f;

    /// <summary>Reference to the VehicleAim script on AimRange</summary>
    [SerializeField]
    private VehicleAim vehicleAim;

    /// <summary>The shootingtimer</summary>
    private float aktShootingTime;

    /// <summary>The transform off all GameObjects with a turret tag</summary>
    private List<GameObject> turrets;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        GameObject[] turretGameObjects;
        aktShootingTime = shootingTime;
        turretGameObjects = GameObject.FindGameObjectsWithTag("Turret");
        turrets = new List<GameObject>(turretGameObjects.Length);
        for (int i = 0; i < turretGameObjects.Length; i++) {
            turrets.Add(turretGameObjects[i]);
        }

        // Sort turrets by magnitude
        VehicleAim.OrderByMagnitude(ref turrets, shotSpawn);
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