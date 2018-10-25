using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyHandler : MonoBehaviour {

    /// <summary>the players current currency</summary>
    private int aktCurrency;

    /// <summary></summary>
    private float currencyTimer;

    /// <summary></summary>
    [SerializeField]
    private float currencyFrequency = 3f;

    /// <summary></summary>
    [SerializeField]
    private int currencyFrequencyAmount = 10;

    /// <summary>The players currency UI text</summary>
    [SerializeField]
    private Text currencyText;

    public int AktCurrency { get; set; }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        currencyTimer += Time.deltaTime;
        
        if(currencyTimer >= currencyFrequency) {
            AktCurrency += currencyFrequencyAmount;
            currencyTimer -= currencyFrequency;
        }

        currencyText.text = "Currency :" + AktCurrency;
	}
}
