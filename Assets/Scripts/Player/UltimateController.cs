using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Image))]
public class UltimateController : MonoBehaviour {
    [SerializeField]
    private GameObject ultimateProjectilePrefab;

    [SerializeField]
    private VehicleWeapon vehicleWeapon;

    private Image image;

    private byte maxCharge;

    private byte charge;
	
    public void AddCharge() {
        charge += charge < maxCharge ? (byte)1 : (byte)0;
        UpdateChargeUI(charge, maxCharge);
    }

    private void UseUltimate() {
        vehicleWeapon.Shoot(ultimateProjectilePrefab, true);
        charge = 0;
        UpdateChargeUI(charge, maxCharge);
    }

    private void UpdateChargeUI(float charge, float maxCharge) {
        image.fillAmount = charge / maxCharge;
    }

	private void Start () {
        image = GetComponent<Image>();
        if (image.type != Image.Type.Filled) {
            Application.Quit();
        }
	}

    private void Update() {
        if (CrossPlatformInputManager.GetButton("UltiShoot")) {
            UseUltimate();
        }
    }
}
