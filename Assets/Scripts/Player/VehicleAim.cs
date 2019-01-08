using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to let the player auto-aim in a certain range
/// </summary>
public class VehicleAim : MonoBehaviour, IConfigurable {
    /// <summary>The range to target shootables in</summary>
    [SerializeField, Header("Customizable"), Tooltip("The Range in UnityUnits/meters that a player is allowed to target")]
    private float targetRange;

    /// <summary>The "shooting FOV". For example 180° would cover the whole front.</summary>
    [SerializeField, Range(0, 360), Tooltip("The \"shooting FOV\". For example 180° would cover the whole front.")]
    private float coneDegrees;

    /// <summary>Tags that are recognized as shootables. Priority top to bottom.</summary>
    [SerializeField, Tooltip("Tags that are recognized as shootables. Priority top to bottom.")]
    private string[] shootablesTags;

    /// <summary>The player GameObject. Used for orientation checks.</summary>
    [SerializeField, Header("References")]
    private GameObject player;

    private TeamHandler teamHandler;

    /// <summary>Converted cone limits</summary>
    private float coneCosLimit;

    /// <summary>List of all the GameObjects tagged with a shootable tag in range</summary>
    private List<GameObject> shootablesInRange = new List<GameObject>();

    /// <summary>List of all the GameObjects tagged with a shootable tag in range</summary>
    private List<GameObject> shootablesInConeAndRange = new List<GameObject>();

    private List<GameObject> highestPriority = new List<GameObject>();

    /// <summary>The last target during last frame</summary>
    private GameObject lastTargeted;

    /// <summary>The shootable that is being aimed at</summary>
    public GameObject AktAimingAt {
        get { return highestPriority.Count >= 1 ? highestPriority[0] : null; }
    }

    /// <summary>
    /// Orders given transforms by magnitude to another transform
    /// </summary>
    /// <param name="transforms">Transforms to order</param>
    /// <param name="referenceTransform">Transform for magnitude check</param>
    public static void OrderByMagnitude(ref List<GameObject> gameObjects, Transform referenceTransform) {
        // Checks if gameobjects in the gameobjects-List are destroyed and removes them
        for (int i = 0; i < gameObjects.Count; i++) {
            if (gameObjects[i] == null || gameObjects[i].Equals(null)) {
                gameObjects.RemoveAt(i);
            }
        }

        gameObjects.Sort(delegate(GameObject a, GameObject b) {
            return Vector3.SqrMagnitude(referenceTransform.position - a.transform.position).CompareTo(Vector3.SqrMagnitude(referenceTransform.position - b.transform.position));
        });
    }

    public static void OrderByCentral(ref List<GameObject> gameObjects, Transform referenceTransform) {
        gameObjects.Sort(delegate (GameObject a, GameObject b) {
            return (-1 * (Vector3.Dot(referenceTransform.forward, Vector3.Normalize(a.transform.position - referenceTransform.position)).CompareTo(Vector3.Dot(referenceTransform.forward, Vector3.Normalize(b.transform.position - referenceTransform.position)))));
        });
    }

    /// <summary>Use this for initialization</summary>
    private void Start() {
        teamHandler = GetComponent<VehicleWeapon>().TeamHandler;
        gameObject.GetComponent<SphereCollider>().radius = targetRange;
        coneCosLimit = Mathf.Cos(coneDegrees / 2);
    }

