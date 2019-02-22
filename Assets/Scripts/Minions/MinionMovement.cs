using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the minions curent target and movement
/// </summary>
public class MinionMovement : MonoBehaviour, IConfigurable {
    public static int lastLane;
    /// <summary>The minions attached hpbar</summary>
    [SerializeField]
    private GameObject healthBar;

    /// <summary>The minions movement order</summary>
    [SerializeField, Tooltip("The transform-targets, which tells the minions where to go and in which order. Priority from top to bottom.")]
    private Transform[] movementOrder;

    /// <summary>The distance when the minion swaps its target</summary>
    [SerializeField]
    private float swapDistance;

    [SerializeField]
    private MinionCloth[] minionCloth = new MinionCloth[2];

    [SerializeField]
    private GameObject minionDestruction;

    /// <summary>The minions Teamhandler</summary>
    private TeamHandler teamHandler;

    /// <summary>The array index of the currently chased target</summary>
    private int currTarget = 1;

    /// <summary>References the minions attached NavMeshAgent</summary>
    [SerializeField]
    private NavMeshAgent agent;
    private GoalManager goalManager;

    /// <summary>
    /// Instantiates the minions hpbar when it spawns
    /// </summary>
    /// <param name="parent"></param>
    public void OnInitialize() {
        for (int i = 0; i < minionCloth.Length; i++) {
            minionCloth[i].InitializeColor();
        }
    }

    public void UpdateConfig() {
        GetComponent<HitPoints>().SaveHp = ConfigButton.MinionsHP;
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        teamHandler = gameObject.GetComponent<TeamHandler>();
        lastLane = lastLane == 0 ? 1 : 0;

        if (teamHandler.TeamID == GameManager.LeftTeam) {
            agent.Warp(GameObject.Find("SpawnLeft").transform.position);
            this.transform.eulerAngles = new Vector3(0, 90, 0);
            movementOrder = GameObject.Find("Waypoint_F" + lastLane).GetComponentsInChildren<Transform>();
        } else {
            agent.Warp(GameObject.Find("SpawnRight").transform.position);
            this.transform.eulerAngles = new Vector3(0, -90, 0);
            movementOrder = GameObject.Find("Waypoint_E" + lastLane).GetComponentsInChildren<Transform>();
        }


        if (teamHandler.TeamID == TeamHandler.TeamState.FRIENDLY) {
            agent.destination = movementOrder[currTarget].position;
        }


        goalManager = GameObject.Find("UIBackground").GetComponent<GoalManager>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if (teamHandler.TeamID == TeamHandler.TeamState.ENEMY) {
            return;
        }

        if (movementOrder.Length == currTarget) {
            GetComponent<MinionNet>().DeInitNet();
            gameObject.SetActive(false);
            SoundController.FSSoundController.StartSound(SoundController.Sounds.TURRET_DESTRUCTION);
            CameraShake.Instance.StartCoroutine(CameraShake.Instance.Shake());
            var explosion = Instantiate(minionDestruction, transform.position, transform.rotation);
            explosion.transform.localScale = new Vector3(3, 3, 3);
            Destroy(gameObject);
            goalManager.AddPoint(TeamHandler.TeamState.FRIENDLY);
            KillfeedManager.FS.AddCustomLine("<color=#3C5EFFFF>A minion earned you a point</color>");
            UltimateController.FS.AddCharge();
            return;
        }

        if (agent.remainingDistance <= swapDistance) {
            currTarget++;

            if (currTarget < movementOrder.Length) {
                agent.destination = movementOrder[currTarget].position;
            }
        }
    }
}