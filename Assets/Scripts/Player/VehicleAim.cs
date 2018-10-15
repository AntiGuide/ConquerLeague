using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to let the player auto-aim in a certain range
/// </summary>
public class VehicleAim : MonoBehaviour {
    /// <summary>Tags that are recognized as shootables</summary>
    [SerializeField]
    private string[] shootablesTags;

    /// <summary>The range to target shootables in</summary>
    [SerializeField]
    private float targetRange;

    /// <summary>The player GameObject. Used for orientation checks.</summary>
    [SerializeField]
    private GameObject player;

    /// <summary>The player GameObject. Used for orientation checks.</summary>
    [SerializeField]
    private float coneDegrees;

    private float coneCosLimit;

    /// <summary>List of all the GameObjects tagged with a shootable tag in range</summary>
    private List<GameObject> shootablesInRange = new List<GameObject>();

    /// <summary>List of all the GameObjects tagged with a shootable tag in range</summary>
    private List<GameObject> shootablesInConeAndRange = new List<GameObject>();

    /// <summary>The last target during last frame</summary>
    private GameObject lastTargeted;

    /// <summary>The shootable that is being aimed at</summary>
    public GameObject AktAimingAt {
        get { return shootablesInConeAndRange.Count >= 1 ? shootablesInConeAndRange[0] : null; }
    }

    /// <summary>Use this for initialization</summary>
    private void Start() {
        gameObject.GetComponent<SphereCollider>().radius = targetRange;
        coneCosLimit = -Mathf.Cos(coneDegrees/2);
    }

    /// <summary>Update is called once per frame</summary>
    private void Update() {
        OrderByMagnitude(ref shootablesInRange, gameObject.transform);
        IsInCone(shootablesInRange, ref shootablesInConeAndRange);
        if (lastTargeted == AktAimingAt) { // Situation didnt change
            return;
        }
        if (AktAimingAt == null && lastTargeted != null) { // Not targeting anything anymore
            lastTargeted.GetComponent<Renderer>().material.color = Color.red;
            lastTargeted = null;
            return;
        }
        if (AktAimingAt != null) { // Changed target
            
            if (lastTargeted != null) {
                lastTargeted.GetComponent<Renderer>().material.color = Color.red;
            }
            AktAimingAt.GetComponent<Renderer>().material.color = Color.gray;
            lastTargeted = AktAimingAt;
        }
    }

    /// <summary>
    /// Triggered if something enters an attached collider set to IsTrigger
    /// </summary>
    /// <param name="other">The collider that entered the collider</param>
    private void OnTriggerEnter(Collider other) {
        if (Array.IndexOf(shootablesTags, other.tag) > -1) {
            shootablesInRange.Add(other.gameObject);
        }
    }

    /// <summary>
    /// Triggered if something leaves an attached collider set to IsTrigger
    /// </summary>
    /// <param name="other">The collider that left the collider</param>
    private void OnTriggerExit(Collider other) {
        if (Array.IndexOf(shootablesTags, other.tag) > -1) {
            other.gameObject.GetComponent<Renderer>().material.color = Color.red;
            shootablesInRange.Remove(other.gameObject);
        }
    }

    /// <summary>
    /// Orders given transforms by magnitude to another transform
    /// </summary>
    /// <param name="transforms">Transforms to order</param>
    /// <param name="referenceTransform">Transform for magnitude check</param>
    void OrderByMagnitude(ref List<GameObject> gameObjects, Transform referenceTransform) {
        gameObjects.Sort(delegate(GameObject a, GameObject b) {
            return Vector3.SqrMagnitude(referenceTransform.position - a.transform.position).CompareTo(Vector3.SqrMagnitude(referenceTransform.position - b.transform.position));
        });
    }

    void IsInCone(List<GameObject> gameObjects, ref List<GameObject> filteredGameObjects) {
        Debug.DrawLine(player.transform.position, player.transform.position + (Quaternion.AngleAxis(coneDegrees / 2, player.transform.up) * player.transform.forward * targetRange));
        Debug.DrawLine(player.transform.position, player.transform.position + (Quaternion.AngleAxis(-coneDegrees / 2, player.transform.up) * player.transform.forward * targetRange));
        filteredGameObjects.Clear();
        for (int i = 0; i < gameObjects.Count; i++) {
            var dotResult = Vector3.Dot(player.transform.forward, Vector3.Normalize(gameObjects[i].transform.position - player.transform.position));
            if (dotResult > coneCosLimit) {
                Debug.Log(gameObjects[i].name + ": " + dotResult);
                filteredGameObjects.Add(gameObjects[i]);
            }
        }
        
    }
}
