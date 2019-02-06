using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatVictoryScreen : MonoBehaviour {
    public void OnClickMainMenue() {
        CommunicationNet.FakeStatic?.client?.Disconnect("OnClickMainMenue");
        SceneManager.LoadScene(0);
    }

    public void OnClickStartGame() {
        CommunicationNet.FakeStatic?.client?.Disconnect("OnClickStartGame");
        SceneManager.LoadScene(1);
    }
}
