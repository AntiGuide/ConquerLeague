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
        if(FSButtonChanger == null) {
            Application.Quit();
        }
        FSButtonChanger = this;
        SetTransparent(true, Buttons.ULTIMATE_BUTTON);
#if UNITY_STANDALONE
        var go = GameObject.Find("/Canvas/ActionButton");
        if (go == null) {
            Debug.Log("ActionButton not found");
        }

        actionButtonImage = go.GetComponent<Image>();

        go = GameObject.Find("/Canvas/ShootButton");
        if (go == null) {
            Debug.Log("ShootButton not found");
        }

        actionButtonImage = go.GetComponent<Image>();
#endif
    }

    public void ChangeButton(ActionButtonState buttonState) {
        buttonImages[(int)Buttons.ACTION_BUTTON].sprite = actionButtonSprites[(int)buttonState];
    }

    public void SetTransparent(bool isTransparent, Buttons button) {
        var image = buttonImages[(int)button];
        image.color = isTransparent ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
    }
}