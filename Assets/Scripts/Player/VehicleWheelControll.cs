using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleWheelControll : MonoBehaviour {
    [SerializeField]
    private bool isEnemy;

    [SerializeField]
    private bool isBackWheel;

    private static float wheelTurnSpeedFriend;
                  
    private static float wheelTurnSpeedEnemy;

    public static float wheelTurnFriend;

    public static float wheelTurnEnemy;

    public bool WheelHasTraction { get; set; }

    private Quaternion q = new Quaternion();

    /// <summary>Update is called once per frame</summary>
    void Update() {
        var wts = new Vector3();
        var wt = new Vector3();
        wts.x = isEnemy ? wheelTurnSpeedEnemy : wheelTurnSpeedFriend;
        wt.y = isEnemy ? wheelTurnEnemy : wheelTurnFriend;
        q.eulerAngles = wt;
        transform.localRotation = isBackWheel ? transform.localRotation : Quaternion.RotateTowards(transform.localRotation, q, 45);
        transform.Rotate(wts);
    }

    private void FixedUpdate() {
        WheelHasTraction = Physics.Raycast(transform.position, Vector3.down, 0.7f, LayerMask.GetMask("Ground"));
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
