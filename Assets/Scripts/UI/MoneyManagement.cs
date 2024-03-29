﻿using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Manages the money of the player</summary>
public class MoneyManagement : MonoBehaviour {
    /// <summary>The time the money takes to lerp to a specific value</summary>
    public float LerpTimeStart = 1.0f;

    /// <summary>Saves the amout of money the player has at the moment</summary>
    private static long money;

    /// <summary>How much seconds of the lerp are completed</summary>
    private float lerpTimeDone = 0.0f;

    /// <summary>The amount of money that the player lerps from</summary>
    private long moneyAmountOld;

    /// <summary>The amount of money that is shown at the moment</summary>
    private long moneyAmountShown;

    /// <summary>The amount of money that is goal of the lerp</summary>
    private long moneyToLerpTo;

    /// <summary>Indicates if the money display is finished lerping</summary>
    private bool isFinished = true;

    /// <summary>The own audio source for money lerping</summary>
    private AudioSource audioSource;

    /// <summary>
    /// Formats the money to german format
    /// </summary>
    /// <param name="money">The amount of money</param>
    /// <returns>Formatted money as a string</returns>
    public static string FormatMoney(long money) {
        var cultureInfo = new CultureInfo("de-DE", false) { NumberFormat = { CurrencySymbol = string.Empty } };
        return money.ToString("C0", cultureInfo).TrimEnd();
    }

    /// <summary>
    /// Checks money without subbing it.
    /// </summary>
    /// <param name="moneyToCheck">How much should be checked</param>
    /// <returns>Returns if player has at least this amount of money</returns>
    public static bool HasMoney(long moneyToCheck) {
        return money >= moneyToCheck;
    }

    /// <summary>
    /// Adds money
    /// </summary>
    /// <param name="moneyToAdd">The amount to add</param>
    public void AddMoney(long moneyToAdd) {
        money = money + moneyToAdd;
        OutputMoney(money, true);
    }

    /// <summary>
    /// Subs money
    /// </summary>
    /// <param name="moneyToSub">The amount to sub</param>
    /// <returns>If the player had enough money for the transaction. True is returned and the transaction is performed. (False --> no transaction)</returns>
    public bool SubMoney(long moneyToSub) {
        if (money < moneyToSub) {
            return false;
        }

        money = money - moneyToSub;
        OutputMoney(money, true);
        return true;
    }

    /// <summary>
    /// Sets the money to a specific amount
    /// </summary>
    /// <param name="valueToSet">To how much money should the players account be set to</param>
    private void SetMoney(long valueToSet) {
        if (valueToSet >= 0) {
            money = valueToSet;
            OutputMoney(money, false);
        } else {
            throw new ArgumentException("Can not set a negative Dollar Value", "valueToSet");
        }
    }

    /// <summary>Sets the money to 600 in the beginning</summary>
    private void Start() {
        var initValue = 30;
        SetMoney(initValue);
    }

    /// <summary>Called once per frame</summary>
    private void Update() {
        if (moneyToLerpTo != moneyAmountShown) {
            lerpTimeDone += Time.deltaTime;
            lerpTimeDone = Mathf.Min(LerpTimeStart, lerpTimeDone);
            moneyAmountShown = (long)Mathf.Lerp(moneyAmountOld, moneyToLerpTo, lerpTimeDone / LerpTimeStart);
            GetComponent<Text>().text = FormatMoney(moneyAmountShown);
        } else if (!isFinished) {
            isFinished = true;
        }
    }

    /// <summary>
    /// Display the money
    /// </summary>
    /// <param name="money">How much money to display</param>
    /// <param name="lerpEffect">Whether the change should be applied instantly or with a lerp effect</param>
    private void OutputMoney(long money, bool lerpEffect) {
        if (lerpEffect) {
            moneyAmountOld = moneyAmountShown;
            moneyToLerpTo = money;
            lerpTimeDone = 0.0f;
            isFinished = false;
        } else {
            GetComponent<Text>().text = FormatMoney(money);
            moneyAmountShown = money;
            moneyToLerpTo = money;
        }
    }
}