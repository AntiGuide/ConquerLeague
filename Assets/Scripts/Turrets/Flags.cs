using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flags : MonoBehaviour, ISideAware {
    private enum Position {
        LEFT = 0,
        RIGHT
    }

    [SerializeField]
    private Position position;

    [SerializeField]
    private Material matBlue;

    [SerializeField]
    private Material matRed;

    private MeshRenderer meshRenderer;

    // Use this for initialization
    void Start() {
        CommunicationNet.FakeStatic.sideAwares.Add(this);
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void InitialUpdateColor() {
        switch (position) {
            case Position.LEFT:
                meshRenderer.material = GameManager.LeftTeam == TeamHandler.TeamState.FRIENDLY ? matBlue : matRed;
                break;
            case Position.RIGHT:
                meshRenderer.material = GameManager.RightTeam == TeamHandler.TeamState.FRIENDLY ? matBlue : matRed;
                break;
            default:
                break;
        }
    }
}