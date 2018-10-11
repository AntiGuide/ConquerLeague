using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour {

    [SerializeField]
    private Transform shootingPoint;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float bulletForce;
    [SerializeField]
    private float shootingTime;
    
    private GameObject[] playersGameObjects;
    private Transform[] players;
    private float aktShootingTime;

    // Use this for initialization
    void Start () {
        aktShootingTime = shootingTime;
        playersGameObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[playersGameObjects.Length];
        for (int i = 0; i < playersGameObjects.Length; i++) {
            players[i] = playersGameObjects[i].transform;
        }

        //Sort Players by Magnitude
        OrderByMagnitude(ref players, shootingPoint);
    }
	
	// Update is called once per frame
	void Update () {
        //Sort Players by Magnitude
        OrderByMagnitude(ref players, shootingPoint);
        aktShootingTime -= Time.deltaTime;
        if (aktShootingTime <= 0f) {
            ShootAtEnemy(players[0]); //Target (nearest)
            aktShootingTime += shootingTime;
        }

    }

    void OrderByMagnitude(ref Transform[] transforms, Transform referenceTransform) {
        var hasSorted = true;
        Transform switchTransform;
        for (int i = 0; hasSorted; i++) {
            hasSorted = false;
            if (transforms.Length <= 1) return;
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

    void ShootAtEnemy(Transform target) {
        Debug.DrawLine(shootingPoint.position, target.position, Color.red, 0.05f);
        var bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation, null);
        bullet.GetComponent<Rigidbody>().AddForce((target.position - bullet.transform.position) * bulletForce);
    }

}
