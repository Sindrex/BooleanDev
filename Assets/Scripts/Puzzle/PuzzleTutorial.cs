using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PuzzleTutorial : MonoBehaviour {

    public GameObject hint;
    public Text hintText;
    public Button hintNextButton;

    public string[] hintTexts;
    public Vector2[] hintSpots;
    public string[] hintTexts2;
    public Vector2[] hintSpots2;

    public EOTP_PuzzleCreator myPuzzle;
    public int tutStep = -1;

    public void setup(EOTP_PuzzleCreator puzzle)
    {
        hint.SetActive(true);
        myPuzzle = puzzle;
        tutStep = -1;
        nextHint();
    }

    public void nextHint()
    {
        if(myPuzzle.id == 0)
        {
            tutStep++;
            print("NextHint: " + tutStep);
            hintText.text = hintTexts[tutStep];
            hint.GetComponent<RectTransform>().localPosition = hintSpots[tutStep];
            if (tutStep >= hintTexts.Length - 1)
            {
                print("Last tutStep");
                hintNextButton.interactable = false;
            }
        }
        else
        {
            tutStep++;
            print("NextHint: " + tutStep);
            hintText.text = hintTexts2[tutStep];
            hint.GetComponent<RectTransform>().localPosition = hintSpots2[tutStep];
            if (tutStep >= hintTexts2.Length - 1)
            {
                print("Last tutStep");
                hintNextButton.interactable = false;
            }
        }
    }

    public void finish()
    {
        if(myPuzzle.id == 0)
        {
            print(MainMenu.puzzlePrefKey + "1");
            PlayerPrefs.SetInt(MainMenu.puzzlePrefKey + "1", 1);
            SceneManager.LoadScene("Menu");
        }
        hint.SetActive(false);
    }
}
