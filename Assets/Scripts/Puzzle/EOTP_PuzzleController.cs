using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EOTP_PuzzleController : MonoBehaviour {

    public GameController GC;
    public GameObject lockPrefab;

    public EOTP_PuzzleCreator myPuzzle;

    public PuzzleObjective PO;
    public PuzzleVerdict PV;
    public PuzzleTutorial PT;

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

    public void setupPuzzle(EOTP_PuzzleCreator puzzle)
    {
        print("EOTP_PuzzleController:SetupPuzzle():!");

        for (int i = 0; i < puzzle.getIO().Length; i++)//For each unit in length
        {
            GameObject prefab = GC.spawnSingle(puzzle.getIO()[i].getTileId(), puzzle.getIO()[i].getTileNr(), puzzle.getIO()[i].getDir());
            prefab.GetComponent<TileController>().locked = true;

            print("EOTP_PuzzleController:SetupPuzzle():spawning i: " + i + "/" + prefab.name);
            if(puzzle.getIO()[i].isInput())
            {
                prefab.GetComponent<TileController>().myLabel = TileController.label.IN;
                inputs.Add(puzzle.getIO()[i]);
                inputObjs.Add(prefab);

                //For each input, add a list 
                realInput.Add(new List<int>());
                actualInput.Add(0);
            }
            else
            {
                prefab.GetComponent<TileController>().myLabel = TileController.label.OUT;
                outputs.Add(puzzle.getIO()[i]);
                outputObjs.Add(prefab);
            }

            GameObject lockP = Instantiate(lockPrefab, prefab.transform);
            lockP.transform.position += new Vector3(-0.35f, 0.35f, 0);
        }

        myPuzzle = puzzle;
        PO.setup(puzzle);
        if(myPuzzle.getID() <= 1)
        {
            PT.setup(puzzle);
        }
    }

    public void puzzlePlay()
    {
        StartCoroutine(puzzlePlayIEnum());
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
            if (outputs[i].getSignal()[EOTPPlayTurn] > 0)
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

        if (EOTPPlayTurn < outputs[0].getSignal().Length)
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
                if (realInput[li][i] != inputs[li].getSignal()[i])
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
            PV.openVerdict(PuzzleVerdict.verdict.WIN, myPuzzle.getName());
        }
        else
        {
            print(this.GetType().Name + ":EOTPwaitFinish(): Try again dumbo!");
            PV.openVerdict(PuzzleVerdict.verdict.LOSS, myPuzzle.getName());
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
