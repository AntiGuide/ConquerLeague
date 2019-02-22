using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This units HitPoints, gets destroyed when its reduced to zero
/// </summary>
public class HitPoints : MonoBehaviour {
    /// <summary>The units current hitpoints</summary>
    private byte aktHp;

    /// <summary>Saves the units hp so that it can reset it if a tower gets destroyed</summary>
    [SerializeField] private byte saveHp;

    /// <summary>References the Team Handler script</summary>
    [SerializeField] private TeamHandler teamHandler;

    /// <summary>Reference to the units HealthBar</summary>
    [SerializeField] private HealthBar healthBar;

    /// <summary>The offset for the HPBar reference point</summary>
    [SerializeField] private Vector3 healthBarOffset;

    /// <summary>The prefab for a generic HPBar</summary>
    [SerializeField] private GameObject healthBarPrefab;

    /// <summary>The parent Transform for all health bars</summary>
    [SerializeField] private Transform healthBarParent;

    /// <summary>The prefab for the minion destruction VFX</summary>
    [SerializeField] private GameObject minionDestruction;

    /// <summary>The prefab for the player destruction VFX</summary>
    [SerializeField] private GameObject playerDestruction;

    /// <summary>Holds a reference to the overheat bar if script is placed on a player </summary>
    [SerializeField] private GameObject overheatBar;

    /// <summary>References the Goalmanager</summary>
    private GoalManager goalManager;

    /// <summary>Reference to the image displayed when the player get damaged</summary>
    private Image hitFeedBackImage;

    /// <summary>The money reward for different enemys</summary>
    private short[] moneyValue = new short[3];

    /// <summary>References the MoneyManagement script</summary>
    private MoneyManagement moneyManagement;

    /// <summary>Marks if the unit is dead at the moment</summary>
    private bool dead = false;

    /// <summary>The different damage sources in the game</summary>
    public enum Damager {
        PLAYER_MG,
        PLAYER_RAM,
        TOWER,
    }

    /// <summary>Gets/Sets the last damager of this unit</summary>
    public Damager LastDamager { get; set; }

    /// <summary>Gets/Sets the HP of this unit</summary>
    public byte AktHp {
        get {
            return aktHp;
        }

        set {
            if (aktHp == value || value < 0 || dead || value > saveHp) {
                return;
            }
            
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

    /// <summary>Gets/Sets the healthBar reference for this unit</summary>
    public HealthBar HealthBar {
        get {
            return healthBar;
        }

        set {
            healthBar = value;
        }
    }
    
    /// <summary>Gets/Sets the maximum HP for this unit</summary>
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

    /// <summary>
    /// Fills the HP of a unit and deactivates the dead state
    /// </summary>
    public void SetFull() {
        dead = false;
        AktHp = saveHp;
        healthBar?.ResetOverheat();
    }

    /// <summary>
    /// Update is called once per frame by Unity
    /// </summary>
    private void Update() {
        float healthPercentage = (float)aktHp / (float)saveHp;

        if (healthPercentage <= 0.33f) {
            healthBar.fullHp.color = Color.red;
        } else if (healthPercentage <= 0.66f) {
            healthBar.fullHp.color = Color.yellow;
        } else {
            healthBar.fullHp.color = Color.green;
        }
    }

    /// <summary>
    /// Start is called on object initialization by Unity
    /// </summary>
    private void Start() {
        moneyValue[0] = 40;
        moneyValue[1] = 10;
        moneyValue[2] = 10;

        if (gameObject.tag == "Player") {
            hitFeedBackImage = GameObject.Find("hitFeedBackImage").GetComponent<Image>();
        }

        goalManager = GameObject.Find("UIBackground").GetComponent<GoalManager>();
        healthBarParent = GameObject.Find("/HealthBars").transform;
        SetFull();
        moneyManagement = GameObject.Find("Currency").GetComponent<MoneyManagement>();
        var bar = Instantiate(healthBarPrefab, healthBarParent);
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
            rectTrans.anchoredPosition = new Vector2(0f, -healthSize.y / 2f);
            healthBar.Images.AddRange(overheatBar.GetComponents<Image>());
        }
    }

    /// <summary>
    /// If the unit is a turret, its teamstate resets to neutral, if not it gets destroyed
    /// </summary>
    /// <param name="tag"></param>
    void OnDeath(string tag) {
        switch (tag) {
            case "Minion":
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
                CameraShake.Instance.StartCoroutine(CameraShake.Instance.Shake());
                SoundController.FSSoundController.StartSound(SoundController.Sounds.TURRET_DESTRUCTION);
                FloatUpSpawner.GenerateFloatUp(moneyValue[1], FloatUp.ResourceType.GAS, Camera.main.WorldToScreenPoint(transform.position), 30);
                gameObject.GetComponentInChildren<TurretController>().Respawning = true;
                dead = false;
                if (teamHandler.TeamID == TeamHandler.TeamState.ENEMY) {
                    moneyManagement.AddMoney(moneyValue[1]);
                    KillfeedManager.FS.AddCustomLine("<color=#3C5EFFFF>You destroyed a tower</color>");
                }

                aktHp += 1;
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

    /// <summary>
    /// Displays a hit feedback for the player for 0.25 seconds
    /// </summary>
    private IEnumerator HitFeedback() {
        hitFeedBackImage.color = new Color32(255, 255, 255, 160);
        yield return new WaitForSeconds(0.25f);
        hitFeedBackImage.color = new Color32(255, 255, 255, 0);
    }
}