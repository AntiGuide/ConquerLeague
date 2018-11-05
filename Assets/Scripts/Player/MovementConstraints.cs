using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class limits the rotation of a given object
/// </summary>
public class MovementConstraints : MonoBehaviour {
    /// <summary>The angle the object may rotate around the x axis</summary>
    [SerializeField] private Vector2 xConstraint;

    /// <summary>The angle the object may rotate around the z axis</summary>
    [SerializeField] private Vector2 zConstraint;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        var newAngles = transform.rotation.eulerAngles;
        newAngles.x = newAngles.x < 180f ? Mathf.Min(newAngles.x, xConstraint.y) : Mathf.Max(newAngles.x, xConstraint.x + 360f);
        newAngles.z = newAngles.z < 180f ? Mathf.Min(newAngles.z, zConstraint.y) : Mathf.Max(newAngles.z, zConstraint.x + 360f);
        transform.rotation = Quaternion.Euler(newAngles);
    }
}