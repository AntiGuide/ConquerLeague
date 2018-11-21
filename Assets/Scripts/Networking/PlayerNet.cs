﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The class handles the behaviour of the player object after network packets came in
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerNet : MonoBehaviour {
    [SerializeField] private Image healthImage;

    /// <summary>Marks if the attached object is an enemy</summary>
    private bool isEnemy;

    /// <summary>The rigidbody of this object</summary>
    private Rigidbody rigidbodyPlayer;

    private HitPoints hitPoints;

    public Transform StartPoint { get; set; }

    /// <summary>
    /// Handles new data from the network component
    /// </summary>
    /// <param name="position">The new positon of the player</param>
    /// <param name="quaternion">The new rotation of the player</param>
    /// <param name="velocity">The new velocity of the player</param>
    /// <param name="hp">The new hp of the player</param>
    public void SetNewMovementPack(Vector3 position, Quaternion quaternion, Vector3 velocity, byte hp) {
        transform.position = position;
        transform.rotation = quaternion;
        rigidbodyPlayer.velocity = velocity;
        //hitPoints.AktHp = hp;
    }

    /// <summary>
    /// Handles new data from the network component
    /// </summary>
    /// <param name="position">The new positon of the player</param>
    /// <param name="quaternion">The new rotation of the player</param>
    /// <param name="velocity">The new velocity of the player</param>
    public void SetNewMovementPack(Vector3 position, Quaternion quaternion, Vector3 velocity) {
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
        healthImage.transform.GetChild(0).GetComponent<Image>().enabled = false;
        healthImage.enabled = false;
        transform.position = StartPoint.position;
        transform.rotation = StartPoint.rotation;
        transform.GetChild(0).gameObject.SetActive(false);
        gameObject.layer = 13;
        if (!isEnemy) {
            GetComponent<VehicleController>().enabled = false;
        }

        //gameObject.SetActive(false);
        if (!isEnemy) {
            StartCoroutine(InitCountdown());
        }

        hitPoints.SetFull();
        yield return new WaitForSeconds(5f);
        healthImage.enabled = true;
        healthImage.transform.GetChild(0).GetComponent<Image>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
        gameObject.layer = 0;
        if (!isEnemy) {
            GetComponent<VehicleController>().enabled = true;
        }
    }

    public void DamageTaken(byte damage) {
        if (hitPoints.AktHp - damage <= hitPoints.AktHp && hitPoints.AktHp - damage > 0) {
            hitPoints.AktHp -= damage;
        } else {
            hitPoints.AktHp = 0;
        }
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start() {
        isEnemy = GetComponent<TeamHandler>().TeamID == TeamHandler.TeamState.ENEMY;
        rigidbodyPlayer = GetComponent<Rigidbody>();
        hitPoints = GetComponent<HitPoints>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update() {
        if (!isEnemy) {
            try {
                CommunicationNet.FakeStatic.SendPlayerMovement(transform, rigidbodyPlayer, hitPoints.AktHp);
            } catch (Exception) {
                Debug.Log("CommunicationNet.FakeStatic.SendPlayerMovement(transform, rigidbodyPlayer); produced an error!");
                throw;
            }
            
        }
    }
}
