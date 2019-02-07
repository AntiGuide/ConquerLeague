using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class HealthBar : MonoBehaviour {
    /// <summary>References the hitpoints script</summary>
    private HitPoints hitPoints;

    /// <summary>The Healtbars target</summary>
    [SerializeField]
    public GameObject Target;

    /// <summary>The Healthbars UI-Offset</summary>
    [SerializeField] private Vector3 offset;

    /// <summary>The fullHp image</summary>
    [SerializeField]
    private Image fullHp;

    /// <summary>The hpBackground image</summary>
    [SerializeField]
    private Image hpBackground;

    /// <summary>The hpBorder image</summary>
    [SerializeField]
    private Image hpBorder;

    /// <summary>The targets position in screenspace</summary>
    private Vector3 screenPos;

    private byte maxHp;

    private bool active = true;

    public bool Active {
        set {
            foreach (var item in Images) {
                item.enabled = value;
            }

            active = value;
        }

        get {
            return active;
        }
    }

    public List<Image> Images { get; } = new List<Image>();

    public Vector3 Offset {
        get {
            return offset;
        }

        set {
            offset = value;
        }
    }

    public byte MaxHp { get; set; }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        hitPoints = Target.GetComponent<HitPoints>();
        maxHp = hitPoints.AktHp;
        Images.Add(fullHp);
        Images.Add(hpBackground);
        Images.Add(hpBorder);
    }

    /// <summary>LateUpdate is called once per frame after update</summary>
    void LateUpdate() {
        if (Target == null) {
            Destroy(gameObject);
            return;
        }

        screenPos = Camera.main.WorldToScreenPoint(Target.transform.position + offset);
        transform.position = screenPos;
        if (Active && screenPos.z < 0) {
            Active = false;
        } else if (!Active && screenPos.z >= 0 && hitPoints.AktHp > 0) {
            Active = true;
        }
    }

    public void UpdateBar() {
        if (hitPoints == null) {
            fullHp.fillAmount = 1f;
        } else {
            fullHp.fillAmount = hitPoints.AktHp / (float)maxHp;
        }

    }
}
