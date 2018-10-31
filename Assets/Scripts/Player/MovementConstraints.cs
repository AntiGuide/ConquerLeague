using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementConstraints : MonoBehaviour {
    [SerializeField]
    private Vector2 xConstraint;
    [SerializeField]
    private Vector2 zConstraint;
	
	// Update is called once per frame
	void Update () {
        var newAngles = transform.rotation.eulerAngles;
	    newAngles.x = newAngles.x < 180f ? Mathf.Min(newAngles.x, xConstraint.y) : Mathf.Max(newAngles.x, xConstraint.x + 360f);
	    newAngles.z = newAngles.z < 180f ? Mathf.Min(newAngles.z, zConstraint.y) : Mathf.Max(newAngles.z, zConstraint.x + 360f);
	    transform.rotation = Quaternion.Euler(newAngles);

	}
}
