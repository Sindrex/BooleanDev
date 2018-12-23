using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class InputBasedWire : TileController {

    //Father for all input-based wires (generic, notGate, hopWire)

    public List<int> currentInputs = new List<int>();
    public List<int> possibleInputs = new List<int>();
    public GameObject[] neighbours = new GameObject[4];

    // Update is called once per frame
    protected virtual void Update()
    {
        if (placed)
        {
            getInput();
            checkPower();
            tryPower(beingPowered);
        }
        else
        {
            tryPower(false);
            currentInputs = new List<int>();
        }

        if(homeObj != null)
        {
            spotIndex = homeObj.GetComponent<FloorTileController>().spotIndex;
        }
    }

    protected virtual void checkPower()
    {
        beingPowered = false;

        //All 4 dirs
        for(int i = 0; i < 4; i++)
        {
            if (currentInputs.Contains(i))
            {
                //print("powered!");
                beingPowered = true;
            }
        }
    }

    override
    protected void tryPower(bool state)
    {
        if (state)
        {
            //print("beingPowered!");
            this.GetComponent<SpriteRenderer>().sprite = spriteOn;

            List<int> okList = new List<int>();
            List<int> notOkList = new List<int>();

            for (int j = 0; j < currentInputs.Count; j++)
            {
                int inDIR = currentInputs[j];
                for (int i = 0; i < possibleOutputs.Count; i++)
                {
                    int outDIR = possibleOutputs[i];
                    if (outDIR >= 0 && outDIR < output.Length && outDIR != inDIR)
                    {
                        //print("OutDir: " + outDIR + ", DIR " + inDIR);
                        output[outDIR] = 1;
                        okList.Add(outDIR);
                    }
                    else
                    {
                        notOkList.Add(outDIR);
                    }
                }
            }
            
            foreach(int DIR in notOkList)
            {
                if(ID == 3 || ID == 4 || ID == 12 || ID == 13)
                {
                    //output[DIR] = 0; //makes race-conditions

                    //Makes power loops
                    if (!okList.Contains(DIR))
                    {
                        output[DIR] = 0;
                    }
                }
                else
                {
                    if (!okList.Contains(DIR))
                    {
                        output[DIR] = 0;
                    }
                }
            }
        }
        else //not being powered
        {
            //print("Off!");
            this.GetComponent<SpriteRenderer>().sprite = spriteOff;

            //de-power outputs
            for (int i = 0; i < possibleOutputs.Count; i++)
            {
                int DIR = possibleOutputs[i];
                if (DIR >= 0 && DIR < output.Length)
                {
                    output[DIR] = 0;
                }
            }
        }
    }

    protected void getInput()
    {
        neighbours = getNeighbours();

        for(int i = 0; i < neighbours.Length; i++)
        {
            int negDir = negDirection(i);

            if (neighbours[i] != null)
            {
                if (neighbours[i].GetComponent<TileController>().hasOutput(negDir) && !currentInputs.Contains(i))
                {
                    currentInputs.Add(i);
                }
                else if(!neighbours[i].GetComponent<TileController>().hasOutput(negDir))
                {
                    currentInputs.Remove(i);
                }
            }
            else
            {
                if (currentInputs.Contains(i))
                {
                    currentInputs.Remove(i);
                }
            }
        }
    }

    protected GameObject[] getNeighbours()
    {
        GameObject[] tempList = new GameObject[4];

        if (spotIndex - 1 >= 0)//Check if there is a spot to the left
        {
            if (possibleInputs.Contains(LEFT)) //If this tile can input from left
            {
                if (GC.tiles[spotIndex - 1] != null)
                {
                    tempList[1] = GC.tiles[spotIndex - 1];
                }
            }
        }
        //right
        if (spotIndex + 1 < GC.tiles.Length)
        {
            if (possibleInputs.Contains(RIGHT))
            {
                if (GC.tiles[spotIndex + 1] != null)
                {
                    tempList[3] = GC.tiles[spotIndex + 1];
                }
            }
        }
        //up
        if (spotIndex - GC.length >= 0)
        {
            if (possibleInputs.Contains(UP))
            {
                if (GC.tiles[spotIndex - GC.length] != null)
                {
                    tempList[0] = GC.tiles[spotIndex - GC.length];
                }
            }
        }
        //down
        if (spotIndex + GC.length < GC.tiles.Length)
        {
            if (possibleInputs.Contains(DOWN))
            {
                if (GC.tiles[spotIndex + GC.length] != null)
                {
                    tempList[2] = GC.tiles[spotIndex + GC.length];
                }
            }
        }
        //print(this.gameObject + " got a neighbour-list");
        return tempList;
    }

    override
    protected void rotateInput()
    {
        //Update i/o
        for (int i = 0; i < possibleInputs.Count; i++)
        {
            possibleInputs[i] += 1;
            if (possibleInputs[i] == 4)
            {
                possibleInputs[i] = 0;
            }
            if (possibleInputs[i] > 4)
            {
                possibleInputs[i] = 1;
            }
        }
    }

    override
    public bool hasOutput(int DIR)
    {
        if(output[DIR] == 1)
        {
            return true;
        }
        return false;
    }

    override
    public bool canHasOutput(int DIR)
    {
        if (possibleOutputs.Contains(DIR))
        {
            return true;
        }
        return false;
    }
}
