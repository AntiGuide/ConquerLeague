using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChanger : MonoBehaviour
{
    [SerializeField]
    private Image actionButtonImage;

    [SerializeField]
    private Image shootButtonImage;

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

    public void SetTransparent(bool isTransparent, bool isActionButton) {
        var image = isActionButton ? actionButtonImage : shootButtonImage;
        image.color = isTransparent ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
    }
}
