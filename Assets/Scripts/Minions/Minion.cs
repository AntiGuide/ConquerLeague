using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour {

    /// <summary>The minions movement order</summary>
    [SerializeField, Tooltip("The transform-targets, which tells the minions where to go and in which order. Priority from top to bottom.")]
    private Transform[] movementOrder;
    
    /// <summary>Defines how fast the minion moves</summary>
    [SerializeField, Range(0.1f, 10f)]
    private float speed = 0.1f;

    /// <summary>The progress </summary>
    private float progress = 0;

    private Vector3 startPosition;

    private float distanceBetweenPoints;

    // Use this for initialization
    void Start () {
        startPosition = transform.position;
        distanceBetweenPoints = Vector3.Distance(startPosition, movementOrder[0].position);
    }

    // Update is called once per frame
    void Update () {
        transform.LookAt(movementOrder[0]);
        
        
        progress += (Time.deltaTime * speed) / distanceBetweenPoints;
        transform.position = Vector3.Lerp(startPosition, movementOrder[0].position, progress);
	}

    void SwitchTarget() {

    }
}