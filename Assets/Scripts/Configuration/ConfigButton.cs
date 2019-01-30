using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IniParser;
using UnityEngine.UI;
using System.Globalization;
using System.IO;

public class ConfigButton : MonoBehaviour {
    public Text DebugText;

    public static HashSet<IConfigurable> ObjectsToUpdate = new HashSet<IConfigurable>();

    private static Dictionary<string, string> game              = new Dictionary<string, string>();
    private static Dictionary<string, string> vehicle           = new Dictionary<string, string>();
    private static Dictionary<string, string> vehicleMachineGun = new Dictionary<string, string>();
    private static Dictionary<string, string> minions           = new Dictionary<string, string>();
    private static Dictionary<string, string> tower             = new Dictionary<string, string>();

    //Game
    public static float GameTime { get { return float.Parse(game["TIME"], CultureInfo.InvariantCulture.NumberFormat); } }

    // Vehicle
    public static short VehicleDestroyValue  { get { return short.Parse(vehicle["DESTROY_VALUE"]); } }
    public static byte  VehicleHP            { get { return byte.Parse(vehicle["HP"]); } }
    public static int   VehicleRespawnTime   { get { return int.Parse(vehicle["RESPAWN_TIME"]); } }

    // VehicleMachineGun
    public static float VehicleMGSpeed             { get { return float.Parse(vehicleMachineGun["SPEED"], CultureInfo.InvariantCulture.NumberFormat); } }
    public static int   VehicleMGTuningSpeed       { get { return int.Parse(vehicleMachineGun["TURNING_SPEED"]); } }
    public static float VehicleMGRange             { get { return float.Parse(vehicleMachineGun["RANGE"], CultureInfo.InvariantCulture.NumberFormat); } }
    public static byte  VehicleMGDamagePerShot     { get { return byte.Parse(vehicleMachineGun["DAMAGE_PER_SHOT"]); } }
    public static int   VehicleMGShotsPerSecond    { get { return int.Parse(vehicleMachineGun["SHOTS_PER_SECOND"]); } }
    public static float VehicleMGCooldownPerSecond { get { return float.Parse(vehicleMachineGun["COOLDOWN_PER_SECOND"], CultureInfo.InvariantCulture.NumberFormat); } }
    public static float VehicleMGCooldownDelay     { get { return float.Parse(vehicleMachineGun["COOLDOWN_DELAY"], CultureInfo.InvariantCulture.NumberFormat); } }
    public static float VehicleMGOverheatPerShot   { get { return float.Parse(vehicleMachineGun["OVERHEAT_PER_SHOT"], CultureInfo.InvariantCulture.NumberFormat); } }

    // Minions
    public static byte  MinionsHP            { get { return byte.Parse(minions["HP"]); } }
    public static int   MinionsVelocity      { get { return int.Parse(minions["VELOCITY"]); } }
    public static short MinionsBuyValue      { get { return short.Parse(minions["BUY_VALUE"]); } }
    public static short MinionsDestroyValue  { get { return short.Parse(minions["DESTROY_VALUE"]); } }
    public static int   MinionsHealAmount    { get { return int.Parse(minions["HEAL_AMOUNT"]); } }

    // Tower
    public static byte  TowerHP              { get { return byte.Parse(tower["HP"]); } }
    public static int   TowerShotsPerSecond  { get { return int.Parse(tower["SHOTS_PER_SECOND"]); } }
    public static byte  TowerDamagePerShot   { get { return byte.Parse(tower["DAMAGE_PER_SHOT"]); } }
    public static int   TowerRange           { get { return int.Parse(tower["RANGE"]); } }
    public static short TowerReward          { get { return short.Parse(tower["REWARD"]); } }
    public static int   TowerRespawnTime     { get { return int.Parse(tower["RESPAWN_TIME"]); } }
    public static int   TowerRewardCapture   { get { return int.Parse(tower["REWARD_CAPTURE"]); } }

    public void OnButtonPress() {
        //DebugText.text = Application.persistentDataPath + "/Config.ini";
        if (!File.Exists(Application.persistentDataPath + "/Config.ini")) {
            DebugText.text = "Config.ini doesn't exist at \"" + Application.persistentDataPath + "/Config.ini\"";
            Debug.Log("Config.ini doesn't exist at \"" + Application.persistentDataPath + "/Config.ini\"");
            return;
        }
        
        var parser = new FileIniDataParser();
        var data = parser.ReadFile(Application.persistentDataPath + "/Config.ini");

        game.Clear();
        vehicle.Clear();
        vehicleMachineGun.Clear();
        minions.Clear();
        tower.Clear();

        foreach (var key in data.Sections["Game"]) {
            game.Add(key.KeyName, key.Value);
        }

        foreach (var key in data.Sections["Vehicle"]) {
            vehicle.Add(key.KeyName, key.Value);
        }

        foreach (var key in data.Sections["VehicleMachineGun"]) {
            vehicleMachineGun.Add(key.KeyName, key.Value);
        }

        foreach (var key in data.Sections["Minions"]) {
            minions.Add(key.KeyName, key.Value);
        }

        foreach (var key in data.Sections["Tower"]) {
            tower.Add(key.KeyName, key.Value);
        }

        foreach (var o in ObjectsToUpdate) {
            o.UpdateConfig();
        }

        /*
        Debug.Log("Imported new settings");
        Debug.Log("---------------------");
        Debug.Log(GameTime);
        Debug.Log(VehicleDestroyValue);
        Debug.Log(VehicleHP);
        Debug.Log(VehicleRespawnTime);
        Debug.Log(nameof(VehicleMGSpeed) + ": " + VehicleMGSpeed);
        debugText.text += nameof(VehicleMGSpeed) + ": " + VehicleMGSpeed + System.Environment.NewLine;
        Debug.Log(VehicleMGTuningSpeed);
        Debug.Log(VehicleMGRange);
        Debug.Log(VehicleMGDamagePerShot);
        Debug.Log(VehicleMGShotsPerSecond);
        Debug.Log(MinionsHP);
        Debug.Log(MinionsVelocity);
        Debug.Log(MinionsBuyValue);
        Debug.Log(MinionsDestroyValue);
        Debug.Log(MinionsHealAmount);
        Debug.Log(TowerHP);
        Debug.Log(TowerShotsPerSecond);
        Debug.Log(TowerDamagePerShot );
        Debug.Log(TowerRange);
        Debug.Log(TowerReward);
        Debug.Log(TowerRespawnTime);
        Debug.Log(TowerRewardCapture);*/
    }

    private void Awake() {
        OnButtonPress();
    }

    private void Start() {
        OnButtonPress();
    }
}
