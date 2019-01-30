using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTracker : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private Transform ownTrans;
    [SerializeField] private float offset = 0.1f;
    public Vector3 debugPos;

    private RectTransform recTransImage;

    // Use this for initialization
    void Start () {
        recTransImage = GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {
        var screenPos = Camera.main.WorldToViewportPoint(target.position); //get viewport positions
        screenPos = screenPos.z < 0 ? -screenPos : screenPos;
        if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1) {
            GetComponent<Image>().enabled = false;
            return;
        }

        GetComponent<Image>().enabled = true;
        var vecToEnemy = Camera.main.WorldToScreenPoint(target.position);
        vecToEnemy = vecToEnemy.z < 0 ? -vecToEnemy : vecToEnemy;
        var vecOwnToEnemy = vecToEnemy - Camera.main.WorldToScreenPoint(ownTrans.position);
        var newZ = Vector3.SignedAngle(vecOwnToEnemy, Vector3.up, Vector3.back);
        recTransImage.localEulerAngles = new Vector3(0, 0, newZ);
        var onScreenPos = new Vector2(screenPos.x - 0.5f, screenPos.y - 0.5f) * 2; //2D version, new mapping
        var max = Mathf.Max(Mathf.Abs(onScreenPos.x), Mathf.Abs(onScreenPos.y)); //get largest offset
        onScreenPos = ((onScreenPos / (max * 2)) * (1f-offset)) + new Vector2(0.5f, 0.5f); //undo mapping
        recTransImage.position = Camera.main.ViewportToScreenPoint(onScreenPos);
    }
}
