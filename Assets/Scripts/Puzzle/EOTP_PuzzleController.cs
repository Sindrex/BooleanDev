using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EOTP_PuzzleController : MonoBehaviour {

    public GameController GC;
    public GameObject lockPrefab;
    
    public EOTP_PuzzleCreator myPuzzle = null;
    public bool isPuzzle = false;

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
    public GameObject puzzleSaveSlots;

    //Info
    public GameObject puzzleInfo;
    public GameObject infoButton;
    public Text infoText;

    //logic
    public float EOTP_WAIT_TIME = 0.5f;
    public float DEFAULT_MIDDLE_WAIT_TIME = 0.5f;
    private float EOTP_MIDDLE_WAIT_TIME_MS = 0.5f;
    private int EOTPPlayTurn = 0;
    private bool EOTP_waitforinput = false;
    [SerializeField]
    private List<int> actualInput = new List<int>();
    public List<EOTP_IOCreator> inputs = new List<EOTP_IOCreator>();
    public List<GameObject> inputObjs = new List<GameObject>();
    public List<EOTP_IOCreator> outputs = new List<EOTP_IOCreator>();
    public List<GameObject> outputObjs = new List<GameObject>();
    private List<List<int>> realInput = new List<List<int>>();

    //stepping
    public bool stepping = false;
    public GameObject stepButton;
    public Text stepText;
    public GameObject stopStepButton;

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
        stepText.gameObject.SetActive(false);
        stepButton.SetActive(false);
        stopStepButton.SetActive(false);
        puzzleSaveSlots.SetActive(false);
    }

    public void setupPuzzle(EOTP_PuzzleCreator puzzle)
    {
        print("EOTP_PuzzleController:SetupPuzzle():!");
        isPuzzle = true;

        //spawn tiles
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

        //Info
        if(myPuzzle.info != null)
        {
            infoButton.SetActive(!myPuzzle.info.Trim().Equals(""));
            infoText.text = myPuzzle.info;
        }
        else
        {
            infoButton.SetActive(false);
        }
        puzzleInfo.SetActive(false);

        //Step
        stepText.gameObject.SetActive(false);
        stopStepButton.SetActive(false);

        //Tutorial
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

        //middleWaitTime
        print("Puzzle Middle Time: " + puzzle.middleWaitTime);
        if(puzzle.middleWaitTime > 0)
        {
            EOTP_MIDDLE_WAIT_TIME_MS = puzzle.middleWaitTime;
        }
        else
        {
            EOTP_MIDDLE_WAIT_TIME_MS = DEFAULT_MIDDLE_WAIT_TIME;
        }

        //show objective at start
        PO.openObjective();
    }

    public void toggleInfo()
    {
        puzzleInfo.SetActive(!puzzleInfo.activeSelf);
    }

    public void autoPlay()
    {
        resetPuzzle();
        print(this.GetType().Name + ": Starting autoplay puzzle!");
        UtilBools.puzzleInteractLock(true);
        puzzlePlay(false);
        GC.audioMixer.playButtonSFX();
    }

    public void puzzlePlay(bool stepping)
    {
        this.stepping = stepping;
        if(myPuzzle != null && myPuzzle.id >= 0)
        {
            stepText.gameObject.SetActive(true);
            stepText.text = EOTPPlayTurn + "/" + (outputs[0].signal.Length + 1);
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
            POutputController curOutObj = outputObjs[i].GetComponent<POutputController>();
            if (outputs[i].signal[EOTPPlayTurn] > 0)
            {
                curOutObj.beingPowered = true;
                curOutObj.tryPower(true);
                curOutObj.sendPower(-1);
                //on = true;
            }
            else
            {
                curOutObj.beingPowered = false;
                curOutObj.tryPower(false);
                curOutObj.sendPower(-1);
            }
            //print(this.GetType().Name + ":puzzlePlay():" + outputs[i].getSignal()[EOTPPlayTurn]);
        }

        print("Waiting for middle: " + EOTP_MIDDLE_WAIT_TIME_MS + "sec!");
        yield return new WaitForSeconds(EOTP_MIDDLE_WAIT_TIME_MS);

        EOTP_waitforinput = true;
        EOTPPlayTurn++;
        print(this.GetType().Name + ": puzzlePlay(): PlayTurn: " + EOTPPlayTurn);

        stepText.text = EOTPPlayTurn + "/" + (outputs[0].signal.Length + 1);

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
        //print("Samling puzzle results...");
        yield return new WaitForSeconds(EOTP_WAIT_TIME);
        EOTP_waitforinput = false;
        if (!stepping)
        {
            puzzlePlay(false);
        }
    }

    private IEnumerator EOTPwaitFinish()
    {
        print("EOTPwaitFinish");
        yield return new WaitForSeconds(EOTP_WAIT_TIME);
        EOTP_waitforinput = false;
        addActualToReal();

        if (!stepping)
        {
            checkResult();
        }
    }

    private void checkResult()
    {
        bool ok = resetPuzzle();
        if (ok)
        {
            print(this.GetType().Name + ":EOTPwaitFinish(): All inputs OK. Good job!");
            PlayerPrefs.SetInt(MainMenu.puzzlePrefKey + myPuzzle.id, 1);
            print("Sat playerprefs for: " + MainMenu.puzzlePrefKey + myPuzzle.id);
            GC.SC.save();
            PV.openWin(myPuzzle.id, myPuzzle.winDesc);
        }
        else
        {
            print(this.GetType().Name + ":EOTPwaitFinish(): Try again dumbo!");
            PV.openLoss();
        }
    }

    public bool resetPuzzle()
    {
        print(this.GetType().Name + ": reseting puzzle");
        UtilBools.puzzleInteractLock(false);
        EOTPPlayTurn = 0; //reset
        bool ok = true;
        for (int li = 0; li < realInput.Count; li++)
        {
            for (int i = 0; i < realInput[li].Count; i++)
            {
                print(this.GetType().Name + ":EOTPwaitFinish():li/i: " + li + "/" + i);
                print(this.GetType().Name + ":EOTPwaitFinish():Input Gotten:" + realInput[li][i]);
                print(this.GetType().Name + ":EOTPwaitFinish():Input Wanted:" + inputs[li].signal[i]);
                if (realInput[li][i] != inputs[li].signal[i])
                {
                    ok = false;
                }
            }
            //reset each list for next check logic
            realInput[li] = new List<int>();
        }
        stepping = false;
        stepText.gameObject.SetActive(false);
        stopStepButton.SetActive(false);
        //reset powered
        for (int i = 0; i < outputs.Count; i++)
        {
            POutputController curOutObj = outputObjs[i].GetComponent<POutputController>();
            curOutObj.beingPowered = false;
            curOutObj.tryPower(false);
            curOutObj.sendPower(-1);
        }
        return ok;
    }

    private void Update()
    {
        if (EOTP_waitforinput)
        {
            for(int i = 0; i < inputObjs.Count; i++)
            {
                if (inputObjs[i].GetComponent<TileController>().beingPowered)
                {
                    //print(Time.time + ": i= " + i + ": acutalInput=1");
                    actualInput[i] = 1;
                }
            }
        }
    }

    //stepping logic
    public void stepPlay()
    {
        stopStepButton.SetActive(true);
        stepping = true;
        UtilBools.puzzleInteractLock(true);
        if (EOTPPlayTurn < outputs[0].signal.Length)
        {
            puzzlePlay(true);
        }
        else
        {
            checkResult();
            stopStepButton.SetActive(false);
            //print(this.GetType().Name + ":puzzlePlay(): finished");
        }
        GC.audioMixer.playButtonSFX();
    }

    public void stopStep()
    {
        resetPuzzle();
        stopStepButton.SetActive(false);
        GC.audioMixer.playButtonSFX();
    }
}
