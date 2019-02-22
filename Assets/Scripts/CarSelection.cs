using UnityEngine;

/// <summary>
/// A class for selecting cars in the main menu
/// </summary>
public class CarSelection : MonoBehaviour {
    /// <summary>The current rendered car</summary>
    private static int currRendering = 0;

    /// <summary>All available car GameObjects</summary>
    [SerializeField] private GameObject[] cars = new GameObject[3];

    /// <summary>Marks if this arrow is the arrow pointing upwards</summary>
    [SerializeField] private bool isUpArrow;

    /// <summary>All mesh renderers of all cars</summary>
    private MeshRenderer[][] carRenderers = new MeshRenderer[3][];

    /// <summary>
    /// Called when arrow is clicked
    /// </summary>
    public void SwapCarRenderer() {
        SetRenderer(false, currRendering);
        currRendering += isUpArrow ? 1 : -1;
        currRendering = currRendering < 0 ? 2 : currRendering;
        currRendering = currRendering > 2 ? 0 : currRendering;
        SetRenderer(true, currRendering);
    }

    /// <summary>
    /// Start is called on object initialization by Unity
    /// </summary>
    private void Start() {
        for (int i = 0; i < cars.Length; i++) {
            carRenderers[i] = cars[i].GetComponentsInChildren<MeshRenderer>();
        }

        for (int i = 0; i < carRenderers[0].Length; i++) {
            carRenderers[currRendering][i].enabled = true;
        }
    }

    /// <summary>
    /// Set the renderers of a car (in-)active
    /// </summary>
    /// <param name="enable">The new status of the given cars renderers</param>
    /// <param name="currRendering">The index of the car to be rendered</param>
    private void SetRenderer(bool enable, int currRendering) {
        for (int i = 0; i < carRenderers[currRendering].Length; i++) {
            carRenderers[currRendering][i].enabled = enable;
        }
    }
}