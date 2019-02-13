using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChanger : MonoBehaviour
{
    public static ButtonChanger FSButtonChanger;

    [SerializeField]
    private Image[] buttonImages = new Image[3];

    [SerializeField]
    private Sprite[] actionButtonSprites = new Sprite[2];

    public enum ActionButtonState : int
    {
        BUY_WARTRUCKS,
        CAPTURE_TURRET
    }

    public enum Buttons : int
    {
        ACTION_BUTTON = 0,
        SHOOT_BUTTON = 1,
        ULTIMATE_BUTTON = 2
    }

    private ActionButtonState actButtonState;

    // Use this for initialization
    void Start() {
        if(FSButtonChanger != null) {
            Application.Quit();
        }

        FSButtonChanger = this;
#if UNITY_STANDALONE
        var go = new GameObject[3];
        go[0] = GameObject.Find("/Canvas/ActionButton");
        go[1] = GameObject.Find("/Canvas/ShootButton");
        go[2] = GameObject.Find("/Canvas/UltiShootButton");

        for (int i = 0; i < buttonImages.Length; i++) {
            if (go[i] == null) {
                Debug.Log("Button" + i + " in ButtonChanger not found");
                break;
            }

            buttonImages[i] = go[i].GetComponent<Image>();
        }
#endif
        SetTransparent(true, Buttons.ULTIMATE_BUTTON);
    }

    public void ChangeButton(ActionButtonState buttonState) {
        buttonImages[(int)Buttons.ACTION_BUTTON].sprite = actionButtonSprites[(int)buttonState];
    }

    public void SetTransparent(bool isTransparent, Buttons button) {
        var image = buttonImages[(int)button];
        image.color = isTransparent ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
    }
}