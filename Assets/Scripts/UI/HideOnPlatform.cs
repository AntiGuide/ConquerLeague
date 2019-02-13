using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlatform : MonoBehaviour {
    public enum Platform {
        UNITY_STANDALONE,
        UNITY_ANDROID
    }

    public Platform Hide;

    private void Awake() {
#if UNITY_STANDALONE
        if (Hide == Platform.UNITY_STANDALONE) {
            gameObject.SetActive(false);
        }
#elif UNITY_ANDROID
        if (Hide == Platform.UNITY_ANDROID) {
            gameObject.SetActive(false);
        }
#endif

    }
}
