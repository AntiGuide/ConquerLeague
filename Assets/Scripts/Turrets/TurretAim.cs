using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to make the turret aim at his prioritized target
/// </summary>
public class TurretAim : MonoBehaviour {
    /// <summary>True if the weapon is locked on a target</summary>
    [HideInInspector]
    public bool Locked = false;

    /// <summary>The range to target shootables in</summary>
    [SerializeField, Header("Customizable"), Tooltip("The Range in UnityUnits/meters that the turret is allowed to target")]
    private float targetRange;

    /// <summary>Tags that are recognized as shootables. Priority top to bottom.</summary>
    [SerializeField, Tooltip("Tags that are recognized as shootables. Priority top to bottom.")]
    private string[] shootablesTags;

    /// <summary>The turrets weapon</summary>
    [SerializeField]
    private GameObject weapon;

    /// <summary> The last target during last frame</summary>
    private GameObject lastTargeted;

    /// <summary>References the teamhandler script attached to this gameobject</summary>
    [SerializeField]
    private TeamHandler teamHandler;

    /// <summary>References the the turrets TurretConquer script</summary>
    [SerializeField]
    private TurretConquer turretConquer;

    /// <summary>References the the turrets TurretController script</summary>
    [SerializeField]
    private TurretController turretController;

    /// <summary>List of all the GameObjects tagged with a shootable tag in range</summary>
    private List<GameObject> shootablesInRange = new List<GameObject>();

    /// <summary>Saves the current weapon rotation</summary>
    private Quaternion saveRotation;

    /// <summary>The weapons lock-on progress</summary>
    private float progress = 0f;

    /// <summary>The shootable that is being aimed at</summary>
    public GameObject AktAimingAt
    {
        get { return shootablesInRange.Count >= 1 ? shootablesInRange[0] : null; }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        gameObject.GetComponent<SphereCollider>().radius = targetRange;
        saveRotation = weapon.transform.rotation;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        // Checks if gameobjects in the shootablesInRange-List are destroyed and removes them
        for (int i = 0; i < shootablesInRange.Count; i++) {
            if (shootablesInRange[i] == null || shootablesInRange[i].Equals(null)) {
                shootablesInRange.RemoveAt(i);
            }
        }

        if (shootablesInRange.Count >= 1) {
            progress = Locked ? 1f : progress + Time.deltaTime / turretController.ShootingTime;
            weapon.transform.rotation = Quaternion.Lerp(saveRotation, Quaternion.LookRotation(weapon.transform.position - shootablesInRange[0].transform.position), progress);
            var newRotation = weapon.transform.rotation.eulerAngles;
            newRotation.x = 0f;
            newRotation.z = 0f;
            weapon.transform.rotation = Quaternion.Euler(newRotation);
            if (progress >= 1) {
                Locked = true;
            }
        }

        if (turretConquer.Conquerable) {
            shootablesInRange.Clear();
        }

        if (shootablesInRange.Count > 1) {
            OrderByPriority(ref shootablesInRange);
        }

        if (lastTargeted == AktAimingAt) { // Situation didnt change
            return;
        }

        if (AktAimingAt == null && lastTargeted != null) { // Not targeting anything anymore
            saveRotation = weapon.transform.rotation;
            progress = 0;
            Locked = false;
            lastTargeted = null;
            return;
        }

        if (AktAimingAt != null) { // Changed target
            lastTargeted = AktAimingAt;
        }
    }

    /// <summary>
    /// Triggered if something enters an attached collider set to IsTrigger
    /// </summary>
    /// <param name="other">The collider that entered the collider</param>
    private void OnTriggerEnter(Collider other) {
        if (Array.IndexOf(shootablesTags, other.tag) > -1 && teamHandler.TeamID != other.gameObject.GetComponent<TeamHandler>().TeamID && !turretConquer.Conquerable) {
            shootablesInRange.Add(other.gameObject);
        }
    }

    /// <summary>
    /// Triggered if something exits an attached collider set to IsTrigger
    /// </summary>
    /// <param name="other">The collider that exited the collider</param>
    private void OnTriggerExit(Collider other) {
        if (Array.IndexOf(shootablesTags, other.tag) > -1) {
            shootablesInRange.Remove(other.gameObject);
        }
    }

    /// <summary>
    /// Orders GameObjects in referenced List shootablesInRange by priority obtained trough tags from shootablesTags
    /// </summary>
    /// <param name="shootablesInRange">These GameObjects are ordered</param>
    private void OrderByPriority(ref List<GameObject> shootablesInRange) {
        var hasSorted = true;
        GameObject switchGameObject;
        for (int i = 0; hasSorted; i++) {
            hasSorted = false;
            if (shootablesInRange.Count <= 1) {
                return;
            }

            for (int i2 = 1; i2 < shootablesInRange.Count; i2++) {
                var intComp1 = Array.IndexOf(shootablesTags, shootablesInRange[i2 - 1].tag);
                var intComp2 = Array.IndexOf(shootablesTags, shootablesInRange[i2].tag);
                if (intComp1 > intComp2) {
                    switchGameObject = shootablesInRange[i2 - 1];
                    shootablesInRange[i2 - 1] = shootablesInRange[i2];
                    shootablesInRange[i2] = switchGameObject;
                    hasSorted = true;
                }
            }
        }
    }
}
