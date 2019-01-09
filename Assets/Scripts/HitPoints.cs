using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This units HitPoints, gets destroyed when its reduced to zero
/// </summary>
public class HitPoints : MonoBehaviour, IConfigurable {
    /// <summary>The units current hitpoints</summary>
    public byte aktHp;

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

    /// <summary>References the MoneyManagement script</summary>
    private MoneyManagement moneyManagement;

    /// <summary>The amount of money you get for killing</summary>
    [SerializeField, Tooltip("From top to bottom: Player, tower, minion")]
    private short[] moneyValue = new short[3];

    public bool Visible {
        set {
            healthBar.Active = value;
        }
    }

    public byte AktHp {
        get {
            return aktHp;
        }

        set {
            aktHp = value;
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
        healthBarParent = GameObject.Find("/HealthBars").transform;
        ConfigButton.ObjectsToUpdate.Add(this);
        SetFull();
        moneyManagement = GameObject.Find("Currency").GetComponent<MoneyManagement>();
        var bar = Instantiate(HPBarPrefab, healthBarParent);
        healthBar = bar.GetComponent<HealthBar>();
        healthBar.Offset = healthBarOffset;
        healthBar.Target = gameObject;
    }

    /// <summary>
    /// If the unit is a turret, its teamstate resets to neutral, if not it gets destroyed
    /// </summary>
    /// <param name="tag"></param>
    void OnDeath(string tag) {
        switch (tag) {
            case "Minion":
                moneyManagement.AddMoney(moneyValue[2]);
                Destroy(gameObject);
                break;
            case "Turret":
                moneyManagement.AddMoney(moneyValue[1]);
                teamHandler.TeamID = TeamHandler.TeamState.NEUTRAL;
                AktHp = saveHp;
                break;
            case "Player":
                SoundController.FSSoundController.StartSound(SoundController.Sounds.ENEMY_ELIMINATED);
                moneyManagement.AddMoney(moneyValue[0]);
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
}