using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChanger : MonoBehaviour
{
    [SerializeField]
    private Image actionButtonImage;

    [SerializeField]
    private Sprite[] actionButtonSprites = new Sprite[2];

    public enum ButtonState
    {
        BUY_WARTRUCKS,
        CAPTURE_TURRET
    }

    private ButtonState actButtonState;

    // Use this for initialization
    void Start() {

    }

    public void ChangeButton(ButtonState buttonState) {
        switch (buttonState) {
            case ButtonState.BUY_WARTRUCKS:
                actionButtonImage.sprite = actionButtonSprites[0];
                break;
            case ButtonState.CAPTURE_TURRET:
                actionButtonImage.sprite = actionButtonSprites[1];
                break;
        }
    }

    public void SetTransparent(bool transparent) {
        if (transparent) {
            actionButtonImage.color = new Color(1, 1, 1, 0.5f);
        } else {
            actionButtonImage.color = new Color(1, 1, 1, 1);
        }
    }
}
