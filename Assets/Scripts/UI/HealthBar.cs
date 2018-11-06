using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    [SerializeField]
    private Vector3 offset;

    [HideInInspector]
    public HitPoints hitPoints;

    [SerializeField]
    private Camera camera;

    [SerializeField]
    public GameObject target;

    [SerializeField]
    private Image fullHp;

    private Vector3 screenPos;

    private float maxHp;

	// Use this for initialization
	void Start () {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        hitPoints = target.GetComponent<HitPoints>();
        maxHp = hitPoints.AktHp;
	}
	
	// Update is called once per frame
	void Update () {
        if (target == null)
        {
            Destroy(gameObject);
        } else {
            screenPos = camera.WorldToScreenPoint(target.transform.position);
            transform.position = screenPos + offset;

            fullHp.fillAmount = (hitPoints.AktHp / maxHp);
        }
    }
}
