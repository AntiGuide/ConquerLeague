using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flare : MonoBehaviour, ISideAware {
    private enum Position
    {
        LEFT = 0,
        MID,
        RIGHT
    }

    [SerializeField]
    private Position position;

    [SerializeField]
    private Material matBlue;

    [SerializeField]
    private Material matRed;

    [SerializeField]
    private Material matWhite;

    [SerializeField]
    private Gradient smokeColorRed;

    [SerializeField]
    private Gradient smokeColorBlue;

    [SerializeField]
    private Gradient smokeColorWhite;

    private ParticleSystem smokeParticle;

	// Use this for initialization
	void Start () {
        CommunicationNet.FakeStatic.sideAwares.Add(this);
        smokeParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void InitialUpdateColor() {
        var grad = smokeParticle.colorOverLifetime;
        switch (position) {
            case Position.LEFT:
                if (GameManager.LeftTeam == TeamHandler.TeamState.FRIENDLY) {
                    GetComponent<Renderer>().material = matBlue;
                    grad.color = smokeColorBlue;
                } else {
                    GetComponent<Renderer>().material = matRed;
                    grad.color = smokeColorRed;
                }
                break;
            case Position.MID:
                GetComponent<Renderer>().material = matWhite;
                grad.color = smokeColorWhite;
                break;
            case Position.RIGHT:
                if (GameManager.RightTeam == TeamHandler.TeamState.FRIENDLY) {
                    GetComponent<Renderer>().material = matBlue;
                    grad.color = smokeColorBlue;
                } else {
                    GetComponent<Renderer>().material = matRed;
                    grad.color = smokeColorRed;
                }
                break;
            default:
                break;
        }
    }

    public void UpdateColor(TeamHandler.TeamState teamState) {
        var grad = smokeParticle.colorOverLifetime;
        switch (teamState) {
            case TeamHandler.TeamState.FRIENDLY:
                GetComponent<Renderer>().material = matBlue;
                grad.color = smokeColorBlue;
                break;
            case TeamHandler.TeamState.ENEMY:
                GetComponent<Renderer>().material = matRed;
                grad.color = smokeColorRed;
                break;
            case TeamHandler.TeamState.NEUTRAL:
                GetComponent<Renderer>().material = matWhite;
                grad.color = smokeColorWhite;
                break;
            default:
                break;
        }
    }
}