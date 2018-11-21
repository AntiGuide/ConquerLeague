using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    [SerializeField]
    private Vector2 offset;

    [HideInInspector]
    public HitPoints hitPoints;

    [SerializeField]
    public GameObject target;

    [SerializeField]
    private Image fullHp;

    private Vector2 screenPos;

    private byte maxHp;

    // Use this for initialization
    void Start() {
        hitPoints = target.GetComponent<HitPoints>();
        maxHp = hitPoints.AktHp;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (target == null) {
            Destroy(gameObject);
            return;
        }

        screenPos = (Vector2)Camera.main.WorldToScreenPoint(target.transform.position);
        //transform.position = camera.WorldToScreenPoint(target.transform.position);
        transform.position = screenPos + offset;
    }

    public void UpdateBar() {
        if (hitPoints == null) {
            fullHp.fillAmount = 1f;
        } else {
            fullHp.fillAmount = hitPoints.AktHp / (float)maxHp;
        }

    }
}
