using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class handles the behaviour of the player object after network packets came in
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerNet : MonoBehaviour {
    /// <summary>Marks if the attached object is an enemy</summary>
    private bool isEnemy;

    /// <summary>The rigidbody of this object</summary>
    private Rigidbody rigidbodyPlayer;

    public Transform StartPoint { get; set; }

    /// <summary>
    /// Handles new data from the network component
    /// </summary>
    /// <param name="position">The new positon of the player</param>
    /// <param name="quaternion">The new rotation of the player</param>
    /// <param name="velocity">The new velocity of the player</param>
    /// <param name="hp">The new hp of the player</param>
    public void SetNewMovementPack(Vector3 position, Quaternion quaternion, Vector3 velocity, byte hp = 1) {
        transform.position = position;
        transform.rotation = quaternion;
        rigidbodyPlayer.velocity = velocity;
    }

    public void OnDeath() {
        if (!isEnemy) {
            CommunicationNet.FakeStatic.SendPlayerDeath();
        }

        StartCoroutine(InitRespawn());
    }

    public void OnNetDeath() {
        StopCoroutine(InitRespawn());
        StartCoroutine(InitRespawn());
    }

    public IEnumerator InitCountdown() {
        GameManager.CountdownTextFakeStatic.text = "5";
        yield return new WaitForSeconds(1f);
        GameManager.CountdownTextFakeStatic.text = "4";
        yield return new WaitForSeconds(1f);
        GameManager.CountdownTextFakeStatic.text = "3";
        yield return new WaitForSeconds(1f);
        GameManager.CountdownTextFakeStatic.text = "2";
        yield return new WaitForSeconds(1f);
        GameManager.CountdownTextFakeStatic.text = "1";
        yield return new WaitForSeconds(1f);
        GameManager.CountdownTextFakeStatic.text = "GO!";
        yield return new WaitForSeconds(1f);
        GameManager.CountdownTextFakeStatic.text = "";
        yield return null;
    }

    public IEnumerator InitRespawn() {
        transform.position = StartPoint.position;
        transform.rotation = StartPoint.rotation;
        transform.GetChild(0).gameObject.SetActive(false);
        
        GetComponent<Collider>().enabled = false;
        if (GetComponent<VehicleController>() != null) {
            GetComponent<VehicleController>().enabled = false;
        }

        //gameObject.SetActive(false);
        if (!isEnemy) {
            StartCoroutine(InitCountdown());
        }

        yield return new WaitForSeconds(5f);
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<Collider>().enabled = true;
        if (GetComponent<VehicleController>() != null) {
            GetComponent<VehicleController>().enabled = true;
        }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        isEnemy = GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.ENEMY;
        rigidbodyPlayer = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if (!isEnemy) {
            try {
                CommunicationNet.FakeStatic.SendPlayerMovement(transform, rigidbodyPlayer);
            } catch (Exception) {
                Debug.Log("CommunicationNet.FakeStatic.SendPlayerMovement(transform, rigidbodyPlayer); produced an error!");
                throw;
            }
            
        }
    }
}
