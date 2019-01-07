﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitPoints))]
public class TowerNet : MonoBehaviour {
    [SerializeField] private byte id;

    /// <summary>The reference to the attached hitpoint script</summary>
    private HitPoints hitPoints;

    public byte ID { get { return id; } set { id = value; } }

    /// <summary>
    /// Triggered if damage is taken. Handles health reduction.
    /// </summary>
    /// <param name="damage"></param>
    public void DamageTaken(byte damage) {
        if (hitPoints.AktHp - damage <= hitPoints.AktHp && hitPoints.AktHp - damage > 0) {
            hitPoints.AktHp -= damage;
        } else {
            hitPoints.AktHp = 0;
        }
    }

    /// <summary>Use this for initialization</summary>
    void Start() {
        hitPoints = GetComponent<HitPoints>();
        GameManager.towers[id] = this;
    }

    /// <summary>Update is called once per frame</summary>
    void Update() {
        
    }
}