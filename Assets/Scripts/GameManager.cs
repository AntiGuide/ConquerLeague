using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private bool paused = false;

    public bool Paused { get { return paused; } set { paused=value; } }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (paused) {
            Time.timeScale = 0;
        }
    }
}