using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// The vehicles weapon, which defines its shootfrequency and what weapontype its using
/// </summary>
public class VehicleWeapon : MonoBehaviour
{
    /// <summary>The Weapontype the Vehicle is using</summary>
    [SerializeField] private GameObject weaponType;

    /// <summary>The position where the bullet spawns</summary>
    [SerializeField] private Transform shotSpawn;

    /// <summary>The magnitude of force which will be applied to the bullet</summary>
    [SerializeField] private float projectileSpeed;

    /// <summary>The players shootfrequency</summary>
    [SerializeField] private float shootingTime = 0.1f;

    /// <summary>Reference to the VehicleAim script on AimRange</summary>
    [SerializeField] private VehicleAim vehicleAim;

    /// <summary>Reference to the TeamHandler of this player</summary>
    [SerializeField] private TeamHandler teamHandler;

    /// <summary>The shot vfx</summary>
    [SerializeField] private GameObject shotVFX;

    /// <summary>The Vehicles VFX Systems</summary>
    private ParticleSystem[] vfxSystems;

    /// <summary>The shootingtimer</summary>
    private float aktShootingTime;

    /// <summary>The transform off all GameObjects with a turret tag</summary>
    private List<GameObject> turrets;

    /// <summary>Determines if the player that this weapon belongs to is an enemy</summary>
    private bool IsEnemy
    {
        get { return teamHandler.TeamID == TeamHandler.TeamState.ENEMY; }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        vfxSystems = GetComponentsInChildren<ParticleSystem>();

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
        if (IsEnemy) {
            return;
        }

        if (vehicleAim.AktAimingAt != null) {
            transform.LookAt(vehicleAim.AktAimingAt.transform);
            Shoot(weaponType);
        } else {
            transform.rotation = new Quaternion(0, 0, 0, 0);
            Shoot(weaponType);
        }
    }

    /// <summary>
    /// Instantiate Bullet and applies force
    /// </summary>
    /// <param name="weaponType"></param>
    void Shoot(GameObject weaponType) {
        if (aktShootingTime <= 0f && CrossPlatformInputManager.GetButton("Shoot")) {
            for (int i = 0; i < vfxSystems.Length; i++) {
                vfxSystems[i].Stop();
                vfxSystems[i].Play();
            }

            var shot = Instantiate(weaponType, shotSpawn.position, shotSpawn.rotation);
            shot.GetComponent<TeamHandler>().TeamID = teamHandler.TeamID;
            shot.GetComponent<Rigidbody>().AddForce(transform.forward * projectileSpeed);
            aktShootingTime += shootingTime;
        } else if (CrossPlatformInputManager.GetButton("Shoot")) {
            aktShootingTime -= Time.deltaTime;
        }
    }
}