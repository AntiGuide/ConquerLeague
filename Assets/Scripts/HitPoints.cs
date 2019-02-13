using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This units HitPoints, gets destroyed when its reduced to zero
/// </summary>
public class HitPoints : MonoBehaviour, IConfigurable {
    //[HideInInspector]
    //public byte maxHp;

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

    private bool dead = false;

    private float colorTimer;

    private bool swappedColor;

    /// <summary>The amount of money you get for killing</summary>
    //[SerializeField, Tooltip("From top to bottom: Player, tower, minion")]
    //private short[] moneyValue = new short[3];

    public bool Visible {
        set {
            healthBar.gameObject.SetActive(value);
        }
    }

    public enum Damager {
        PLAYER_MG,
        PLAYER_RAM,
        TOWER,
    }

    public Damager LastDamager {
        get;
        set;
    }

    public byte AktHp {
        get {
            return aktHp;
        }

        set {
            if (aktHp == value || value < 0 || dead || value > saveHp) {
                return;
            }

            //Debug.Log("HP set to " + value);
            var oldHp = aktHp;
            aktHp = value;
            if (gameObject.tag == "Player" && teamHandler.TeamID == TeamHandler.TeamState.FRIENDLY) {
                if (oldHp > aktHp) {
                    StartCoroutine(HitFeedback());
                }
            }

            if (aktHp <= 0) {
                aktHp = 0;
                dead = true;
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

    private void Update() {
        if (gameObject.CompareTag("Player")) {
            float hpPro = (float)aktHp/(float)saveHp;

            if(hpPro <= 0.33f) {
                healthBar.fullHp.color = Color.red;
            } else if (hpPro <= 0.66f) {
                healthBar.fullHp.color = Color.yellow;
            } else {
                healthBar.fullHp.color = Color.green;
            }
        }
    }

    public void SetFull() {
        dead = false;
        AktHp = saveHp;
        healthBar?.ResetOverheat();
        //Debug.Log("Set to full.");
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
        healthBar.fullHp.color = Color.green;
        if (gameObject.CompareTag("Player") && teamHandler.TeamID == TeamHandler.TeamState.FRIENDLY) {
            overheatBar.transform.SetParent(bar.transform);
            overheatBar.transform.localPosition = Vector3.zero;
            var rectTrans = overheatBar.GetComponent<RectTransform>();
            var healthSize = bar.GetComponent<RectTransform>().sizeDelta;
            rectTrans.sizeDelta = healthSize;
            rectTrans.anchorMin = new Vector2(0.5f, 0f);
            rectTrans.anchorMax = new Vector2(0.5f, 0f);
            rectTrans.anchoredPosition = new Vector2(0f, -healthSize.y/2f);
            healthBar.Images.AddRange(overheatBar.GetComponents<Image>());
        }
        //aktHp = maxHp;
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
                CameraShake.FSCameraShake.StartCoroutine(CameraShake.Shake(0.5f, 0.5f));
                SoundController.FSSoundController.StartSound(SoundController.Sounds.TURRET_DESTRUCTION);
                FloatUpSpawner.GenerateFloatUp(moneyValue[1], FloatUp.ResourceType.GAS, Camera.main.WorldToScreenPoint(transform.position), 30);
                gameObject.GetComponentInChildren<TurretController>().Respawning = true;
                dead = false;
                if (teamHandler.TeamID == TeamHandler.TeamState.ENEMY) {
                    moneyManagement.AddMoney(moneyValue[1]);
                }
                aktHp += 1;

                KillfeedManager.FS.AddCustomLine("<color=#3C5EFFFF>You destroyed a tower</color>");

                break;
            case "Player":
                if (teamHandler.TeamID == TeamHandler.TeamState.ENEMY) {
                    SoundController.FSSoundController.StartSound(SoundController.Sounds.PLAYER_DESTRUCTION);
                    moneyManagement.AddMoney(moneyValue[0]);
                    goalManager.AddPoint(TeamHandler.TeamState.FRIENDLY);
                    goalManager.AddPoint(TeamHandler.TeamState.FRIENDLY);
                    FloatUpSpawner.GenerateFloatUp(moneyValue[0], FloatUp.ResourceType.GAS, Camera.main.WorldToScreenPoint(transform.position));
                }

                var go = Instantiate(playerDestruction, transform.position, transform.rotation);
                var destroyTime = playerDestruction.GetComponent<ParticleSystem>()?.main.duration ?? 1.2f;
                Destroy(go, destroyTime);
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