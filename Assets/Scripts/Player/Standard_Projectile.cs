using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The players standard-shot projectile
/// </summary>
public class Standard_Projectile : MonoBehaviour {
    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        Destroy(this.gameObject, 2f);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
    }
}