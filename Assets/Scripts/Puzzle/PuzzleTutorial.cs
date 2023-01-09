using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Tilemaps;
using System;

public class PuzzleTutorial : MonoBehaviour
{

    public EOTP_PuzzleController PC;

    public bool isTutorial;

    public GameObject hint;
    public Text hintText;
    public Button hintNextButton;
    public Text hintNextButtonText;

    //json
    public PuzzleTutorialHints myHintWrapper;
    public TutorialAction[] myActions;

    public PuzzleArray puzzleArray;
    public EOTP_PuzzleCreator myPuzzle;
    public int tutStep = -1;

    //arrow
    public GameObject arrow;
    public Sprite[] arrowDirSprites;

    public GameObject placeActionObject;

    public void setup(EOTP_PuzzleCreator puzzle, PuzzleTutorialHints hintsWrapper)
    {
        Debug.Log("Setting up tutorial! Hints: " + (hintsWrapper.actions != null ? "true" : "false"));
        Debug.Log("ToString:" + hintsWrapper.ToString());
        int displayHints = PlayerPrefs.GetInt(OptionController.genericsOK[6]);
        if (displayHints == 0) //do not display hints
        {
            hint.SetActive(false);
            return;
        }

        myPuzzle = puzzle;
        tutStep = -1;

        myHintWrapper = hintsWrapper;
        myActions = hintsWrapper.actions;

        placeActionObject.SetActive(false);

        if (myActions.Length > 0)
        {
            hint.SetActive(true);
            nextHint();
        }
        else
        {
            hint.SetActive(false);
        }

        isTutorial = true;
    }

