using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public static CameraShake FSCameraShake;

    private void Start() {
        FSCameraShake = this;
    }

    public static IEnumerator Shake (float duration, float magnitude)
    {
        Vector3 originalPos = FSCameraShake.transform.localPosition;

        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1, 1f) * magnitude;

            FSCameraShake.transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        
        FSCameraShake.transform.localPosition = originalPos;
    }
}
