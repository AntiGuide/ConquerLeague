using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverheatManager : MonoBehaviour, IConfigurable {
    public static OverheatManager FS;

    [HideInInspector]
    public HeatState state = HeatState.SHOOTABLE;

    [SerializeField]
    private Image overheatFullImage;

    [SerializeField]
    private float cooldownPerSecond;

    private float cooldownAfter = 1f;

    private float overheatPercentage;

    private float lastShot;

    private bool inactivityCooldown = true;

    private float blinkingImageTimer;

    public enum HeatState {
        SHOOTABLE,
        COOLING
    }

    public float OverheatPercentage {
        get {
            return overheatPercentage;
        }

        set {
            if (state == HeatState.SHOOTABLE) {
                overheatPercentage = Mathf.Min(Mathf.Max(value, 0f), 1f);
                overheatFullImage.fillAmount = overheatPercentage;
                if (overheatPercentage >= 1f - float.Epsilon) {
                    overheatPercentage = 1f;
                    state = HeatState.COOLING;
                }
            }
        }
    }

    public void ShootFired() {
        StopAllCoroutines();
        inactivityCooldown = false;
        StartCoroutine(ResetShootingState());
    }

    private IEnumerator ResetShootingState() {
        yield return new WaitForSeconds(cooldownAfter);
        inactivityCooldown = true;
    }

    private void Start() {
        if (FS != null) {
            Application.Quit();
        }

        FS = this;
        overheatPercentage = 0f;
        overheatFullImage.fillAmount = overheatPercentage;
        ConfigButton.ObjectsToUpdate.Add(this);
    }

    private void Update() {
        if(state == HeatState.COOLING) {
            blinkingImageTimer += Time.deltaTime;
            overheatFullImage.color = new Color(Mathf.PingPong(Time.time, 1f), 0f, 0f);
        } else {
            blinkingImageTimer = 0;
            overheatFullImage.color = Color.white;
        }

        if (state == HeatState.COOLING || (inactivityCooldown && state == HeatState.SHOOTABLE)) {
            overheatPercentage -= cooldownPerSecond * Time.deltaTime;
            if (overheatPercentage <= 0f) {
                overheatPercentage = 0f;
                state = HeatState.SHOOTABLE;
            }

            overheatFullImage.fillAmount = overheatPercentage;
        }
    }

    public void UpdateConfig() {
        cooldownPerSecond = ConfigButton.VehicleMGCooldownPerSecond;
        cooldownAfter = ConfigButton.VehicleMGCooldownDelay;
    }
}