    /// <summary>Update is called once  per frame</summary>
    private void Update() {
        //OrderByMagnitude(ref shootablesInRange, gameObject.transform);
        IsInCone(shootablesInRange, ref shootablesInConeAndRange);
        highestPriority = FilterByPriority(shootablesInConeAndRange, shootablesTags);
        
        if (highestPriority?.Count > 1) {
            OrderByCentral(ref highestPriority, player.transform); // NOT TESTED
        }

        if (lastTargeted == AktAimingAt) { // Situation didnt change
            return;
        }

        if (AktAimingAt == null && lastTargeted != null) { // Not targeting anything anymore
            lastTargeted.GetComponentInChildren<ParticleSystem>()?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            lastTargeted = null;
            return;
        }

        if (AktAimingAt != null) { // Changed target
            if(lastTargeted != null) {
                lastTargeted.GetComponentInChildren<ParticleSystem>()?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            AktAimingAt.GetComponentInChildren<ParticleSystem>()?.Play();
            lastTargeted = AktAimingAt;
        }
    }

    /// <summary>
    /// Triggered if something enters an attached collider set to IsTrigger
    /// </summary>
    /// <param name="other">The collider that entered the collider</param>
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<TeamHandler>()?.TeamID != teamHandler.TeamID &&
            other.gameObject.GetComponent<TeamHandler>()?.TeamID != TeamHandler.TeamState.NEUTRAL) {
            if (Array.IndexOf(shootablesTags, other.tag) > -1) {
                shootablesInRange.Add(other.gameObject);
            }
        }
    }

    /// <summary>
    /// Triggered if something leaves an attached collider set to IsTrigger
    /// </summary>
    /// <param name="other">The collider that left the collider</param>
    private void OnTriggerExit(Collider other) {
        if (Array.IndexOf(shootablesTags, other.tag) > -1) {
            shootablesInRange.Remove(other.gameObject);
        }
    }

    /// <summary>
    /// Filters out all GameObjects that are not in the cone
    /// </summary>
    /// <param name="gameObjects">The List to filter</param>
    /// <param name="filteredGameObjects">The result of the filtration</param>
    private void IsInCone(List<GameObject> gameObjects, ref List<GameObject> filteredGameObjects) {
        Debug.DrawLine(player.transform.position, player.transform.position + (Quaternion.AngleAxis(coneDegrees / 2, player.transform.up) * player.transform.forward * targetRange));
        Debug.DrawLine(player.transform.position, player.transform.position + (Quaternion.AngleAxis(-coneDegrees / 2, player.transform.up) * player.transform.forward * targetRange));
        filteredGameObjects.Clear();
        for (int i = 0; i < gameObjects.Count; i++) {
            var dotResult = Vector3.Dot(player.transform.forward, Vector3.Normalize(gameObjects[i].transform.position - player.transform.position));
            if (dotResult > coneCosLimit) {
                filteredGameObjects.Add(gameObjects[i]);
            }
        }
    }

    /// <summary>
    /// Orders GameObjects in referenced array shootablesInConeAndRange by priority obtained trough tags from tagsByPriority
    /// </summary>
    /// <param name="shootablesInConeAndRange">These GameObjects are ordered</param>
    /// <param name="tagsByPriority">GameObjects are orderd by this list</param>
    private void OrderByPriority(ref List<GameObject> shootablesInConeAndRange, string[] tagsByPriority) {
        var hasSorted = true;
        GameObject switchGameObject;
        for (int i = 0; hasSorted; i++) {
            hasSorted = false;
            if (shootablesInConeAndRange.Count <= 1) {
                return;
            }

            for (int i2 = 1; i2 < shootablesInConeAndRange.Count; i2++) {
                var intComp1 = Array.IndexOf(tagsByPriority, shootablesInConeAndRange[i2 - 1].tag);
                var intComp2 = Array.IndexOf(tagsByPriority, shootablesInConeAndRange[i2].tag);
                if (intComp1 > intComp2) {
                    switchGameObject = shootablesInConeAndRange[i2 - 1];
                    shootablesInConeAndRange[i2 - 1] = shootablesInConeAndRange[i2];
                    shootablesInConeAndRange[i2] = switchGameObject;
                    hasSorted = true;
                }
            }
        }
    }

    private List<GameObject> FilterByPriority(List<GameObject> shootablesInConeAndRange, string[] tagsByPriority) {
        if (shootablesInConeAndRange.Count <= 1) {
            return shootablesInConeAndRange;
        }

        var maxPriority = Array.IndexOf(tagsByPriority, shootablesInConeAndRange[0].tag);

        // Look for highest priority in List
        for (int i = 1; i < shootablesInConeAndRange.Count; i++) {
            var tmp = Array.IndexOf(tagsByPriority, shootablesInConeAndRange[i].tag);
            maxPriority = maxPriority > tmp ? tmp : maxPriority;
        }

        var newList = new List<GameObject>(shootablesInConeAndRange.Count);
        // Filtereverything that is not highest priority in List
        for (int i = 0; i < shootablesInConeAndRange.Count; i++) {
            if (Array.IndexOf(tagsByPriority, shootablesInConeAndRange[i].tag) == maxPriority) {
                newList.Add(shootablesInConeAndRange[i]);
            }
        }

        return newList;
    }

    public void UpdateConfig() {
        targetRange = ConfigButton.VehicleMGRange;
        var col = gameObject.GetComponent<SphereCollider>();
        col.enabled = false;
        col.radius = targetRange;
        col.enabled = true;
    }
}
