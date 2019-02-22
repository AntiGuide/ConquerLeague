using System.Collections;
using UnityEngine;

/// <summary>
/// A simple implementation for a camera shake
/// </summary>
public class CameraShake : MonoBehaviour {
    /// <summary>The static reference to the single instance of this class</summary>
    public static CameraShake Instance;

    /// <summary>Determines if the camera is shaking</summary>
    private bool shaking = false;

    /// <summary>The magnitude of the camera shake</summary>
    private float magnitude;

    /// <summary>The original position of the camera</summary>
    private Vector3 originalPos;

    /// <summary>
    /// Makes the camera shake
    /// </summary>
    /// <param name="duration">The duration of the shake</param>
    /// <param name="magnitude">The magnitude of the camera shake</param>
    public IEnumerator Shake(float duration = 0.5f, float magnitude = 0.5f) {
        originalPos = Instance.transform.localPosition;
        this.magnitude = magnitude;
        shaking = true;
        yield return new WaitForSeconds(duration);
        shaking = false;
        Instance.transform.localPosition = originalPos;
    }

    /// <summary>
    /// Start is called on object initialization by Unity
    /// </summary>
    private void Start() {
        if (Instance != null) {
            Application.Quit();
        }

        Instance = this;
    }

    /// <summary>
    /// Update is called once per frame by Unity
    /// </summary>
    private void Update() {
        if (!shaking) {
            return;
        }

        var x = Random.Range(-1f, 1f) * magnitude;
        var y = Random.Range(-1, 1f) * magnitude;
        Instance.transform.localPosition = originalPos + new Vector3(x, y);
    }
}
