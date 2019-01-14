﻿using System.Collections;
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

    public void finish()
    {
        SceneManager.LoadScene("Menu");
    }
}