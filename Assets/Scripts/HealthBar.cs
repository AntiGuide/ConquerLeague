using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    [SerializeField]
    private HitPoints hitPoints;

    [SerializeField]
    private GameObject camera;

    [SerializeField]
    private bool fullBar;

    private Vector2 size;

	// Use this for initialization
	void Start () {
        camera = GameObject.Find("Main Camera");
        size = GetComponent<SpriteRenderer>().size;
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(camera.transform);

        if (fullBar)
        {
            GetComponent<SpriteRenderer>().size = new Vector2(hitPoints.AktHp, size.y);
        }
	}
}
