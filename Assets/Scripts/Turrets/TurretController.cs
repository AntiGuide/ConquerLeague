using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlls movement and shooting of a turret
/// </summary>
public class TurretController : MonoBehaviour {
    /// <summary>References the turret-gamobject</summary>
    [SerializeField]
    private GameObject turret;
    
    /// <summary>References the turrets hitpoints script</summary>
    [SerializeField]
    private HitPoints hitPoints;

    /// <summary>The bullet prefab to spawn</summary>
    [SerializeField]
    private GameObject bulletPrefab;

    /// <summary>Force for bullets is multiplied by this</summary>
    [SerializeField]
    private float bulletForce;

    /// <summary>The time it takes for the turret to shoot again</summary>
    [SerializeField]
    private float shootingTime;

    /// <summary>The transform from which the turret shoots and the range calculation is done</summary>
    [SerializeField]
    private Transform shootingPoint;

    /// <summary>Reference to the TurretAim script</summary>
    [SerializeField]
    private TurretAim turretAim;

    /// <summary>Reference to the TurretConquer script</summary>
    [SerializeField]
    private TurretConquer turretConquer;

    /// <summary>References the turrets Teamhandler script</summary>
    [SerializeField]
    private TeamHandler teamHandler;

    /// <summary>Defines how fast the turret will be build</summary>
    [SerializeField]
    private float respawnSpeed = 20f;

    /// <summary>All of the turrets renderer</summary>
    private MeshRenderer[] renderer;

    /// <summary>The transform of all GameObjects with a player tag</summary>
    private List<GameObject> players;

    /// <summary>The time until the next shot occurs</summary>
    private float aktShootingTime;

    //[HideInInspector]
    public bool respawning = false;

    /// <summary>Use this for initialization</summary>
    void Start() {
        aktShootingTime = shootingTime;
        renderer = turret.GetComponentsInChildren<MeshRenderer>();
    }

    /// <summary>Update is called once per frame</summary>
    void Update() {
        if (respawning) {
            hitPoints.AktHp += Time.deltaTime * respawnSpeed;
            teamHandler.TeamID = TeamHandler.TeamState.NEUTRAL;
            renderer[0].enabled = true;
            turretConquer.conquerable = true;

            for(int i = 1; i < renderer.Length; i++) {
                renderer[i].enabled = false;
            }

            if (hitPoints.AktHp > hitPoints.saveHp) {
                hitPoints.AktHp = hitPoints.saveHp;
                renderer[0].enabled = false;
                for (int i = 1; i < renderer.Length; i++) {
                    renderer[i].enabled = true;
                }
                respawning = false;
            }
        }

        // Sort Players by Magnitude
        if (!turretConquer.Conquerable) {
            if (turretAim.AktAimingAt != null) {
                ShootAtEnemy(turretAim.AktAimingAt.transform);
            } else {
                return;
            }
        }
    }

    /// <summary>
    /// Shoots at the targeted transform
    /// </summary>
    /// <param name="target">The target to shoot at</param>
    void ShootAtEnemy(Transform target) {
        aktShootingTime -= Time.deltaTime;
        if (aktShootingTime <= 0f) {
            Debug.DrawLine(shootingPoint.position, target.position, Color.red, 0.05f);
            var bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation, null);
            bullet.GetComponent<TeamHandler>().TeamID = teamHandler.TeamID;
            bullet.GetComponent<Rigidbody>().AddForce((target.position - bullet.transform.position) * bulletForce);
            aktShootingTime += shootingTime;
            aktShootingTime = Mathf.Min(aktShootingTime, shootingTime / 2f);
        }
    }
}
