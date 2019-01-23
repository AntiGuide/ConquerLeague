using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMG : MonoBehaviour {
    /// <summary>True if the weapon is locked on a target</summary>
    [HideInInspector]
    public bool Locked = false;

    /// <summary>The bullet prefab to spawn</summary>
    [SerializeField]
    private GameObject bulletPrefab;

    /// <summary>The transform from which the turret shoots and the range calculation is done</summary>
    [SerializeField]
    private Transform shootingPoint;

    [SerializeField]
    private byte damagePerShot = 15;

    /// <summary>Force for bullets is multiplied by this</summary>
    [SerializeField]
    private float bulletForce;

    /// <summary>References the teamhandler script attached to this gameobject</summary>
    private TeamHandler teamHandler;

    /// <summary>List of all the GameObjects tagged with a shootable tag in range</summary>
    private List<GameObject> shootablesInRange = new List<GameObject>();

    /// <summary>Saves the current weapon rotation</summary>
    private Quaternion saveRotation;

    /// <summary>The weapons lock-on progress</summary>
    private float progress = 0f;

    /// <summary>The time it takes for the turret to shoot again</summary>
    public float ShootingTime;

    /// <summary>The time until the next shot occurs</summary>
    private float aktShootingTime;

    /// <summary>The shootable that is being aimed at</summary>
    public GameObject AktAimingAt
    {
        get { return shootablesInRange.Count >= 1 ? shootablesInRange[0] : null; }
    }

    // Use this for initialization
    void Start () {
        aktShootingTime = ShootingTime;
        teamHandler = GetComponent<TeamHandler>();
        saveRotation = transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {
        if (shootablesInRange.Count >= 1) {
            progress = Locked ? 1f : progress + Time.deltaTime / ShootingTime;
            transform.rotation = Quaternion.Lerp(saveRotation, Quaternion.LookRotation(transform.position - shootablesInRange[0].transform.position), progress);
            var newRotation = transform.rotation.eulerAngles;
            newRotation.x = 0f;
            newRotation.z = 0f;
            transform.rotation = Quaternion.Euler(newRotation);
            if (progress >= 1) {
                Locked = true;
            }
        }

        if (AktAimingAt != null && Locked) {
            ShootAtEnemy(AktAimingAt.transform);
        } else {
            return;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player") && teamHandler.TeamID != other.gameObject.GetComponent<TeamHandler>().TeamID) {
            shootablesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        shootablesInRange.Remove(other.gameObject);
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
}