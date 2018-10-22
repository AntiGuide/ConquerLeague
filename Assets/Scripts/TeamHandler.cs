using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamHandler : MonoBehaviour {

    /// <summary>This units team-ID</summary>
    [SerializeField]
    private TeamState teamID = 0;

    public enum TeamState
    {
        FRIENDLY = 0,
        ENEMY = 1,
        NEUTRAL = 2
    };

    public TeamState TeamID {
        get { return teamID; }
        set { teamID=value; }
    }
}
