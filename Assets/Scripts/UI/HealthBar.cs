using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class HealthBar : MonoBehaviour {
    /// <summary>References the hitpoints script</summary>
    [HideInInspector]
    public HitPoints HitPoints;

    /// <summary>The Healtbars target</summary>
    [SerializeField]
    public GameObject Target;

    /// <summary>The Healthbars UI-Offset</summary>
    [SerializeField]
    private Vector2 offset;

    /// <summary>The fullHp image</summary>
    [SerializeField]
    private Image fullHp;

    /// <summary>The targets position in screenspace</summary>
    private Vector2 screenPos;

    private byte maxHp;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        HitPoints = Target.GetComponent<HitPoints>();
        maxHp = HitPoints.AktHp;
    }

    /// <summary>LateUpdate is called once per frame after update</summary>
    void LateUpdate() {
        if (Target == null) {
            Destroy(gameObject);
            return;
        }

        screenPos = (Vector2)Camera.main.WorldToScreenPoint(Target.transform.position);
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
