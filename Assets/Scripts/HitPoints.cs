using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This units HitPoints, gets destroyed when its reduced to zero
/// </summary>
public class HitPoints : MonoBehaviour {
    /// <summary>The units current hitpoints</summary>
    public int hitPoints;

    private int hp;

    /// <summary>
    /// The units category, which decides the amount of hp it has
    /// </summary>
    public enum UnitCategory
    {
        STANDARD_PLAYER = 0,
        MINION = 1,
        TOWER = 2
    }

	/// <summary>
    /// 
    /// </summary>
	void Start () {
		
	}
	
	/// <summary>
    /// 
    /// </summary>
	void Update () {

	}
}
