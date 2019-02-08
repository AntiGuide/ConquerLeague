using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlls movement and shooting of a turret
/// </summary>
public class TurretController : MonoBehaviour, IConfigurable, ISideAware{
    private enum Position {
        LEFT = 0,
        MID,
        RIGHT
    }

    [SerializeField]
    private Position position;

    /// <summary>The time it takes for the turret to shoot again</summary>
    public float ShootingTime;

    [SerializeField]
    private GameObject destroyedTower;

    /// <summary></summary>
    [HideInInspector]
    public bool Respawning = false;

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

    [SerializeField]
    private byte damagePerShot = 15;

    /// <summary>All of the turrets renderer</summary>
    private MeshRenderer[] turretRenderers;

    /// <summary>The transform of all GameObjects with a player tag</summary>
    private List<GameObject> players;

    /// <summary>The time until the next shot occurs</summary>
    private float aktShootingTime;

    private float respawnRest;

    private bool flareSwapped = false;

    /// <summary>Use this for initialization</summary>
    void Start() {
        ConfigButton.ObjectsToUpdate.Add(this);
        aktShootingTime = ShootingTime;
        turretRenderers = turret.GetComponentsInChildren<MeshRenderer>();
        CommunicationNet.FakeStatic.sideAwares.Add(this);
    }

    /// <summary>Update is called once per frame</summary>
    void Update() {
        if (Respawning) {
            var respawnTimeTemp = (Time.deltaTime * respawnSpeed) + respawnRest;
            var roundedRespawnTimeTemp = Mathf.RoundToInt(respawnTimeTemp);
            respawnRest = respawnTimeTemp - roundedRespawnTimeTemp;
            hitPoints.AktHp += (byte)roundedRespawnTimeTemp;
            teamHandler.TeamID = TeamHandler.TeamState.NEUTRAL;

            if (!flareSwapped) {
                for (int i = 0; i < turretConquer.flare.Length; i++) {
                    turretConquer.flare[i].UpdateColor(teamHandler.TeamID);
                }
                flareSwapped = true;
            }

            for (int i = 1; i < turretRenderers.Length; i++) {
                turretRenderers[i].enabled = false;
            }

            destroyedTower.SetActive(true);

            if (hitPoints.AktHp >= hitPoints.SaveHp) {
                hitPoints.AktHp = hitPoints.SaveHp;
                for (int i = 1; i < turretRenderers.Length; i++) {
                    turretRenderers[i].enabled = true;
                }
                //print("hi");
                //turretConquer.Conquerable = true;
                turretConquer.ResetTowerNeutral();
                destroyedTower.SetActive(false);
                Respawning = false;
                flareSwapped = false;
            }
        }

        // Sort Players by Magnitude
        if (!turretConquer.Conquerable) {
            if (turretAim.AktAimingAt != null && turretAim.Locked) {
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
            if (teamHandler.TeamID == TeamHandler.TeamState.FRIENDLY) {
                CommunicationNet.FakeStatic.SendPlayerDamage(damagePerShot);
                target.gameObject.GetComponent<HitPoints>().AktHp -= damagePerShot;
            }
            SoundController.FSSoundController.StartSound(SoundController.Sounds.TOWER_MG);
            var bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation, null);
            bullet.GetComponent<TeamHandler>().TeamID = teamHandler.TeamID;
            bullet.GetComponent<Rigidbody>().AddForce((target.position - bullet.transform.position) * bulletForce);
            bullet.GetComponent<Standard_Projectile>().Damage = 0;
            aktShootingTime += ShootingTime;
            aktShootingTime = Mathf.Min(aktShootingTime, ShootingTime / 2f);
        }
    }

    public void UpdateConfig() {
        hitPoints.SaveHp = ConfigButton.TowerHP;
        ShootingTime = 1 / ConfigButton.TowerShotsPerSecond;
        damagePerShot = ConfigButton.TowerDamagePerShot;
        respawnSpeed = hitPoints.SaveHp / ConfigButton.TowerRespawnTime;
    }

    public void InitialUpdateColor() {
        switch (position) {
            case Position.LEFT:
                turretConquer.BuildTurretNoReward(GameManager.LeftTeam ?? TeamHandler.TeamState.NEUTRAL);
                break;
            case Position.MID:
                turretConquer.BuildTurretNoReward(TeamHandler.TeamState.NEUTRAL);
                break;
            case Position.RIGHT:
                turretConquer.BuildTurretNoReward(GameManager.RightTeam ?? TeamHandler.TeamState.NEUTRAL);
                break;
            default:
                break;
        }
    }
}
