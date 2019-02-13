using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatVictoryScreen : MonoBehaviour {
    public void OnClickMainMenue() {
        CommunicationNet.FakeStatic?.client?.Shutdown("OnClickMainMenue");
        SceneManager.LoadScene(0);
    }

    public void OnClickRematch() {
        CommunicationNet.FakeStatic?.client?.Shutdown("OnClickStartGame");
        SceneManager.LoadScene(2);
    }
}
