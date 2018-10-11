using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to destroy the attached Object
/// </summary>
public class DestroyAfter : MonoBehaviour {
    /// <summary>The time after which the object is destroyed</summary>
    [SerializeField]
    private float destroyTime;

    /// <summary>Update is called once per frame</summary>
    void Update() {
        destroyTime -= Time.deltaTime;
        if (destroyTime <= 0f) {
            Destroy(gameObject);
        }
    }
}
