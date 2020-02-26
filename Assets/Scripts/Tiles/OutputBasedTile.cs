using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OutputBasedTile : TileController {

    //Father for all output (and input) based tiles (those that deal with power)

    //output
    [SerializeField]
    protected int[] output = new int[4]; //0 = off, 1 = on
    [SerializeField]
    protected List<int> possibleOutputs = new List<int>();

    //Power updating
    public GameObject[] neighbours = new GameObject[4];
    public int checkNeighbourIndex = -1; //new
    public bool needUpdatePower = false; //new
    //public int[] exludeSendPowerDir = new int[4];
    public List<int> exludeDirs; //queue

    protected virtual void Start()
    {
        output = new int[4];
        checkNeighbourIndex = 0;
        neighbours = getNeighbours();
        //exludeSendPowerDir = new int[4];
        exludeDirs = new List<int>();
    }

    protected GameObject[] getNeighbours()
    {
        //print("Getting neighbours for: " + spotIndex);
        GameObject[] tempList = new GameObject[4];

        if (spotIndex - 1 >= 0)//Check if there is a spot to the left
        {
            if (GC.tiles[spotIndex - 1] != null)
            {
                tempList[1] = GC.tiles[spotIndex - 1];
            }
        }
        //right
        if (spotIndex + 1 < GC.tiles.Length)
        {
            if (GC.tiles[spotIndex + 1] != null)
            {
                tempList[3] = GC.tiles[spotIndex + 1];
            }
        }
        //up
        if (spotIndex - GC.length >= 0)
        {
            if (GC.tiles[spotIndex - GC.length] != null)
            {
                tempList[0] = GC.tiles[spotIndex - GC.length];
            }
        }
        //down
        if (spotIndex + GC.length < GC.tiles.Length)
        {
            if (GC.tiles[spotIndex + GC.length] != null)
            {
                tempList[2] = GC.tiles[spotIndex + GC.length];
            }
        }
        return tempList;
    }

    //NEW, instead of getInput
    public virtual void sendPower(int exludeDir) //exludeDir -1 for no exlude
    {
        int leftTileIndex = spotIndex - 1;
        int rightTileIndex = spotIndex + 1;
        int upTileIndex = spotIndex - GC.length;
        int downTileIndex = spotIndex + GC.length;

        //possibleOutputs
        for (int i = 0; i < possibleOutputs.Count; i++)
        {
            //Can we send power to left?
            if (possibleOutputs[i] == LEFT && leftTileIndex >= 0 && exludeDir != LEFT)
            {
                powerNeighbour(LEFT, leftTileIndex);
            }
            //Can we send power to right?
            else if (possibleOutputs[i] == RIGHT && rightTileIndex < GC.tiles.Length && exludeDir != RIGHT)
            {
                powerNeighbour(RIGHT, rightTileIndex);
            }
            //Can we send power to up?
            else if(possibleOutputs[i] == UP && upTileIndex >= 0 && exludeDir != UP)
            {
                powerNeighbour(UP, upTileIndex);
            }
            //Can we send power to down?
            else if(possibleOutputs[i] == DOWN && downTileIndex < GC.tiles.Length && exludeDir != DOWN)
            {
                powerNeighbour(DOWN, downTileIndex);
            }
        }
    }

    //protected = child can access, virtual = child can override if wanted
    protected virtual void powerNeighbour(int dir, int tileIndex)
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
                    if (output[dir] == 1)
                    {
                        if (!inputTile.currentInputs.Contains(negDir))
                        {
                            inputTile.currentInputs.Add(negDir);
                        }
                        inputTile.needUpdatePower = true;
                        inputTile.exludeDirs.Add(negDir);
                        //inputTile.exludeSendPowerDir = -1;
                    }
                    else
                    {
                        inputTile.currentInputs.Remove(negDir);
                        inputTile.needUpdatePower = true;
                        inputTile.exludeDirs.Add(negDir);
                    }
                }
            }
        }
    }

    public abstract void tryPower(bool state);

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

    public bool hasOutput(int DIR)
    {
        if (output[DIR] == 1)
        {
            return true;
        }
        return false;
    }

    public bool canHasOutput(int DIR)
    {
        if (possibleOutputs.Contains(DIR))
        {
            return true;
        }
        return false;
    }

    public void enableOutputs()
    {
        foreach(int dir in possibleOutputs)
        {
            output[dir] = 1;
        }
    }

    protected void disableOutputs()
    {
        //de-power outputs
        output = new int[4];
        /*
        for (int i = 0; i < possibleOutputs.Count; i++)
        {
            int DIR = possibleOutputs[i];
            if (DIR >= 0 && DIR < output.Length)
            {
                output[DIR] = 0;
            }
        }*/
    }

    override
    public bool destroyMe(bool addUndo)
    {
        beingPowered = false;
        disableOutputs();
        exludeDirs = new List<int>();
        sendPower(-1);
        disableOutputs();

        //call parent destroyme
        return base.destroyMe(addUndo);
    }
}