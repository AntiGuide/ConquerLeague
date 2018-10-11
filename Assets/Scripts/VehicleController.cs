using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class VehicleController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private Rigidbody rb;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        Movement();
    }

    void Movement() {
        Vector2 rotation = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));

        if (rotation == Vector2.zero) return;
        
        var rotate = Vector2.Angle(Vector2.up, rotation);
        var sRotate = rotate;

        if(CrossPlatformInputManager.GetAxis("Horizontal") <= 0) {
            rotate = -rotate;
        } else {
            rotate = sRotate;
        }

        this.transform.localEulerAngles = new Vector3(0, rotate, 0);
        this.rb.velocity = this.transform.forward * rotation.magnitude;
    }
}
