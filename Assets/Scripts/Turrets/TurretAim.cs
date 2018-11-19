using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to make the turret aim at his prioritized target
/// </summary>
public class TurretAim : MonoBehaviour {
    /// <summary>The range to target shootables in</summary>
    [SerializeField, Header("Customizable"), Tooltip("The Range in UnityUnits/meters that the turret is allowed to target")]
    private float targetRange;

    /// <summary>Tags that are recognized as shootables. Priority top to bottom.</summary>
    [SerializeField, Tooltip("Tags that are recognized as shootables. Priority top to bottom.")]
    private string[] shootablesTags;

    /// <summary> The last target during last frame</summary>
    private GameObject lastTargeted;

    /// <summary>References the teamhandler script attached to this gameobject</summary>
    [SerializeField]
    private TeamHandler teamHandler;

    [SerializeField]
    private TurretConquer turretConquer;

    /// <summary>List of all the GameObjects tagged with a shootable tag in range</summary>
    private List<GameObject> shootablesInRange = new List<GameObject>();

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

        if (turretConquer.conquerable) {
            shootablesInRange.Clear();
        }

        if (shootablesInRange.Count > 1) {
            OrderByPriority(ref shootablesInRange);
        }

        if (lastTargeted == AktAimingAt) { // Situation didnt change
            return;
        }

        if (AktAimingAt == null && lastTargeted != null) { // Not targeting anything anymore
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
        if (Array.IndexOf(shootablesTags, other.tag) > -1 && teamHandler.TeamID !=other.gameObject.GetComponent<TeamHandler>().TeamID && !turretConquer.conquerable) {
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
