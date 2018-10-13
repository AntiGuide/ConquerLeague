using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// The vehicles controller, which defines how fast its moving and rotating
/// </summary>
public class VehicleController : MonoBehaviour
{
    /// <summary>Defines how fast the vehicle moves</summary>
    [SerializeField]
    private float movementSpeed;

    /// <summary>The vehicles rigidbody</summary>
    [SerializeField]
    private Rigidbody rb;

    /// <summary>
    /// Use this for initialization
    /// </summary>    
    void Start() {
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>    
    void Update() {
        Movement();
    }

    /// <summary>
    /// Uses CrossplatformInput to move and rotate player vehicle
    /// </summary>
    void Movement() {
        Vector2 rotation = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
        if (rotation == Vector2.zero) {
            return;
        }

        var rotate = Vector2.Angle(Vector2.up, rotation);
        var saveRotate = rotate;
        if (CrossPlatformInputManager.GetAxis("Horizontal") <= 0) {
            rotate = -rotate;
        } else {
            rotate = saveRotate;
        }

        this.transform.localEulerAngles = new Vector3(0, rotate, 0);
        this.rb.velocity = this.transform.forward * rotation.magnitude * movementSpeed;
    }
}
