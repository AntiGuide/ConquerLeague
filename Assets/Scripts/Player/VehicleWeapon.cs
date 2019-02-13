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

    [SerializeField] private Transform carTrans;

    [SerializeField] private byte damagePerShot;

    [SerializeField] private byte ultimateDamage;

    [SerializeField] private float overheatPerShot;

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

        if (CrossPlatformInputManager.GetButton("Shoot") &&
            OverheatManager.FS.state == OverheatManager.HeatState.SHOOTABLE) {
            PlayerNet.PlayerIsShooting = true;
            Shoot(weaponType);
        } else {
            PlayerNet.PlayerIsShooting = false;
        }
    }

    public void Shoot(GameObject projectilePrefab) {
        if (aktShootingTime > 0f) {
            aktShootingTime -= Time.deltaTime;
            return;
        }

         aktShootingTime += shootingTime;
         OverheatManager.FS.ShootFired();
         OverheatManager.FS.OverheatPercentage += overheatPerShot;

            SoundController.FSSoundController.StartSound(SoundController.Sounds.MG_SHOT);
        for (int i = 0; i < vfxSystems.Length; i++) {
            vfxSystems[i].Stop();
            vfxSystems[i].Play();
        }

        FireVisualShot(projectilePrefab, shotSpawn.position, shotSpawn.rotation, teamHandler.TeamID, projectileSpeed);
        ApplyDamageDirectly(vehicleAim.AktAimingAt, damagePerShot);
    }

    private void FireVisualShot(GameObject prefab, Vector3 shotSpawnPosition, Quaternion shotSpawnRotation, TeamHandler.TeamState tID, float speed) {
        var shot = Instantiate(prefab, shotSpawnPosition, shotSpawnRotation);
        shot.GetComponent<Transform>().localEulerAngles = carTrans.localEulerAngles - new Vector3(0, -90, 0);
        shot.GetComponent<TeamHandler>().TeamID = tID;
        shot.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        shot.GetComponent<Standard_Projectile>().Damage = 0;
    }

    private void ApplyDamageDirectly(GameObject target, byte damage) {
        var hp = target?.GetComponent<HitPoints>();
        if (hp == null ||
            target.gameObject.GetComponent<TeamHandler>().TeamID == teamHandler.TeamID ||
            damage == 0) {
            return;
        }
        
        damage = (byte)Mathf.Min(damage, hp.AktHp);
        switch (target.tag) {
            case "Player":
                CommunicationNet.FakeStatic.SendPlayerDamage(damage);
                StartCoroutine(Blink());
                break;
            case "Turret":
                CommunicationNet.FakeStatic.SendTowerDamage(target.GetComponent<TowerNet>().ID, damage);
                break;
            case "Minion":
                StartCoroutine(Blink());
                break;
            default:
                return;
        }

        hp.LastDamager = HitPoints.Damager.PLAYER_MG;
        hp.AktHp -= damage;
    }

    public static void Kill(GameObject gameObject) {
        var hp = gameObject?.GetComponent<HitPoints>();
        if (hp == null) {
            return;
        }

        switch (gameObject.tag) {
            case "Player":
                CommunicationNet.FakeStatic.SendPlayerDamage(hp.AktHp);
                break;
            case "Turret":
                CommunicationNet.FakeStatic.SendTowerDamage(gameObject.GetComponent<TowerNet>().ID, hp.AktHp);
                break;
            case "Minion":
                break;
            default:
                return;
        }

        hp.AktHp -= hp.AktHp;
    }

    public void UpdateConfig() {
        shootingTime = 1f / (float)ConfigButton.VehicleMGShotsPerSecond;
        damagePerShot = IsEnemy ? (byte)0 : ConfigButton.VehicleMGDamagePerShot;
        overheatPerShot = IsEnemy ? (byte)0 : ConfigButton.VehicleMGOverheatPerShot;
    }

    private IEnumerator Blink() {
        MeshRenderer[] ren = vehicleAim.AktAimingAt.GetComponentsInChildren<MeshRenderer>();
        for(int i = 0; i < ren.Length; i++) {
            ren[i].material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < ren.Length; i++) {
            if (ren[i] != null) {
                ren[i].material.color = Color.white;
            }
        }
    }
}