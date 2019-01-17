using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// The vehicles weapon, which defines its shootfrequency and what weapontype its using
/// </summary>
public class VehicleWeapon : MonoBehaviour, IConfigurable {
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

    [SerializeField] private byte damagePerShot;

    [SerializeField] private byte ultimateDamage;

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

    public TeamHandler TeamHandler {
        get { return teamHandler; }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        ConfigButton.ObjectsToUpdate.Add(this);
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
            if (vehicleAim.AktAimingAt != null) {
                transform.LookAt(vehicleAim.AktAimingAt.transform);
            }

            if (PlayerNet.EnemyIsShooting) {
                Shoot(weaponType);
            }

            return;
        }

        if (vehicleAim.AktAimingAt != null) {
            transform.LookAt(vehicleAim.AktAimingAt.transform);
        } else {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        if (CrossPlatformInputManager.GetButton("UltiShoot")) {
            PlayerNet.PlayerIsShootingUltimate();
            Shoot(weaponType, true);
            return;
        }

        if (CrossPlatformInputManager.GetButton("Shoot")) {
            PlayerNet.PlayerIsShooting = true;
            Shoot(weaponType);
        } else {
            PlayerNet.PlayerIsShooting = false;
        }
    }

    void Shoot(GameObject weaponType, bool isUltimate = false) {
        if (aktShootingTime > 0f) {
            aktShootingTime -= Time.deltaTime;
            return;
        }
        
        SoundController.FSSoundController.StartSound(SoundController.Sounds.MG_SHOT);
        for (int i = 0; i < vfxSystems.Length; i++) {
            vfxSystems[i].Stop();
            vfxSystems[i].Play();
        }

        FireVisualShot(weaponType, shotSpawn.position, shotSpawn.rotation, teamHandler.TeamID, projectileSpeed);
        aktShootingTime += shootingTime;
        var damage = isUltimate ? ultimateDamage : damagePerShot;
        ApplyDamageDirectly(vehicleAim.AktAimingAt, damage);
    }

    private void FireVisualShot(GameObject prefab, Vector3 shotSpawnPosition, Quaternion shotSpawnRotation, TeamHandler.TeamState tID, float speed) {
        var shot = Instantiate(prefab, shotSpawnPosition, shotSpawnRotation);
        shot.GetComponent<TeamHandler>().TeamID = tID;
        shot.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        shot.GetComponent<Standard_Projectile>().Damage = 0;
    }

    private void ApplyDamageDirectly(GameObject target, byte damage) {
        if (target?.GetComponent<HitPoints>() == null ||
            target.gameObject.GetComponent<TeamHandler>().TeamID == teamHandler.TeamID) {
            return;
        }
        
        switch (target.tag) {
            case "Player":
                CommunicationNet.FakeStatic.SendPlayerDamage(damage);
                target.gameObject.GetComponent<HitPoints>().AktHp -= damage;
                StartCoroutine(Blink());
                break;
            case "Turret":
                var id = target.GetComponent<TowerNet>().ID;
                CommunicationNet.FakeStatic.SendTowerDamage(id, damage);
                target.gameObject.GetComponent<HitPoints>().AktHp -= damage;
                break;
            case "Minion":
                StartCoroutine(Blink());
                target.gameObject.GetComponent<HitPoints>().AktHp -= damage;
                break;
            default:
                break;
        }
    }

    public void UpdateConfig() {
        shootingTime = 1 / ConfigButton.VehicleMGShotsPerSecond;
        damagePerShot = ConfigButton.VehicleMGDamagePerShot;
    }

    private IEnumerator Blink() {
        MeshRenderer[] ren = vehicleAim.AktAimingAt.GetComponentsInChildren<MeshRenderer>();
        for(int i = 0; i < ren.Length; i++) {
            ren[i].material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < ren.Length; i++) {
            ren[i].material.color = Color.white;
        }
    }
}