using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class Autoexec {
    static Autoexec() {
        //Debug.Log("No Emulation");
        EditorApplication.delayCall += NoEmulation;
    }
    static void NoEmulation() {
        EditorApplication.ExecuteMenuItem("Edit/Graphics Emulation/No Emulation");
        EditorApplication.delayCall -= NoEmulation;
    }
}