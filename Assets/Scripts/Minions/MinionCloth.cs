using UnityEngine;

public class MinionCloth : MonoBehaviour {
    [SerializeField]
    private TeamHandler teamHandler;

    [SerializeField]
    private Material matBlue;

    [SerializeField]
    private Material matRed;

    private bool isEnemy;

    public void InitializeColor() {
        if (teamHandler.TeamID == TeamHandler.TeamState.NEUTRAL) {
            return;
        }

        GetComponent<MeshRenderer>().material = teamHandler.TeamID == TeamHandler.TeamState.FRIENDLY ? matBlue : matRed;
    }
}
