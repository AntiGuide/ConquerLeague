using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Image))]
public class UltimateController : MonoBehaviour {
    public static UltimateController FS;

    [SerializeField]
    private VehicleWeapon vehicleWeapon;

    [SerializeField]
    private VehicleController vehicleController;

    [SerializeField]
    private float ultimateBoostStrength = 1.5f;

    [SerializeField]
    private float ultimateDuration = 1.5f;

    [SerializeField]
    private byte maxCharge = 3;

    [SerializeField]
    private Image image;

    private byte charge;
	
    public void AddCharge() {
        charge += charge < maxCharge ? (byte)1 : (byte)0;
        UpdateChargeUI(charge, maxCharge);
    }

    private void UseUltimate() {
        if (charge == maxCharge) {
            vehicleController.Boost(ultimateBoostStrength, ultimateDuration, true);
            //vehicleWeapon.Shoot(ultimateProjectilePrefab, true);
            charge = 0;
            UpdateChargeUI(charge, maxCharge);
        }
    }

    private void UpdateChargeUI(float charge, float maxCharge) {
        if (image == null) {
            return;
        }

        image.fillAmount = charge / maxCharge;
    }

	private void Start () {
        //image = GetComponent<Image>();
        if (FS != null) {
            Application.Quit();
        }

        FS = this;

#if UNITY_STANDALONE
        var go = GameObject.Find("/Canvas/UltiShootButton/Ultimeter");
        if (go == null) {
            Debug.Log("Ultimeter not found");
        }

        image = go.GetComponent<Image>();
#endif

        UpdateChargeUI(charge, maxCharge);
    }

    private void Update() {
        if (CrossPlatformInputManager.GetButtonDown("UltiShoot")) {
            UseUltimate();
        }
    }
}
