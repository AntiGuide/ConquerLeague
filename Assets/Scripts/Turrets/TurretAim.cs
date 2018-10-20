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

    /// <summary>List of all the GameObjects tagged with a shootable tag in range</summary>
    private List<GameObject> shootablesInRange = new List<GameObject>();

    /// <summary>The shootable that is being aimed at</summary>
    public GameObject AktAimingAt
    {
        get { return shootablesInRange.Count >= 1 ? shootablesInRange[0] : null; }
    }

    // Use this for initialization
    void Start () {
        gameObject.GetComponent<SphereCollider>().radius = targetRange;
	}
	
	// Update is called once per frame
	void Update () {
        OrderByPriority(ref shootablesInRange);

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

    private void OnTriggerEnter(Collider other) {
        if(Array.IndexOf(shootablesTags, other.tag) > -1) {
            shootablesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(Array.IndexOf(shootablesTags, other.tag) > -1) {
            shootablesInRange.Remove(other.gameObject);
        }
    }

    private void OrderByPriority(ref List<GameObject> shootablesInRange) {
        var hasSorted = true;
        GameObject switchGameObject;
        for (int i = 0; hasSorted; i++) {
            hasSorted = false;
            if (shootablesInRange.Count <= 1) return;
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
