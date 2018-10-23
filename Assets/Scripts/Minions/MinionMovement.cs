﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the minions curent target and movement
/// </summary>
public class MinionMovement : MonoBehaviour {
    /// <summary>The minions movement order</summary>
    [SerializeField, Tooltip("The transform-targets, which tells the minions where to go and in which order. Priority from top to bottom.")]
    private Transform[] movementOrder;

    /// <summary>Defines how fast the minion moves</summary>
    [SerializeField, Range(0.1f, 10f)]
    private float speed = 0.1f;

    /// <summary>Defines how fast the minion will turn</summary>
    [SerializeField]
    private float turnSpeed = 2;

    /// <summary>The progress of the minions current journey</summary>
    private float progress = 0;

    /// <summary>The minions start position</summary>
    private Vector3 startPosition;

    /// <summary>The distance between points</summary>
    private float distanceBetweenPoints;

    /// <summary>The array index of the currently chased target</summary>
    private int currTarget = 0;

    /// <summary>The progress of the minions turn</summary>
    private float turnProgress = 0;

    /// <summary>Tells if the minion is turning at the moment</summary>
    private bool turning = false;

    /// <summary>Saves the minions current rotation</summary>
    private Quaternion currRotation;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        startPosition = transform.position;
        distanceBetweenPoints = Vector3.Distance(startPosition, movementOrder[0].position);
        transform.rotation = Quaternion.LookRotation(movementOrder[0].position - transform.position);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        progress += (Time.deltaTime * speed) / distanceBetweenPoints;
        transform.position = Vector3.Lerp(startPosition, movementOrder[currTarget].position, progress);

        // Change minion target + calculates distance between minion and the new target + starts turning
        if (progress >= 0.9f && progress < 1f) {
            turning = true;
        } else if (progress >= 1f) {
            currTarget++;
            if (movementOrder.Length == currTarget) {
                Destroy(gameObject);
                return;
            }

            startPosition = transform.position;
            progress -= 1f;
            progress *= distanceBetweenPoints;
            distanceBetweenPoints = Vector3.Distance(startPosition, movementOrder[currTarget].position);
            progress /= distanceBetweenPoints;
        }

        // Make the minion face its next target
        if (turning) {
            turnProgress += Time.deltaTime * turnSpeed;
            currRotation = transform.rotation;
            var currDirection = movementOrder[currTarget].position - transform.position;
            var newRota = Quaternion.LookRotation(currDirection);
            transform.rotation = Quaternion.Lerp(currRotation, newRota, turnProgress);
            if (turnProgress >= 1f) {
                turnProgress = 0f;
                turning = false;
            }
        }
    }
}