using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class SceneLoader : MonoBehaviour {

    private bool loadedScene = false;
    private bool allowSceneActivationInCoroutine = false;
    private float progress = 0f;

    [SerializeField]
    private int scene = 1;

    [SerializeField]
    private Text loadingText;

    [SerializeField]
    private Image progressBar;

    private void Update() {
        progressBar.fillAmount = progress;
        if (loadedScene) {
            loadingText.text = "START";
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
        }

        if (Input.touchCount >= 1 || Input.GetMouseButtonDown(0) || CrossPlatformInputManager.GetButtonDown("Shoot")) {
            allowSceneActivationInCoroutine = true;
        }

    }

    private void Start() {
        StartCoroutine(LoadNewScene());
    }

    // The coroutine runs on its own at the same time as Update() and takes an integer indicating which scene to load.
    private IEnumerator LoadNewScene() {
        yield return new WaitForSeconds(0.1f);
        var async = SceneManager.LoadSceneAsync(scene);
        async.allowSceneActivation = false;
        while (!allowSceneActivationInCoroutine) {
            if (async.progress >= 0.9f) {
                progress = 1f;
                loadedScene = true;
            } else {
                progress = async.progress;
            }
            yield return null;
        }
        async.allowSceneActivation = true;
    }
}