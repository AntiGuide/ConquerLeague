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


    /// <summary>The progress of the minions current journey</summary>
    private float progress = 0;

    /// <summary>The minions start position</summary>
    private Vector3 startPosition;

    /// <summary>The distance between points</summary>
    private float distanceBetweenPoints;

    /// <summary>The array index of the currently chased target</summary>
    private int currTarget = 0;

    // Use this for initialization
    void Start () {
        startPosition = transform.position;
        distanceBetweenPoints = Vector3.Distance(startPosition, movementOrder[0].position);
    }

    // Update is called once per frame
    void Update () {
        transform.LookAt(movementOrder[currTarget]);
        progress += (Time.deltaTime * speed) / distanceBetweenPoints;

        transform.position = Vector3.Lerp(startPosition, movementOrder[currTarget].position, progress);

        if(progress >= 1f) {
            startPosition = transform.position;
            currTarget++;

            if(movementOrder.Length == currTarget) {
                Destroy(gameObject);
                return;
            }

            progress -= 1f;
            progress *= distanceBetweenPoints;
            distanceBetweenPoints = Vector3.Distance(startPosition, movementOrder[currTarget].position);
            progress /= distanceBetweenPoints;
        }
    }
}