using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The class handles the behaviour of the player object after network packets came in
/// </summary>
[RequireComponent(typeof(TeamHandler), typeof(Rigidbody), typeof(HitPoints))]
public class PlayerNet : MonoBehaviour, IConfigurable {
    [SerializeField] private int respawnTime = 5;

    [SerializeField]
    private ParticleSystem[] particleSystemsExhaust;

    /// <summary>Marks if the attached object is an enemy</summary>
    private bool isEnemy;

    /// <summary>The rigidbody of this object</summary>
    private Rigidbody rigidbodyPlayer;

    /// <summary>The reference to the attached hitpoint script</summary>
    private HitPoints hitPoints;

    /// <summary>The StartPoint to use for respawn</summary>
    public Transform StartPoint { get; set; }

    public static bool PlayerIsShooting { get; set; }

    public static bool PlayerIsUsingBoost { get; set; }

    public static bool EnemyIsShooting { get; set; }

    public static bool EnemyIsUsingBoost { get; set; }

    /// <summary>
    /// Handles new data from the network component
    /// </summary>
    /// <param name="position">The new positon of the player</param>
    /// <param name="quaternion">The new rotation of the player</param>
    /// <param name="velocity">The new velocity of the player</param>
    /// <param name="hp">The new hp of the player</param>
    public void SetNewMovementPack(Vector3 position, Quaternion quaternion, Vector3 velocity, byte hp, bool shooting, bool boosting) {
        transform.position = position;
        transform.rotation = quaternion;
        rigidbodyPlayer.velocity = velocity;
        if (hitPoints.AktHp != hp) {
            hitPoints.AktHp = hp;
        }

        EnemyIsShooting = shooting;
        EnemyIsUsingBoost = boosting;
    }

    /// <summary>
    /// Handles new data from the network component
    /// </summary>
    /// <param name="position">The new positon of the player</param>
    /// <param name="quaternion">The new rotation of the player</param>
    /// <param name="velocity">The new velocity of the player</param>
    public void SetNewMovementPack(Vector3 position, Quaternion quaternion, Vector3 velocity) {
        transform.position = position;
        transform.rotation = quaternion;
        rigidbodyPlayer.velocity = velocity;
        VehicleWheelControll.UpdateWheelsSpin(rigidbodyPlayer, true);
    }

    /// <summary>
    /// Triggered when this player dies
    /// </summary>
    public void OnDeath() {
        if (!isEnemy) {
            CommunicationNet.FakeStatic.SendPlayerDeath();
            KillfeedManager.FS.AddDeathEvent(TeamHandler.TeamState.ENEMY, KillfeedManager.DeathCategory.MG, TeamHandler.TeamState.FRIENDLY);
        } else {
            KillfeedManager.FS.AddDeathEvent(TeamHandler.TeamState.FRIENDLY, KillfeedManager.DeathCategory.MG, TeamHandler.TeamState.ENEMY);
        }

        StartCoroutine(InitRespawn());
    }

    /// <summary>
    /// Triggered when the death is triggered over network
    /// </summary>
    public void OnNetDeath() {
        StopCoroutine(InitRespawn());
        StartCoroutine(InitRespawn());
    }

    /// <summary>
    /// Initializes the respawn countdown
    /// </summary>
    public IEnumerator InitCountdown() {
        GameManager.FSGameManager.CountdownBackgroundImage.enabled = true;
        GameManager.FSGameManager.CountdownImage.enabled = true;
        for (int i = respawnTime; i > 0; i--) {
            GameManager.FSGameManager.CountdownImage.sprite = GameManager.FSGameManager.SpritesCountdown[i];
            yield return new WaitForSeconds(1f);
        }

        GameManager.FSGameManager.CountdownImage.enabled = false;
        GameManager.FSGameManager.CountdownBackgroundImage.enabled = false;
        yield return null;
    }

    public static void PlayerIsShootingUltimate() {
        PlayerIsShooting = false;
        CommunicationNet.FakeStatic.SendPlayerUltimate();
    }

    /// <summary>
    /// Initializes respawn
    /// </summary>
    public IEnumerator InitRespawn(bool resetOnly = false) {
        transform.position = StartPoint.position;
        transform.rotation = StartPoint.rotation;
        rigidbodyPlayer.velocity = Vector3.zero;
        if (!resetOnly) {
            //hitPoints.Visible = false;
            hitPoints.HealthBar.Active = false;
            transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().enabled = false;
            transform.GetChild(1).gameObject.SetActive(false);
            var savedLayer = gameObject.layer;
            gameObject.layer = 13;
            if (!isEnemy) {
                GetComponent<VehicleController>().enabled = false;
            }
        
            if (!isEnemy) {
                StartCoroutine(InitCountdown());
            }

            yield return new WaitForSeconds(respawnTime);
            hitPoints.SetFull();
            gameObject.layer = savedLayer;
            //hitPoints.Visible = true;
            hitPoints.HealthBar.Active = true;
            transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().enabled = true;
            transform.GetChild(1).gameObject.SetActive(true);
            if (!isEnemy) {
                GetComponent<VehicleController>().enabled = true;
            }
        }
    }

    /// <summary>
    /// Triggered if damage is taken. Handles health reduction.
    /// </summary>
    /// <param name="damage"></param>
    public void DamageTaken(byte damage) {
        //Debug.Log("Damage taken over network " + damage + " damage to " + hitPoints.gameObject.name + " (Full HP " + hitPoints.AktHp + "). TH: FRIENDLY");
        if (hitPoints.AktHp - damage <= hitPoints.AktHp && hitPoints.AktHp - damage > 0) {
            hitPoints.AktHp -= damage;
        } else {
            //Debug.Log("Damage recieved and HP set to 0!");
            hitPoints.AktHp = 0;
        }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        ConfigButton.ObjectsToUpdate.Add(this);
        isEnemy = GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.ENEMY;
        rigidbodyPlayer = GetComponent<Rigidbody>();
        hitPoints = GetComponent<HitPoints>();
    }

    private void Update() {
        for (int i = 0; i < particleSystemsExhaust.Length; i++) {
            var em = particleSystemsExhaust[i].emission;
            em.enabled = isEnemy ? EnemyIsUsingBoost : PlayerIsUsingBoost;
        }
    }

    /// <summary>
    /// FixedUpdate is called on physics refresh
    /// </summary>
    void FixedUpdate() {
        if (!isEnemy) {
            try {
                CommunicationNet.FakeStatic.SendPlayerMovement(transform, rigidbodyPlayer, hitPoints.AktHp, PlayerIsShooting, PlayerIsUsingBoost);
            } catch (Exception) {
                Debug.Log("CommunicationNet.FakeStatic.SendPlayerMovement(transform, rigidbodyPlayer); produced an error!");
                throw;
            }
        }
    }

    public void UpdateConfig() {
        respawnTime = ConfigButton.VehicleRespawnTime;
        hitPoints.SaveHp = ConfigButton.VehicleHP;
    }

    public void OnNetUltimate() {
        //Visual Ultimate for this player
    }
}
