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

    [SerializeField]
    private float shakeFactor = 10f;

    private byte charge;

    private RectTransform rectTrans;

    private Vector2 startLocation;

    public float UltimateDuration {
        get {
            return ultimateDuration;
        }

        set {
            ultimateDuration = value;
        }
    }

    public void AddCharge() {
        charge += charge < maxCharge ? (byte)1 : (byte)0;
        UpdateChargeUI(charge, maxCharge);
    }

    private void UseUltimate() {
        if (charge == maxCharge) {
            vehicleController.Boost(ultimateBoostStrength, ultimateDuration, true);
            CommunicationNet.FakeStatic.SendPlayerUltimate();
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

        rectTrans = image.transform.parent.GetComponent<RectTransform>();
        startLocation = rectTrans.anchoredPosition;
    }

    private void Update() {
        if (CrossPlatformInputManager.GetButtonDown("UltiShoot")) {
            UseUltimate();
        }

        if (charge == maxCharge) {
            var x = startLocation.x + (Random.value * shakeFactor);
            var y = startLocation.y + (Random.value * shakeFactor);

            rectTrans.anchoredPosition = new Vector2(x, y);
        }
    }
}
