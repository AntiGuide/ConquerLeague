using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IniParser;
using IniParser.Model;

public class ConfigButton : MonoBehaviour {
    private Dictionary<string, string> vehicle = new Dictionary<string, string>();
    private Dictionary<string, string> vehicleMachineGun = new Dictionary<string, string>();
    private Dictionary<string, string> minions = new Dictionary<string, string>();
    private Dictionary<string, string> tower = new Dictionary<string, string>();

    public void OnButtonPress() {
        var parser = new FileIniDataParser();
        var data = parser.ReadFile("Config.ini");

        vehicle.Clear();
        foreach (var key in data.Sections["Vehicle"]) {
            vehicle.Add(key.KeyName, key.Value);
        }

        vehicleMachineGun.Clear();
        foreach (var key in data.Sections["VehicleMachineGun"]) {
            vehicleMachineGun.Add(key.KeyName, key.Value);
        }

        minions.Clear();
        foreach (var key in data.Sections["Minions"]) {
            minions.Add(key.KeyName, key.Value);
        }

        tower.Clear();
        foreach (var key in data.Sections["Tower"]) {
            tower.Add(key.KeyName, key.Value);
        }

        Debug.Log("Imported new settings");
    }

    /// <summary>Use this for initialization</summary>
    void Start() {

    }

    /// <summary>Update is called once per frame</summary>
    void Update() {
        
    }
}