    public void nextHint()
    {
        placeActionObject.SetActive(false);

        tutStep++; //starts at -1
        if(tutStep != 0) //not first
        {
            PC.GC.audioMixer.playButtonSFX();
        }
        if (tutStep < myActions.Length - 1) //not last
        {
            //print("NextHint: " + tutStep);
            //print("Hint: " + (myActions[tutStep].hint != null ? "true" : "false"));
            //print("Arrow: " + (myActions[tutStep].arrow != null ? "true" : "false"));
            hintText.text = myActions[tutStep].hint.text;
            hint.GetComponent<RectTransform>().localPosition = new Vector3(myActions[tutStep].hint.x, myActions[tutStep].hint.y, 0);

            setArrow();

            hintNextButton.gameObject.SetActive(false);
            TutorialAction currentAction = myActions[tutStep];
            TutorialAction.ActionType type = currentAction.getActionType();
            if (type == TutorialAction.ActionType.Text)
            {
                hintNextButton.gameObject.SetActive(true);
            }
            else if(type == TutorialAction.ActionType.Place)
            {
                placeActionObject.SetActive(true);
                placeActionObject.transform.position = PC.GC.floorList[currentAction.spotIndex].transform.position + new Vector3(0,0,5);
            }
        }
        else if (tutStep == myActions.Length - 1) //last
        {
            print("Last tutStep");
            hintText.text = myActions[tutStep].hint.text;
            hint.GetComponent<RectTransform>().localPosition = new Vector3(myActions[tutStep].hint.x, myActions[tutStep].hint.y, 0);

            setArrow();

            if (myHintWrapper.exitOnfinish)
            {
                hintNextButtonText.text = "Next Puzzle";
            }
            else
            {
                hintNextButton.gameObject.SetActive(false);
            }
        }
        else //over last (exitOnfinish)
        {
            //tutorial puzzle
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

    private void setArrow() 
    {
        //Arrow
        if (myActions[tutStep].arrow.enable)
        {
            arrow.SetActive(true);
            arrow.transform.localPosition = new Vector3(myActions[tutStep].arrow.x - myActions[tutStep].hint.x, myActions[tutStep].arrow.y - myActions[tutStep].hint.y, 0);
            arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            arrow.transform.Rotate(new Vector3(0, 0, myActions[tutStep].arrow.rotz));

            arrow.GetComponent<Image>().sprite = arrowDirSprites[0];
            if (myActions[tutStep].arrow.rotz >= 45 && myActions[tutStep].arrow.rotz < 135)
            {
                arrow.GetComponent<Image>().sprite = arrowDirSprites[3];
                arrow.transform.Rotate(new Vector3(0, 0, -90));
            }
            else if (myActions[tutStep].arrow.rotz >= 135 && myActions[tutStep].arrow.rotz <= 225)
            {
                arrow.GetComponent<Image>().sprite = arrowDirSprites[2];
                arrow.transform.Rotate(new Vector3(0, 0, -180));
            }
            else if (myActions[tutStep].arrow.rotz >= -135 && myActions[tutStep].arrow.rotz <= -45)
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

    public void closeAll()
    {
        hint.SetActive(false);
    }

    public void closeHint()
    {
        hint.SetActive(false);
        PC.GC.audioMixer.playButtonSFX();
    }

    public void continueTutorial()
    {
        nextHint();

        //TutorialAction currentAction = myActions[tutStep];
        //TutorialAction.ActionType type = currentAction.getActionType();
        //if(type == TutorialAction.ActionType.Text)
        //{
        //    nextHint();
        //}
    }

    public void PlaceAction(int spotIndex, int tileID)
    {
        //print("spotIndex: " + spotIndex + ", tile: " + tileID);
        if (PC.isPuzzle && isTutorial)
        {
            TutorialAction currentAction = myActions[tutStep];
            if (currentAction.getActionType() == TutorialAction.ActionType.Place)
            {
                if (currentAction.spotIndex == spotIndex && currentAction.targetID == tileID)
                {
                    //nextHint();
                    hintNextButton.gameObject.SetActive(true);
                }
            }
        }
    }

    public void ButtonAction(int targetID)
    {
        if (PC.isPuzzle && isTutorial)
        {
            TutorialAction currentAction = myActions[tutStep];
            if (currentAction.getActionType() == TutorialAction.ActionType.Button)
            {
                if (currentAction.targetID == targetID)
                {
                    hintNextButton.gameObject.SetActive(true);
                }
            }
        }
    }

    public void ControlAction(int targetID)
    {
        //if (PC.isPuzzle && isTutorial) //not necessary, does this on update
        {
            TutorialAction currentAction = myActions[tutStep];
            if (currentAction.getActionType() == TutorialAction.ActionType.Control)
            {
                if (currentAction.targetID == targetID)
                {
                    hintNextButton.gameObject.SetActive(true);
                }
            }
        }
    }

    private void Update()
    {
        if (PC.isPuzzle && isTutorial)
        {
            if (InputController.getInput(InputPurpose.CAM_MOVEMENT_HORIZ) || InputController.getInput(InputPurpose.CAM_MOVEMENT_VERT) || InputController.getInput(InputPurpose.CAM_MOVEMENT_DRAG))
            {
                ControlAction((int)TutorialAction.ControlIDs.WASD);
            }
            else if (InputController.getInput(InputPurpose.ZOOM_IN) || InputController.getInput(InputPurpose.ZOOM_OUT))
            {
                ControlAction((int)TutorialAction.ControlIDs.ZOOM);
            }
            else if (InputController.getInput(InputPurpose.ACTIONBAR_LEFT) || InputController.getInput(InputPurpose.ACTIONBAR_RIGHT))
            {
                ControlAction((int)TutorialAction.ControlIDs.SCROLL);
            }
            else if (InputController.getInput(InputPurpose.TILE_ROTATE_LEFT) || InputController.getInput(InputPurpose.TILE_ROTATE_RIGHT))
            {
                ControlAction((int)TutorialAction.ControlIDs.ROTATE);
            }
            else if (InputController.getInput(InputPurpose.SELECTIONBAR))
            {
                ControlAction((int)TutorialAction.ControlIDs.SPACE);
            }
            else if (InputController.getInput(InputPurpose.DELETE_TILE))
            {
                ControlAction((int)TutorialAction.ControlIDs.DELETE);
            }
            else if (InputController.getInput(InputPurpose.UNDO))
            {
                ControlAction((int)TutorialAction.ControlIDs.UNDO);
            }
        }
    }
}

[System.Serializable]
public class PuzzleTutorialHints
{
    //Wrapper class for json-extraction
    public int puzzleId;
    public bool exitOnfinish;
    public TutorialAction[] actions;
    public override string ToString()
    {
        return "PuzzleID: " + puzzleId + ", exitOnfinish: " + exitOnfinish + ", hints: ";
    }
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