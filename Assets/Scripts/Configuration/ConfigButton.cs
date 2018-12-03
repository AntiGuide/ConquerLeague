using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IniParser;
using IniParser.Model;

public class ConfigButton : MonoBehaviour {

    public void OnButtonPress() {
        var parser = new FileIniDataParser();
        var data = parser.ReadFile("Config.ini");
        
        foreach (var section in data.Sections) {
            Debug.Log("" + section.SectionName +"");
            foreach (var key in section.Keys) {
                Debug.Log(key.KeyName + " = " + key.Value);
            }
        }
    }

    /// <summary>Use this for initialization</summary>
    void Start() {

    }

    /// <summary>Update is called once per frame</summary>
    void Update() {
        
    }
}
