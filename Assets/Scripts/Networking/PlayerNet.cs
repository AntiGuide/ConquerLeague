using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The class handles the behaviour of the player object after network packets came in
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerNet : MonoBehaviour, IConfigurable {
    [SerializeField] private int respawnTime = 5;

    /// <summary>Marks if the attached object is an enemy</summary>
    private bool isEnemy;

    /// <summary>The rigidbody of this object</summary>
    private Rigidbody rigidbodyPlayer;

    /// <summary>The reference to the attached hitpoint script</summary>
    private HitPoints hitPoints;

    /// <summary>The StartPoint to use for respawn</summary>
    public Transform StartPoint { get; set; }

    public static bool PlayerIsShooting { get; set; }

    public static bool EnemyIsShooting { get; set; }

    /// <summary>
    /// Handles new data from the network component
    /// </summary>
    /// <param name="position">The new positon of the player</param>
    /// <param name="quaternion">The new rotation of the player</param>
    /// <param name="velocity">The new velocity of the player</param>
    /// <param name="hp">The new hp of the player</param>
    public void SetNewMovementPack(Vector3 position, Quaternion quaternion, Vector3 velocity, byte hp, bool shooting) {
        transform.position = position;
        transform.rotation = quaternion;
        rigidbodyPlayer.velocity = velocity;
        hitPoints.AktHp = hp;
        EnemyIsShooting = shooting;
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
    }

    /// <summary>
    /// Triggered when this player dies
    /// </summary>
    public void OnDeath() {
        if (!isEnemy) {
            CommunicationNet.FakeStatic.SendPlayerDeath();
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

    /// <summary>
    /// Initializes respawn
    /// </summary>
    public IEnumerator InitRespawn() {
        hitPoints.Visible = false;
        transform.position = StartPoint.position;
        transform.rotation = StartPoint.rotation;
        transform.GetChild(0).gameObject.SetActive(false);
        var savedLayer = gameObject.layer;
        gameObject.layer = 13;
        if (!isEnemy) {
            GetComponent<VehicleController>().enabled = false;
        }
        
        if (!isEnemy) {
            StartCoroutine(InitCountdown());
        }

        hitPoints.SetFull();
        yield return new WaitForSeconds(respawnTime);
        hitPoints.Visible = true;
        transform.GetChild(0).gameObject.SetActive(true);
        gameObject.layer = savedLayer;
        if (!isEnemy) {
            GetComponent<VehicleController>().enabled = true;
        }
    }

    /// <summary>
    /// Triggered if damage is taken. Handles health reduction.
    /// </summary>
    /// <param name="damage"></param>
    public void DamageTaken(byte damage) {
        if (hitPoints.AktHp - damage <= hitPoints.AktHp && hitPoints.AktHp - damage > 0) {
            hitPoints.AktHp -= damage;
        } else {
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

    /// <summary>
    /// FixedUpdate is called on physics refresh
    /// </summary>
    void FixedUpdate() {
        if (!isEnemy) {
            try {
                CommunicationNet.FakeStatic.SendPlayerMovement(transform, rigidbodyPlayer, hitPoints.AktHp, PlayerIsShooting);
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
}
