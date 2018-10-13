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

    /// <summary>The shootingtimer</summary>
    private float aktShootingTime;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        aktShootingTime = shootingTime;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        Shoot(this.weaponType);
    }

    /// <summary>
    /// Instantiate Bullet and applies force
    /// </summary>
    /// <param name="ammo"></param>
    void Shoot(GameObject ammo) {
        if (aktShootingTime <= 0f && CrossPlatformInputManager.GetButton("Shoot")) {
            var shot = Instantiate(ammo, shotSpawn.position, new Quaternion(90, this.transform.rotation.y, 0, 0));
            shot.GetComponent<Rigidbody>().AddForce(this.transform.forward * projectileSpeed);
            aktShootingTime += shootingTime;
        } else if (CrossPlatformInputManager.GetButton("Shoot")) {
            aktShootingTime -= Time.deltaTime;
        }
    }
}