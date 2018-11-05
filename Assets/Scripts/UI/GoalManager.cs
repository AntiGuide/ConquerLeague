using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalManager : MonoBehaviour {
    /// <summary>The Text which displays the current goals</summary>
    [SerializeField]
    private Text goalText;

    private int myGoals;

    private int enemyGoals;

    public int MyGoals {get { return myGoals;} set { myGoals=value;} }

    public int EnemyGoals { get { return enemyGoals; } set { enemyGoals=value; } }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        goalText.text = myGoals + " / " + enemyGoals;
	}
}
