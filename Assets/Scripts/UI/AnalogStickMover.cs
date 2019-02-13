using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Image), typeof(RectTransform))]
public class AnalogStickMover : MonoBehaviour {

    private RectTransform trans;
    private Vector2 startPos;

    // Use this for initialization
    void Start () {
        trans = GetComponent<RectTransform>();
        startPos = trans.anchoredPosition;
    }
	
	// Update is called once per frame
	void Update () {
        var v = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
        v = Vector2.ClampMagnitude(100 * v, 100f);
        trans.anchoredPosition = startPos + v;
    }
}
