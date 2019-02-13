using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private Transform cameraDestGarageTrans;

    [SerializeField]
    private Transform cameraDestBattleTrans;

    [SerializeField]
    private float cameraSpeed = 1;

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private GameObject startUI;

    [SerializeField]
    private GameObject tutorialUI;

    [SerializeField]
    private GameObject tutorialButton;

    [SerializeField]
    private Collider garageCollider;


    private Vector3 cameraGaragePos;
    private Quaternion cameraGarageRotation;

    private Vector3 cameraBattlePos;
    private Quaternion cameraBattleRotation;

    private Vector3 cameraStartPosition;
    private Quaternion cameraStartRotation;

    private Vector3 cameraStartMovingPosition;
    private Quaternion cameraStartMovingRotation;

    private Vector3 destinationPos;
    private Quaternion destinationRotation;
   
    private bool cameraMoving = false;
    private float movingTimer = 0;
    private RaycastHit hit;
    private Ray ray;

    private void Start() {
        startUI.SetActive(false);
        tutorialUI.SetActive(false);
        cameraStartPosition = transform.position;
        cameraStartRotation = transform.rotation;

        cameraGaragePos = cameraDestGarageTrans.position;
        cameraGarageRotation = cameraDestGarageTrans.rotation;

        cameraBattlePos = cameraDestBattleTrans.position;
        cameraBattleRotation = cameraDestBattleTrans.rotation;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && !cameraMoving && !EventSystem.current.IsPointerOverGameObject()) {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.Raycast(ray, out hit, 50, LayerMask.GetMask("ButtonMainMenu"))) {
                cameraStartMovingPosition = transform.position;
                cameraStartMovingRotation = transform.rotation;
                var rayTarget = hit.transform.gameObject;
                
                switch (rayTarget.tag) {
                    case "Garage":
                        garageCollider.enabled = false;
                        destinationPos = cameraGaragePos;
                        destinationRotation = cameraGarageRotation;
                        break;
                    case "BattleButton":
                        startUI.SetActive(true);
                        destinationPos = cameraBattlePos;
                        destinationRotation = cameraBattleRotation;
                        break;
                    case "CarSelection":
                        rayTarget.GetComponent<CarSelection>().SwapCarRenderer();
                        break;
                    case "MenuBack":
                        garageCollider.enabled = true;
                        OnClickBack();
                        break;
                }
                cameraMoving = true;
            }
        }

        if (cameraMoving) {
            movingTimer += Time.deltaTime * cameraSpeed;
            LerpTransform(cameraStartMovingPosition, cameraStartMovingRotation, destinationPos, destinationRotation, movingTimer);
            if (movingTimer >= 1) {
                movingTimer = 0;
                cameraMoving = false;
            }
        }
    }

    public void OnClickMainMenue() {
        CommunicationNet.FakeStatic?.client?.Disconnect("OnClickMainMenue");
        SceneManager.LoadScene(0);
    }

    public void OnClickStartGame() {
        CommunicationNet.FakeStatic?.client?.Disconnect("OnClickStartGame");
        SceneManager.LoadScene(1);
    }

    public void OnClickBack() {
        if (!cameraMoving) {
            if (startUI.activeSelf) {
                startUI.SetActive(false);
            }

            cameraStartMovingPosition = transform.position;
            cameraStartMovingRotation = transform.rotation;

            destinationPos = cameraStartPosition;
            destinationRotation = cameraStartRotation;

            cameraMoving = true;
        }
    }

    public void OnClickTutorial() {
        tutorialUI.SetActive(true);
        tutorialButton.SetActive(false);
    }

    private void LerpTransform(Vector3 startPos, Quaternion startRotation, Vector3 goalPos, Quaternion goalRotation, float movingTimer) {
        transform.position = Vector3.Lerp(startPos, goalPos, movingTimer);
        transform.rotation = Quaternion.Lerp(startRotation, goalRotation, movingTimer);
    }
}