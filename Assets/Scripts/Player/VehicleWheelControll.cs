using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleWheelControll : MonoBehaviour {
    [SerializeField]
    private bool isEnemy;

    private static float wheelTurnSpeedFriend;
                  
    private static float wheelTurnSpeedEnemy;

    public static float wheelTurnFriend;

    public static float wheelTurnEnemy;

    public bool WheelHasTraction { get; set; }

    /// <summary>Update is called once per frame</summary>
    void Update() {
        if (isEnemy) {
            transform.Rotate(new Vector3(wheelTurnSpeedEnemy, 0f, 0f));
        } else {
            transform.Rotate(new Vector3(wheelTurnSpeedFriend, 0f, 0f));
        }
    }

    private void FixedUpdate() {
        var r = new Ray(transform.position, Vector3.down);
        //var hit = new RaycastHit();
        var grounded = Physics.Raycast(r, 0.7f, LayerMask.GetMask("Ground"));
        WheelHasTraction = grounded;
        //if (!grounded) {
        //    Debug.Log(gameObject.name + " is in air!");
        //}
        
    }

    public static void UpdateWheelsSpin(Rigidbody rb, bool isEnemy) {
        if (isEnemy) {
            wheelTurnSpeedEnemy = rb.velocity.magnitude;
        } else {
            wheelTurnSpeedFriend = rb.velocity.magnitude;
        }
    }

    public static void UpdateWheelsTurn(float turnFactor, bool isEnemy) {
        if (isEnemy) {
            wheelTurnEnemy = turnFactor;
        } else {
            wheelTurnFriend = turnFactor;
        }
    }
}
