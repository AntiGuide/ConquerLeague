using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlls movement and shooting of a turret
/// </summary>
public class TurretController : MonoBehaviour {
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

    /// <summary>The transform off all GameObjects with a player tag</summary>
    private List<GameObject> players;

    /// <summary>The time until the next shot occurs</summary>
    private float aktShootingTime;

    /// <summary>Use this for initialization</summary>
    void Start() {
        GameObject[] playersGameObjects;
        aktShootingTime = shootingTime;
        playersGameObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new List<GameObject>(playersGameObjects.Length);
        for (int i = 0; i < playersGameObjects.Length; i++) {
            players.Add(playersGameObjects[i]);
        }

        // Sort Players by Magnitude
        VehicleAim.OrderByMagnitude(ref players, shootingPoint);
    }

    /// <summary>Update is called once per frame</summary>
    void Update() {
        // Sort Players by Magnitude
        VehicleAim.OrderByMagnitude(ref players, shootingPoint);
        aktShootingTime -= Mathf.Min(Time.deltaTime, shootingTime);
        if (aktShootingTime <= 0f) {
            // Target (nearest)
            ShootAtEnemy(players[0].transform);
            aktShootingTime += shootingTime;
        }
    }

    /// <summary>
    /// Shoots at the targeted transform
    /// </summary>
    /// <param name="target">The target to shoot at</param>
    void ShootAtEnemy(Transform target) {
        Debug.DrawLine(shootingPoint.position, target.position, Color.red, 0.05f);
        var bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation, null);
        bullet.GetComponent<Rigidbody>().AddForce((target.position - bullet.transform.position) * bulletForce);
    }
}
