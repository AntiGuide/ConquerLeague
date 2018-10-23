using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This units HitPoints, gets destroyed when its reduced to zero
/// </summary>
public class HitPoints : MonoBehaviour {
    /// <summary>The units current hitpoints</summary>
    public int hp;


    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if(hp <= 0) {
            Destroy(gameObject);
        }
    }
}