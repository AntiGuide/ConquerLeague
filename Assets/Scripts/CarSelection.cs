using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelection : MonoBehaviour {
    [SerializeField]
    private GameObject[] cars = new GameObject[3];

    [SerializeField]
    private bool upArrow;

    private static int currRendering = 0;

    private MeshRenderer[][] carRenderers = new MeshRenderer[3][];

	// Use this for initialization
	void Start () {
		for(int i = 0; i < cars.Length; i++) {
            carRenderers[i] = cars[i].GetComponentsInChildren<MeshRenderer>();
        }

        for (int i = 0; i < carRenderers[0].Length; i++) {
                carRenderers[currRendering][i].enabled = true;
        }
	}
	
    public void SwapCarRenderer() {
        SetRenderer(false, currRendering);

        currRendering += upArrow ? 1 : -1;
        currRendering = currRendering < 0 ? 2 : currRendering;
        currRendering = currRendering > 2 ? 0 : currRendering;

        SetRenderer(true, currRendering);
    }

    void SetRenderer(bool enable, int currRendering) {
        for (int i = 0; i < carRenderers[currRendering].Length; i++) {
            carRenderers[currRendering][i].enabled = enable;
        }
    }
}