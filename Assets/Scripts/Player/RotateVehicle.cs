using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateVehicle : MonoBehaviour {

    [SerializeField]
    private float rotationSpeed;

    private float rotation;

	// Use this for initialization
	void Start () {
        this.transform.eulerAngles = new Vector3(0, 180, 0);
	}
	
	// Update is called once per frame
	void Update () {
        rotation += Time.deltaTime * rotationSpeed;
        this.transform.eulerAngles = new Vector3(0, rotation, 0);
	}
}
