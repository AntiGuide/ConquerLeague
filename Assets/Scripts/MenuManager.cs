using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    public void OnClickMainMenue() {
        SceneManager.LoadScene(0);
    }

    public void OnClickStartGame() {
        SceneManager.LoadScene(1);
    }
}
