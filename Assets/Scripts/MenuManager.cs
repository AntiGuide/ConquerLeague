using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private Transform cameraDestinationGarage;

    [SerializeField]
    private Transform cameraDestinationBattle;

    [SerializeField]
    private float cameraSpeed = 1;

    [SerializeField]
    private Camera mainCamera;

    private Transform cameraDestination;
    private bool moveToGarage = false;
    private bool cameraMoving;
    private float movingTimer = 0;
    private RaycastHit hit;
    private Ray ray;

    private Transform cameraStartTrans;

    private void Start() {
        cameraStartTrans = transform;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && !cameraMoving) {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("ButtonMainMenu"))) {
                cameraMoving = true;
                cameraDestination = hit.transform.gameObject.CompareTag("Garage") ? cameraDestinationGarage : cameraDestinationBattle;
            }
        }

        if (cameraMoving) {
            movingTimer += Time.deltaTime * cameraSpeed;
            LerpTransform(cameraStartTrans, cameraDestination, movingTimer);

            if (movingTimer >= 1) {
                cameraMoving = false;
                movingTimer = 0;
            }
        }
    }

    public void OnClickMainMenue() {
        SceneManager.LoadScene(0);
    }

    public void OnClickStartGame() {
        SceneManager.LoadScene(1);
    }

    private void LerpTransform(Transform startTrans, Transform goalTrans, float movingTimer) {
        transform.position = Vector3.Lerp(startTrans.position, goalTrans.position, movingTimer);
        transform.rotation = Quaternion.Lerp(transform.rotation, goalTrans.rotation, movingTimer);
    }
}