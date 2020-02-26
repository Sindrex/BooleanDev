using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class InputBasedTile : OutputBasedTile {

    //Father for all input-based wires (generic, notGate, hopWire)

    public List<int> currentInputs = new List<int>();
    public List<int> possibleInputs = new List<int>();

    //for startup
    protected void checkNeighbour(int dir)  //new
    {
        GameObject go = neighbours[dir];
        if(go != null)
        {
            if(possibleInputs.Contains(dir) || possibleOutputs.Contains(dir))
            {
                InputBasedTile inputTile = go.GetComponent<InputBasedTile>();
                if (inputTile != null)
                {
                    //print("Updating neighbour: " + inputTile.spotIndex);
                    inputTile.needUpdatePower = true;
                    inputTile.exludeDirs.Add(-1);
                }
            }
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //new
        if(checkNeighbourIndex >= 0) //for startup, check 1 neighbour each frame
        {
            checkNeighbour(checkNeighbourIndex);
            checkNeighbourIndex++;
            if(checkNeighbourIndex > 3)
            {
                checkNeighbourIndex = -1;
            }
        }

        if (placed)
        {
            //getInput(); //checks neighbours for whether powered
            if (needUpdatePower)
            {
                while (exludeDirs.Count > 0) //one solution
                {
                    int exludeDir = exludeDirs[0];
                    exludeDirs.RemoveAt(0);

                    getInput();
                    checkPower();
                    sendPower(exludeDir); //new
                }
                needUpdatePower = false; //new
            }
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

        //new
        tryPower(beingPowered);
    }

    override
    public void tryPower(bool state)
    {
        if (state)
        {
            //print("beingPowered!");
            this.GetComponent<SpriteRenderer>().sprite = spriteOn;

            //List<int> okList = new List<int>();
            //List<int> notOkList = new List<int>();

            disableOutputs();

            //set power on outputs (all except the one getting input, for each input)
            foreach(int inDir in currentInputs)
            {
                for(int outDir = 0; outDir < 4; outDir++)
                {
                    if(possibleOutputs.Contains(outDir) && inDir != outDir)
                    {
                        output[outDir] = 1;
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

    //help method for sendpower
    override
    protected void powerNeighbour(int dir, int tileIndex)
    {
        //is there a tile there?
        GameObject curTile = GC.tiles[tileIndex];
        if (curTile != null)
        {
            //can it get an input?
            InputBasedTile inputTile = curTile.GetComponent<InputBasedTile>();
            if (inputTile != null)
            {
                //can it get input this way?
                int negDir = negDirection(dir);
                if (inputTile.possibleInputs.Contains(negDir))
                {
                    //print(Time.frameCount + ": " + spotIndex + " is sending ripple to: " + tileIndex + " from: " + dir + ", negDir: " + negDir + ", output: " + output[dir]);
                    if(output[dir] == 1 && inputTile.currentInputs.Contains(negDir))
                    {
                        return;
                    }
                    else if (output[dir] == 1) //send power
                    {
                        //print("Updating neighbour! I AM powered, but neighbour isnt");
                        if (!inputTile.currentInputs.Contains(negDir))
                        {
                            inputTile.currentInputs.Add(negDir);
                        }
                        inputTile.needUpdatePower = true;
                        inputTile.exludeDirs.Add(negDir);
                    }
                    else if(output[dir] == 0 && !inputTile.beingPowered)
                    {
                        //print(spotIndex + " is sending ripple to: " + tileIndex + " -> is returning");
                        return;
                    }
                    else
                    {
                        //print("Updating neighbour! I (" + spotIndex + ") am NOT powered, but neighbour is");
                        inputTile.currentInputs.Remove(negDir);
                        inputTile.needUpdatePower = true;
                        inputTile.exludeDirs.Add(negDir);
                    }
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
                if (neighbours[i].GetComponent<OutputBasedTile>().hasOutput(negDir) && !currentInputs.Contains(i) && possibleInputs.Contains(i))
                {
                    currentInputs.Add(i);
                }
                else if(!neighbours[i].GetComponent<OutputBasedTile>().hasOutput(negDir))
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
    public void rotateX(int times, bool rotate)
    {
        if (times < 0)
        {
            times = 3;
        }
        for (int x = 0; x < times; x++)
        {
            //AC.rotate();
            dir += 1;
            if (dir > 3)
            {
                dir = 0;
            }

            rotateInput();

            for (int i = 0; i < possibleOutputs.Count; i++)
            {
                possibleOutputs[i] += 1;
                if (possibleOutputs[i] > 3)
                {
                    possibleOutputs[i] = 0;
                }
            }

            int temp = output[output.Length - 1];
            for (int i = output.Length - 1; i >= 0; i--)
            {
                if (i - 1 >= 0)
                {
                    output[i] = output[i - 1]; //i: 3=2, 2=1, 1=0
                }
                else
                {
                    output[i] = temp; //0=3
                }

            }
            if (rotate)
            {
                transform.Rotate(0, 0, 90);
            }
        }
    }
}
