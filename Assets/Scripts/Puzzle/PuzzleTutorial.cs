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

    public PuzzleArray puzzleArray;
    public EOTP_PuzzleCreator myPuzzle;
    public int tutStep = -1;

    //arrow
    public GameObject arrow;
    public Sprite[] arrowDirSprites;

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

            //Arrow
            if(myHints[tutStep].ax != 0 || myHints[tutStep].ay != 0)
            {
                arrow.SetActive(true);
                arrow.transform.localPosition = new Vector3(myHints[tutStep].ax - myHints[tutStep].x, myHints[tutStep].ay - myHints[tutStep].y, 0);
                arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                arrow.transform.Rotate(new Vector3(0, 0, myHints[tutStep].rotz));
                arrow.GetComponent<Image>().sprite = arrowDirSprites[0];
                if (myHints[tutStep].rotz >= 45 && myHints[tutStep].rotz < 135)
                {
                    arrow.GetComponent<Image>().sprite = arrowDirSprites[3];
                    arrow.transform.Rotate(new Vector3(0, 0, -90));
                }
                else if(myHints[tutStep].rotz >= 135 && myHints[tutStep].rotz <= 225)
                {
                    arrow.GetComponent<Image>().sprite = arrowDirSprites[2];
                    arrow.transform.Rotate(new Vector3(0, 0, -180));
                }
                else if(myHints[tutStep].rotz >= -135 && myHints[tutStep].rotz <= -45)
                {
                    arrow.GetComponent<Image>().sprite = arrowDirSprites[2];
                    arrow.transform.Rotate(new Vector3(0, 0, 90));
                }
            }
            else
            {
                arrow.SetActive(false);
            }
        }
        else if (tutStep == myHints.Length - 1)
        {
            print("Last tutStep");
            hintText.text = myHints[tutStep].text;
            hint.GetComponent<RectTransform>().localPosition = new Vector3(myHints[tutStep].x, myHints[tutStep].y, 0);
            //Arrow
            if (myHints[tutStep].ax != 0 || myHints[tutStep].ay != 0)
            {
                arrow.SetActive(true);
                arrow.transform.localPosition = new Vector3(myHints[tutStep].ax - myHints[tutStep].x, myHints[tutStep].ay - myHints[tutStep].y, 0);
                arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                arrow.transform.Rotate(new Vector3(0, 0, myHints[tutStep].rotz));
                arrow.GetComponent<Image>().sprite = arrowDirSprites[0];
                if (myHints[tutStep].rotz >= 45 && myHints[tutStep].rotz < 135)
                {
                    arrow.GetComponent<Image>().sprite = arrowDirSprites[3];
                    arrow.transform.Rotate(new Vector3(0, 0, -90));
                }
                else if (myHints[tutStep].rotz >= 135 && myHints[tutStep].rotz <= 225)
                {
                    arrow.GetComponent<Image>().sprite = arrowDirSprites[2];
                    arrow.transform.Rotate(new Vector3(0, 0, -180));
                }
                else if (myHints[tutStep].rotz >= -135 && myHints[tutStep].rotz <= -45)
                {
                    arrow.GetComponent<Image>().sprite = arrowDirSprites[2];
                    arrow.transform.Rotate(new Vector3(0, 0, 90));
                }
            }
            else
            {
                arrow.SetActive(false);
            }
            if (myHintWrapper.exitOnfinish)
            {
                hintNextButtonText.text = "Next Puzzle";
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
                //SceneManager.LoadScene("Menu");
                UtilBools.actionBarLock = false;
                print("Going to next puzzle!");
                int myPuzzleId = myPuzzle.id;
                if (myPuzzleId >= 0)
                {
                    if ((myPuzzleId + 1) < puzzleArray.EOTP_puzzles.Length)
                    {
                        print("New Puzz: " + (myPuzzleId + 1) + "/" + (puzzleArray.EOTP_puzzles.Length));
                        Game.current = new Game(puzzleArray.EOTP_puzzles[myPuzzleId + 1]);
                        SceneManager.LoadScene("Main");
                    }
                }
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
    public int ax;
    public int ay;
    public int rotz;
}