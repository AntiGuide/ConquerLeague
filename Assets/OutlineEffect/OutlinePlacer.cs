using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OutlinePlacer : MonoBehaviour {

    private void OnEnable() {
        var arr = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < arr.Length; i++) {
            arr[i].gameObject.AddComponent<Outline>();
        }
    }

    void OnDisable() {
        var arr = GetComponentsInChildren<Outline>();
        for (int i = 0; i < arr.Length; i++) {
            DestroyImmediate(arr[i]);
        }
    }
}
