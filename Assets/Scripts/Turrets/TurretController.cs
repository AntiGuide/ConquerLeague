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
    private Transform[] players;

    /// <summary>The time until the next shot occurs</summary>
    private float aktShootingTime;

    /// <summary>
    /// Orders given transforms by magnitude to another transform
    /// </summary>
    /// <param name="transforms">Transforms to order</param>
    /// <param name="referenceTransform">Transform for magnitude check</param>
    public static void OrderByMagnitude(ref Transform[] transforms, Transform referenceTransform) {
        var hasSorted = true;
        Transform switchTransform;
        for (int i = 0; hasSorted; i++) {
            hasSorted = false;
            if (transforms.Length <= 1) {
                return;
            }

            for (int i2 = 1; i2 < transforms.Length; i2++) {
                var dist1 = Vector3.SqrMagnitude(referenceTransform.position - transforms[i2 - 1].position);
                var dist2 = Vector3.SqrMagnitude(referenceTransform.position - transforms[i2].position);
                if (dist1 > dist2) {
                    switchTransform = transforms[i2 - 1];
                    transforms[i2 - 1] = transforms[i2];
                    transforms[i2] = switchTransform;
                    hasSorted = true;
                }
            }
        }
    }

    /// <summary>Use this for initialization</summary>
    void Start() {
        GameObject[] playersGameObjects;
        aktShootingTime = shootingTime;
        playersGameObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[playersGameObjects.Length];
        for (int i = 0; i < playersGameObjects.Length; i++) {
            players[i] = playersGameObjects[i].transform;
        }

        // Sort Players by Magnitude
        OrderByMagnitude(ref players, shootingPoint);
    }

    /// <summary>Update is called once per frame</summary>
    void Update() {
        // Sort Players by Magnitude
        OrderByMagnitude(ref players, shootingPoint);
        aktShootingTime -= Mathf.Min(Time.deltaTime, shootingTime);
        if (aktShootingTime <= 0f) {
            // Target (nearest)
            ShootAtEnemy(players[0]);
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
