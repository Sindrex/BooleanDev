﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EOTP_PuzzleController : MonoBehaviour {

    public GameController GC;
    public GameObject lockPrefab;

    public EOTP_PuzzleCreator myPuzzle = null;

    public PuzzleObjective PO;
    public PuzzleVerdict PV;
    public PuzzleTutorial PT;
    public PuzzleTruthTable PTT;

    //Gameobjects
    public GameObject puzzleBG;
    public GameObject puzzleObjectiveButton;
    public GameObject puzzlePlayButton;
    public GameObject puzzleTruthButton;
    public GameObject puzzleHint;

    //Info
    public GameObject puzzleInfo;
    public GameObject infoButton;
    public Text infoText;

    public float EOTP_WAIT_TIME = 1;
    public float EOTP_MIDDLE_WAIT_TIME_MS = 1;
    private int EOTPPlayTurn = 0;
    private bool EOTP_waitforinput = false;
    [SerializeField]
    private List<int> actualInput = new List<int>();
    public List<EOTP_IOCreator> inputs = new List<EOTP_IOCreator>();
    public List<GameObject> inputObjs = new List<GameObject>();
    public List<EOTP_IOCreator> outputs = new List<EOTP_IOCreator>();
    public List<GameObject> outputObjs = new List<GameObject>();
    private List<List<int>> realInput = new List<List<int>>();

    public void close()
    {
        puzzleBG.SetActive(false);
        puzzleObjectiveButton.SetActive(false);
        puzzlePlayButton.SetActive(false);
        puzzleTruthButton.SetActive(false);
        puzzleHint.SetActive(false);
        PT.closeAll();
        puzzleInfo.SetActive(false);
        infoButton.SetActive(false);
    }

    public void setupPuzzle(EOTP_PuzzleCreator puzzle)
    {
        print("EOTP_PuzzleController:SetupPuzzle():!");

        int inputIndex = 0, outputIndex = 0;
        for (int i = 0; i < puzzle.IO.Length; i++)//For each unit in length
        {
            GameObject prefab = GC.spawnSingle(puzzle.IO[i].tileId, puzzle.IO[i].getSpotIndex(puzzle.length), puzzle.IO[i].dir);
            prefab.GetComponent<TileController>().locked = true;

            print("EOTP_PuzzleController:SetupPuzzle():spawning i: " + i + "/" + prefab.name);
            if(puzzle.IO[i].input)
            {
                prefab.GetComponent<TileController>().myLabel = TileController.label.IN;
                prefab.GetComponent<PInputController>().setSprite(inputIndex);
                inputIndex++;
                inputs.Add(puzzle.IO[i]);
                inputObjs.Add(prefab);

                //For each input, add a list 
                realInput.Add(new List<int>());
                actualInput.Add(0);
            }
            else
            {
                prefab.GetComponent<TileController>().myLabel = TileController.label.OUT;
                prefab.GetComponent<POutputController>().setSprite(outputIndex);
                outputIndex++;
                outputs.Add(puzzle.IO[i]);
                outputObjs.Add(prefab);
            }

            GameObject lockP = Instantiate(lockPrefab, prefab.transform);
            lockP.transform.position += new Vector3(-0.35f, 0.35f, 0);
        }

        myPuzzle = puzzle;
        PO.setup(puzzle);
        PTT.setup(puzzle);

        infoButton.SetActive(!myPuzzle.info.Trim().Equals(""));
        infoText.text = myPuzzle.info;
        puzzleInfo.SetActive(false);

        List<PuzzleTutorialHints> temp = ResourceLoader.loadJsonFolder<PuzzleTutorialHints>("/Tutorial/");
        PuzzleTutorialHints myHintWrapper = null;
        foreach (PuzzleTutorialHints hints in temp)
        {
            if (hints.puzzleId == myPuzzle.id)
            {
                myHintWrapper = hints;
            }
        }
        if (myHintWrapper != null) //Spawn tut on all that have tut from json
        {
            PT.setup(puzzle, myHintWrapper);
        }
        else
        {
            PT.closeAll();
        }
    }

    public void toggleInfo()
    {
        puzzleInfo.SetActive(!puzzleInfo.activeSelf);
    }

    public void puzzlePlay()
    {
        if(myPuzzle != null && myPuzzle.id >= 0)
        {
            StartCoroutine(puzzlePlayIEnum());
        }
        else
        {
            print("No puzzle!");
        }
    }

    private IEnumerator puzzlePlayIEnum()
    {
        addActualToReal(); //Adds from previous reading

        for (int i = 0; i < actualInput.Count; i++) //Reset all inputs
        {
            actualInput[i] = 0;
            //print(this.GetType().Name + ":puzzlePlay():i/actualInput: " + i + "/"+ actualInput[i]);
            //print(Time.time + ": acutalInput=0");
        }
        //bool on = false;
        for (int i = 0; i < outputs.Count; i++)
        {
            if (outputs[i].signal[EOTPPlayTurn] > 0)
            {
                outputObjs[i].GetComponent<TileController>().beingPowered = true;
                //on = true;
            }
            else
            {
                outputObjs[i].GetComponent<TileController>().beingPowered = false;
            }
            //print(this.GetType().Name + ":puzzlePlay():" + outputs[i].getSignal()[EOTPPlayTurn]);
        }

        print("Waiting for " + EOTP_MIDDLE_WAIT_TIME_MS + "sec!");
        yield return new WaitForSeconds(EOTP_MIDDLE_WAIT_TIME_MS);

        EOTP_waitforinput = true;
        EOTPPlayTurn++;
        print(this.GetType().Name + "puzzlePlay(): PlayTurn: " + EOTPPlayTurn);

        if (EOTPPlayTurn < outputs[0].signal.Length)
        {
            StartCoroutine(EOTPwait());
        }
        else
        {
            StartCoroutine(EOTPwaitFinish());
            //print(this.GetType().Name + ":puzzlePlay(): finished");
        }
    }
    
    private void addActualToReal()
    {
        if (EOTPPlayTurn > 0) //skip first turn
        {
            //Add to real-list for storage
            for (int li = 0; li < realInput.Count; li++)
            {
                realInput[li].Add(actualInput[li]);
                print(this.GetType().Name + ":addActualToReal():i/actualInput: " + li + "/" + actualInput[li]);
            }
        }
    }

    private IEnumerator EOTPwait()
    {
        yield return new WaitForSeconds(EOTP_WAIT_TIME);
        EOTP_waitforinput = false;
        puzzlePlay();
    }

    private IEnumerator EOTPwaitFinish()
    {
        yield return new WaitForSeconds(EOTP_WAIT_TIME);
        EOTP_waitforinput = false;
        addActualToReal();
        EOTPPlayTurn = 0; //reset

        bool ok = true;
        for (int li = 0; li < realInput.Count; li++)
        {
            for (int i = 0; i < realInput[li].Count; i++)
            {
                //print(this.GetType().Name + ":EOTPwaitFinish():li/i: " + li + "/" + i);
                print(this.GetType().Name + ":EOTPwaitFinish():Input Gotten:" + realInput[li][i]);
                //print(this.GetType().Name + ":EOTPwaitFinish():Input Wanted:" + inputs[li].getSignal()[i]);
                if (realInput[li][i] != inputs[li].signal[i])
                {
                    ok = false;
                }
            }
            //reset each list
            realInput[li] = new List<int>();
        }
        if (ok)
        {
            print(this.GetType().Name + ":EOTPwaitFinish(): All inputs OK. Good job!");
            PlayerPrefs.SetInt(MainMenu.puzzlePrefKey + myPuzzle.id, 1);
            print("Sat playerprefs for: " + MainMenu.puzzlePrefKey + myPuzzle.id);
            PV.openWin(myPuzzle.id, myPuzzle.winDesc);
        }
        else
        {
            print(this.GetType().Name + ":EOTPwaitFinish(): Try again dumbo!");
            PV.openLoss();
        }

    }

    private void Update()
    {
        if (EOTP_waitforinput)
        {
            for(int i = 0; i < inputObjs.Count; i++)
            {
                if (inputObjs[i].GetComponent<TileController>().beingPowered)
                {
                    actualInput[i] = 1;
                    //print(Time.time + ": acutalInput=1");
                }
            }
        }
    }
}
