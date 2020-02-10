using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PuzzleVerdict : MonoBehaviour {

    public GameController GC;
    public EOTP_PuzzleController PC;

    public GameObject verdictObject;
    public GameObject solved;
    public GameObject nextPuzzleButton;
    public Text desc;
    public Text tilesUsed;

    public int myPuzzleId = -1;
    public PuzzleArray puzzleArray;

	// Use this for initialization
	void Start () {
        verdictObject.SetActive(false);
    }

    public void openWin(int myPuzzleId, string winDesc)
    {
        this.myPuzzleId = myPuzzleId;

        UtilBools.actionBarLock = true;
        verdictObject.SetActive(true);
        nextPuzzleButton.GetComponent<Button>().interactable = true;
        setTilesUsed();

        desc.color = Color.green;
        desc.text = winDesc;

        int puzzlesDone = 0;
        for (int i = 0; i < 1000; i++)
        {
            if (PlayerPrefs.GetInt(MainMenu.puzzlePrefKey + i, 0) == 1)
            {
                puzzlesDone++;
            }
        }
        //print("New Puzz: " + (myPuzzleId + 1) + "/" + (puzzleArray.EOTP_puzzles.Length));
        //print("PuzzlesDone: " + puzzlesDone + " vs " + puzzleArray.EOTP_puzzles[myPuzzleId + 1].minDone);
        if ((myPuzzleId + 1) >= puzzleArray.EOTP_puzzles.Length || puzzlesDone < puzzleArray.EOTP_puzzles[myPuzzleId + 1].minDone)
        {
            print("shoo shoo");
            nextPuzzleButton.GetComponent<Button>().interactable = false;
        }
    }
    public void openLoss()
    {
        UtilBools.actionBarLock = true;
        verdictObject.SetActive(true);
        setTilesUsed();

        nextPuzzleButton.GetComponent<Button>().interactable = false;
        solved.SetActive(false);

        desc.color = Color.red;
        desc.text = "Something is not quite right... \nTry again!";
    }

    public void setTilesUsed()
    {
        int count = 0;
        foreach(GameObject go in GC.tiles)
        {
            if(go != null)
            {
                count++;
            }
        }
        foreach (GameObject go in PC.inputObjs)
        {
            count--;
        }
        foreach(GameObject go in PC.outputObjs)
        {
            count--;
        }
        tilesUsed.text = "You used " + count + " tiles.";
    }

    public void nextPuzzle()
    {
        GC.audioMixer.playButtonSFX();
        UtilBools.actionBarLock = false;
        print("Going to next puzzle!");
        if(myPuzzleId >= 0)
        {
            if((myPuzzleId + 1) < puzzleArray.EOTP_puzzles.Length)
            {
                print("New Puzz 2: " + (myPuzzleId + 1) + "/" + (puzzleArray.EOTP_puzzles.Length) );
                Game.current = new Game(puzzleArray.EOTP_puzzles[myPuzzleId + 1]);
                SceneManager.LoadScene("Main");
            }
        }
    }

    public void exitMenu()
    {
        GC.audioMixer.playButtonSFX();
        UtilBools.actionBarLock = false;
        SceneManager.LoadScene("Menu");
    }

    public void cancel()
    {
        GC.audioMixer.playButtonSFX();
        solved.SetActive(true);
        nextPuzzleButton.SetActive(true);
        UtilBools.actionBarLock = false;
        verdictObject.SetActive(false);
    }

}
