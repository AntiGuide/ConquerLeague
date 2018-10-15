using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to let the player auto-aim in a certain range
/// </summary>
public class VehicleAim : MonoBehaviour {
    /// <summary>Tags that are recognized as enemys</summary>
    [SerializeField]
    private string[] enemyTags;

    /// <summary>The range to target enemys in</summary>
    [SerializeField]
    private float targetRange;

    /// <summary>List of all the GameObjects tagged 'Turret' in range</summary>
    private List<GameObject> turretsInRange = new List<GameObject>();

    /// <summary>The tower that was targeted last frame</summary>
    private GameObject lastTargetedTower;

    /// <summary>Use this for initialization</summary>
    private void Start() {
        gameObject.GetComponent<SphereCollider>().radius = targetRange;
    }

    /// <summary>Update is called once per frame</summary>
    private void Update() {
        OrderByMagnitude(ref turretsInRange, gameObject.transform);
        if (turretsInRange.Count <= 0 || lastTargetedTower == turretsInRange[0]) {
            return;
        }

        if (lastTargetedTower != null) {
            lastTargetedTower.GetComponent<Renderer>().material.color = Color.red;
        }

        turretsInRange[0].GetComponent<Renderer>().material.color = Color.gray;
        lastTargetedTower = turretsInRange[0];
    }

    /// <summary>
    /// Triggered if something enters an attached collider set to IsTrigger
    /// </summary>
    /// <param name="other">The collider that entered the collider</param>
    private void OnTriggerEnter(Collider other) {
        if (Array.IndexOf(enemyTags, other.tag) > -1) {
            turretsInRange.Add(other.gameObject);
        }
    }

    /// <summary>
    /// Triggered if something leaves an attached collider set to IsTrigger
    /// </summary>
    /// <param name="other">The collider that left the collider</param>
    private void OnTriggerExit(Collider other) {
        if (Array.IndexOf(enemyTags, other.tag) > -1) {
            other.gameObject.GetComponent<Renderer>().material.color = Color.red;
            turretsInRange.Remove(other.gameObject);
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
}
