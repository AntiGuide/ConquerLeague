
using UnityEngine;

/// <summary>
/// Class for genarating FloatUp feedback elements
/// </summary>
public class FloatUpSpawner : MonoBehaviour {
    public static FloatUpSpawner FSFloatUpSpawner;

    /// <summary>Prefab for new FloatUp elements</summary>
    public GameObject FloatUpPrefab;

    /// <summary>The time that is needed for newly generated FloatUp elements to completely fade</summary>
    public float FadeTime;

    /// <summary>The distance that the newly generated FloatUp elements travel until disappearing</summary>
    public float TravelDistance;

    public SoundController soundController;

    /// <summary>Method creates a new FloatUp element of the given parameters</summary>
    /// <param name="value">The value to display in the generated FloatUp element</param>
    /// <param name="type">The type of the generated FloatUp element. For example this could decide between Powerlevel and Dollar.</param>
    /// <param name="pos">The position at which the FloatUp will spawn and begin traveling</param>
    public static void GenerateFloatUp(long value, FloatUp.ResourceType type, Vector2 pos, float objectHeight = 25f) {
        objectHeight = value > 0 ? objectHeight : -objectHeight;
        pos.y = pos.y + objectHeight;
        var go = Instantiate(FSFloatUpSpawner.FloatUpPrefab, pos, Quaternion.identity, FSFloatUpSpawner.transform);
        go.GetComponent<FloatUp>().Initialize(type, value, FSFloatUpSpawner.FadeTime, FSFloatUpSpawner.TravelDistance);
    }

    private void Start() {
        FSFloatUpSpawner = this;
    }
}