using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class PuzzleTutorial : MonoBehaviour {

    public GameObject hint;
    public Text hintText;
    public Button hintNextButton;
    public Text hintNextButtonText;

    //json
    public PuzzleTutorialHints myHintWrapper;
    public PuzzleTutorialHint[] myHints;

    public EOTP_PuzzleCreator myPuzzle;
    public int tutStep = -1;

    public void setup(EOTP_PuzzleCreator puzzle, PuzzleTutorialHints hintsWrapper)
    {
        myPuzzle = puzzle;
        tutStep = -1;

        myHintWrapper = hintsWrapper;
        myHints = hintsWrapper.hints;

        if (myHints.Length > 0)
        {
            hint.SetActive(true);
            nextHint();
        }
        else
        {
            hint.SetActive(false);
        }
    }

    public void nextHint()
    {
        tutStep++;
        if (tutStep < myHints.Length - 1)
        {
            //print("NextHint: " + tutStep);
            hintText.text = myHints[tutStep].text;
            hint.GetComponent<RectTransform>().localPosition = new Vector3(myHints[tutStep].x, myHints[tutStep].y, 0);
        }
        else if (tutStep == myHints.Length - 1)
        {
            print("Last tutStep");
            hintText.text = myHints[tutStep].text;
            hint.GetComponent<RectTransform>().localPosition = new Vector3(myHints[tutStep].x, myHints[tutStep].y, 0);
            if (myHintWrapper.exitOnfinish)
            {
                hintNextButtonText.text = "Finish";
            }
            else
            {
                hintNextButton.gameObject.SetActive(false);
            }
        }
        else
        {
            if (myHintWrapper.exitOnfinish)
            {
                print(MainMenu.puzzlePrefKey + myPuzzle.id);
                PlayerPrefs.SetInt(MainMenu.puzzlePrefKey + myPuzzle.id, 1);
                SceneManager.LoadScene("Menu");
            }
            else
            {
                hint.SetActive(false);
            }
        }
    }

    public void closeAll()
    {
        hint.SetActive(false);
    }

    public void closeHint()
    {
        hint.SetActive(false);
    }
}

[System.Serializable]
public class PuzzleTutorialHints
{
    //Wrapper class for json-extraction
    public int puzzleId;
    public bool exitOnfinish;
    public PuzzleTutorialHint[] hints;
}

[System.Serializable]
public class PuzzleTutorialHint
{
    public string text;
    public int x;
    public int y;
}