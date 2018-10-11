using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour {

    [SerializeField]
    private float destroyTime;
	
	// Update is called once per frame
	void Update () {
        destroyTime -= Time.deltaTime;
        if (destroyTime <= 0f) {
            Destroy(gameObject);
        }
    }
}
