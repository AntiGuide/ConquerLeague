using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the minions curent target and movement
/// </summary>
public class MinionMovement : MonoBehaviour, IConfigurable {
    /// <summary>The minions attached hpbar</summary>
    [SerializeField]
    private GameObject healthBar;

    /// <summary>The minions movement order</summary>
    [SerializeField, Tooltip("The transform-targets, which tells the minions where to go and in which order. Priority from top to bottom.")]
    private Transform[] movementOrder;

    /// <summary>The distance when the minion swaps its target</summary>
    [SerializeField]
    private float swapDistance;

    /// <summary>True if minion swapped target</summary>
    private bool swappedTarget = false;

    /// <summary>References the Goalmanager</summary>
    private GoalManager goalManager;

    /// <summary>The minions Teamhandler</summary>
    private TeamHandler teamHandler;

    /// <summary>The array index of the currently chased target</summary>
    private int currTarget = 1;

    private Vector3 lastTargetPosition;

    /// <summary>References the minions attached NavMeshAgent</summary>
    [SerializeField]
    private NavMeshAgent agent;

    /// <summary>
    /// Instantiates the minions hpbar when it spawns
    /// </summary>
    /// <param name="parent"></param>
    public void OnInitialize(Transform parent) {
        //var aktHpBar = Instantiate(healthBar, parent);
        //aktHpBar.GetComponent<HealthBar>().Target = gameObject;
    }

    public void UpdateConfig() {
        GetComponent<HitPoints>().SaveHp = ConfigButton.MinionsHP;
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        teamHandler = gameObject.GetComponent<TeamHandler>();
        goalManager = GameObject.Find("UIBackground").GetComponent<GoalManager>();

        if (teamHandler.TeamID == GameManager.LeftTeam) {
            agent.Warp(GameObject.Find("SpawnLeft").transform.position);
            this.transform.eulerAngles = new Vector3(0, 90, 0);
            movementOrder = GameObject.Find("Waypoint_F" + Random.Range(0, 2)).GetComponentsInChildren<Transform>();
        } else {
            agent.Warp(GameObject.Find("SpawnRight").transform.position);
            this.transform.eulerAngles = new Vector3(0, -90, 0);
            movementOrder = GameObject.Find("Waypoint_E" + Random.Range(0, 2)).GetComponentsInChildren<Transform>();
        }

        agent.destination = movementOrder[currTarget].position;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if (teamHandler.TeamID == TeamHandler.TeamState.ENEMY) {
            return;
        }

        if (movementOrder.Length == currTarget) {
            goalManager.AddPoint(TeamHandler.TeamState.FRIENDLY);
            GetComponent<MinionNet>().DeInitNet();
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        if (agent.remainingDistance <= swapDistance && !swappedTarget) {
            lastTargetPosition = agent.destination;
            currTarget++;

            if (currTarget < movementOrder.Length) {
                agent.destination = movementOrder[currTarget].position;
            }

            swappedTarget = true;
        }

        if (lastTargetPosition != agent.destination && swappedTarget) {
            swappedTarget = false;
        }
    }
}