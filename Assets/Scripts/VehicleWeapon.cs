using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class VehicleWeapon : MonoBehaviour {

    [SerializeField]
    private GameObject weaponType;

    [SerializeField]
    private Transform shotSpawn;

    [SerializeField]
    private float projectileSpeed;

    [SerializeField]
    private float shootingTime = 0.5f;

    private float aktShootingTime;

	// Use this for initialization
	void Start () {
        aktShootingTime = shootingTime;
	}
	
	// Update is called once per frame
	void Update () {
        Shoot(weaponType);
	}

    void Shoot(GameObject ammo) {
        if(aktShootingTime <= 0f && (CrossPlatformInputManager.GetButton("Shoot"))) {
            var shot = Instantiate(ammo, shotSpawn.position, ammo.transform.rotation);
            shot.GetComponent<Rigidbody>().AddForce(this.transform.forward*projectileSpeed);
            aktShootingTime += shootingTime;
        }else if (CrossPlatformInputManager.GetButton("Shoot")) {
            aktShootingTime -= Time.deltaTime;
        }
    }
}
