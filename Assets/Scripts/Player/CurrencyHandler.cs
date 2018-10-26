using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyHandler : MonoBehaviour {
    /// <summary>The players currency UI text</summary>
    [SerializeField]
    private Text currencyText;

    /// <summary>the players current currency</summary>
    private int aktCurrency;

    public int AktCurrency { get; set; }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start () {
        
	}

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update () {
        currencyText.text = "Currency :" + AktCurrency;
	}
}