using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This units HitPoints, gets destroyed when its reduced to zero
/// </summary>
public class HitPoints : MonoBehaviour, IConfigurable {
    [HideInInspector]
    public byte maxHp;

    /// <summary>The units current hitpoints</summary>
    private byte aktHp;

    /// <summary>Saves the units hp so that it can reset it if a tower gets destroyed</summary>
    [SerializeField]
    private byte saveHp;

    /// <summary>References the Team Handler script</summary>
    [SerializeField]
    private TeamHandler teamHandler;

    [SerializeField] private HealthBar healthBar;

    [SerializeField] private Vector3 healthBarOffset;

    [SerializeField] private GameObject HPBarPrefab;

    [SerializeField] private Transform healthBarParent;

    [SerializeField] private GameObject minionDestruction;

    [SerializeField] private GameObject playerDestruction;

    [SerializeField] private GameObject overheatBar;

    /// <summary>References the Goalmanager</summary>
    private GoalManager goalManager;

    private Image hitFeedBackImage;

    private short[] moneyValue = new short[3];

    /// <summary>References the MoneyManagement script</summary>
    private MoneyManagement moneyManagement;

    /// <summary>The amount of money you get for killing</summary>
    //[SerializeField, Tooltip("From top to bottom: Player, tower, minion")]
    //private short[] moneyValue = new short[3];

    public bool Visible {
        set {
            healthBar.gameObject.SetActive(value);
        }
    }

    public byte AktHp {
        get {
            return aktHp;
        }

        set {
            var oldHp = aktHp;
            aktHp = value;
            if (gameObject.tag == "Player" && teamHandler.TeamID == TeamHandler.TeamState.FRIENDLY) {
                if (oldHp > aktHp) {
                    StartCoroutine(HitFeedback());
                }
            }

            if (aktHp <= 0) {
                aktHp = 0;
                OnDeath(gameObject.tag);
            }
            healthBar?.UpdateBar();
        }
    }

    public HealthBar HealthBar { get { return healthBar; } set { healthBar = value; } }

    public byte SaveHp {
        get {
            return saveHp;
        }

        set {
            saveHp = value;
            if (healthBar == null) {
                return;
            }

            healthBar.MaxHp = saveHp;
            healthBar.UpdateBar();
        }
    }

    public void SetFull() {
        AktHp = saveHp;
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        if(gameObject.tag == "Player") {
            hitFeedBackImage = GameObject.Find("hitFeedBackImage").GetComponent<Image>();
        }

        goalManager = GameObject.Find("UIBackground").GetComponent<GoalManager>();
        healthBarParent = GameObject.Find("/HealthBars").transform;
        ConfigButton.ObjectsToUpdate.Add(this);
        SetFull();
        moneyManagement = GameObject.Find("Currency").GetComponent<MoneyManagement>();
        var bar = Instantiate(HPBarPrefab, healthBarParent);
        healthBar = bar.GetComponent<HealthBar>();
        healthBar.Offset = healthBarOffset;
        healthBar.Target = gameObject;
        if (gameObject.CompareTag("Player") && teamHandler.TeamID == TeamHandler.TeamState.FRIENDLY) {
            overheatBar.transform.SetParent(bar.transform);
            overheatBar.transform.localPosition = Vector3.zero;
            var rectTrans = overheatBar.GetComponent<RectTransform>();
            var healthSize = bar.GetComponent<RectTransform>().sizeDelta;
            rectTrans.sizeDelta = healthSize;
            rectTrans.anchorMin = new Vector2(0.5f, 0f);
            rectTrans.anchorMax = new Vector2(0.5f, 0f);
            rectTrans.anchoredPosition = new Vector2(0f, -healthSize.y/2f);
        }
        aktHp = maxHp;
    }

    /// <summary>
    /// If the unit is a turret, its teamstate resets to neutral, if not it gets destroyed
    /// </summary>
    /// <param name="tag"></param>
    void OnDeath(string tag) {
        switch (tag) {
            case "Minion":
                UpdateConfig();
                SoundController.FSSoundController.StartSound(SoundController.Sounds.WARTRUCK_DESTRUCTION);
                FloatUpSpawner.GenerateFloatUp(moneyValue[2], FloatUp.ResourceType.GAS, Camera.main.WorldToScreenPoint(transform.position));
                if (teamHandler.TeamID == TeamHandler.TeamState.ENEMY) {
                    moneyManagement.AddMoney(moneyValue[2]);
                    UltimateController.FS.AddCharge();
                }
                Instantiate(minionDestruction, transform.position, transform.rotation);
                CommunicationNet.FakeStatic.SendMinionHP((byte)gameObject.GetComponent<MinionNet>().Id, 0);
                Destroy(gameObject);
                break;
            case "Turret":
                aktHp += 1;
                CameraShake.FSCameraShake.StartCoroutine(CameraShake.Shake(0.5f, 0.5f));
                SoundController.FSSoundController.StartSound(SoundController.Sounds.TURRET_DESTRUCTION);
                FloatUpSpawner.GenerateFloatUp(moneyValue[1], FloatUp.ResourceType.GAS, Camera.main.WorldToScreenPoint(transform.position), 30);
                gameObject.GetComponentInChildren<TurretController>().Respawning = true;
                if (teamHandler.TeamID == TeamHandler.TeamState.ENEMY) {
                    moneyManagement.AddMoney(moneyValue[1]);
                }
                
                break;
            case "Player":
                SoundController.FSSoundController.StartSound(SoundController.Sounds.PLAYER_DESTRUCTION);
                if (teamHandler.TeamID == TeamHandler.TeamState.ENEMY) {
                    moneyManagement.AddMoney(moneyValue[0]);
                    goalManager.AddPoint(TeamHandler.TeamState.FRIENDLY);
                    FloatUpSpawner.GenerateFloatUp(moneyValue[0], FloatUp.ResourceType.GAS, Camera.main.WorldToScreenPoint(transform.position));
                }
                Instantiate(playerDestruction, transform.position, transform.rotation);
                GetComponent<PlayerNet>().OnDeath();
                break;
            default:
                Destroy(gameObject);
                break;
        }
    }

    public void UpdateConfig() {
        moneyValue[0] = ConfigButton.VehicleDestroyValue;
        moneyValue[1] = ConfigButton.TowerReward;
        moneyValue[2] = ConfigButton.MinionsDestroyValue;
    }

    private IEnumerator HitFeedback() {
        hitFeedBackImage.color = new Color32(255, 255, 255, 160);
        yield return new WaitForSeconds(0.25f);
        hitFeedBackImage.color = new Color32(255, 255, 255, 0);
    }
}