using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleWheelControll : MonoBehaviour {
    [SerializeField]
    private bool isEnemy;

    public static float WheelTurnSpeedFriend { get; set; }

    public static float WheelTurnSpeedEnemy { get; set; }

    /// <summary>Update is called once per frame</summary>
    void Update() {
        if (isEnemy) {
            transform.Rotate(new Vector3(WheelTurnSpeedEnemy, 0f, 0f));
        } else {
            transform.Rotate(new Vector3(WheelTurnSpeedFriend, 0f, 0f));
        }
    }
}
