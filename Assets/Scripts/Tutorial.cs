using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {
    [SerializeField]
    private GameObject tutorialButton;

    [SerializeField]
    private Image tutorialImage;

    [SerializeField]
    private Sprite[] tutorialSprites;

    private int currSprite = 0;

	// Use this for initialization
	void Start () {
        tutorialImage.sprite = tutorialSprites[currSprite];
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void OnClickNext() {
        if(currSprite >= tutorialSprites.Length-1) {
            currSprite = 0;
            tutorialButton.SetActive(true);
            gameObject.SetActive(false);
        } else {
            ++currSprite;
        }
        //currSprite = currSprite >= tutorialSprites.Length-1 ? 0 : ++currSprite;
        tutorialImage.sprite = tutorialSprites[currSprite];
    }

    public void OnClickBack() {
        currSprite = currSprite <= 0 ? tutorialSprites.Length-1 : --currSprite;
        tutorialImage.sprite = tutorialSprites[currSprite];
    }

    public void OnClickReturn() {
        gameObject.SetActive(false);
    }
}
