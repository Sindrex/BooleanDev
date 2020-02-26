using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HopWireController : InputBasedTile
{
    public Sprite spriteOn3; //For both on
    public Sprite spriteOn2; //for horiz on

    [SerializeField]
    private bool beingPoweredVert = false;
    [SerializeField]
    private bool beingPoweredHoriz = false;

    override
    protected void checkPower()
    {
        beingPoweredVert = false;
        beingPoweredHoriz = false;

        if (currentInputs.Contains(UP) || currentInputs.Contains(DOWN))
        {
            if(dir == 0 || dir == 2)
            {
                beingPoweredVert = true;
            }
            else //1 and 3
            {
                beingPoweredHoriz = true;
            }

        }
        if (currentInputs.Contains(LEFT) || currentInputs.Contains(RIGHT))
        {
            if (dir == 0 || dir == 2)
            {
                beingPoweredHoriz = true;
            }
            else //1 and 3
            {
                beingPoweredVert = true;
            }
        }

        beingPowered = beingPoweredHoriz || beingPoweredVert;

        tryPower(beingPoweredVert, beingPoweredHoriz);
    }

    protected void tryPower(bool vert, bool horiz)
    {
        if(vert && horiz)
        {
            this.GetComponent<SpriteRenderer>().sprite = spriteOn3;
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
                        if((inDIR == 0 || inDIR == 2) && (outDIR == 0 || outDIR == 2))
                        {
                            //print("OutDir: " + outDIR + ", DIR " + DIR);
                            output[outDIR] = 1;
                            okList.Add(outDIR);
                        }
                        if ((inDIR == 1 || inDIR == 3) && (outDIR == 1 || outDIR == 3))
                        {
                            //print("OutDir: " + outDIR + ", DIR " + DIR);
                            output[outDIR] = 1;
                            okList.Add(outDIR);
                        }
                    }
                    else
                    {
                        notOkList.Add(outDIR);
                    }
                }
            }

            foreach (int DIR in notOkList)
            {
                if (!okList.Contains(DIR))
                {
                    output[DIR] = 0;
                }
            }
        }
        else if (horiz)
        {
            this.GetComponent<SpriteRenderer>().sprite = spriteOn2;
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
                        if((outDIR == LEFT || outDIR == RIGHT) && (dir == 0 || dir == 2))
                        {
                            output[outDIR] = 1;
                            okList.Add(outDIR);
                        }
                        else if((outDIR == UP || outDIR == DOWN) && (dir == 1 || dir == 3))
                        {
                            output[outDIR] = 1;
                            okList.Add(outDIR);
                        }
                        else
                        {
                            notOkList.Add(outDIR);
                        }
                    }
                    else
                    {
                        notOkList.Add(outDIR);
                    }
                }
            }

            foreach (int DIR in notOkList)
            {
                if (!okList.Contains(DIR))
                {
                    output[DIR] = 0;
                }
            }
        }
        else if (vert)
        {
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
                        if ((outDIR == UP || outDIR == DOWN) && (dir == 0 || dir == 2))
                        {
                            output[outDIR] = 1;
                            okList.Add(outDIR);
                        }
                        else if((outDIR == LEFT || outDIR == RIGHT) && (dir == 1 || dir == 3))
                        {
                            output[outDIR] = 1;
                            okList.Add(outDIR);
                        }
                        else
                        {
                            notOkList.Add(outDIR);
                        }
                    }
                    else
                    {
                        notOkList.Add(outDIR);
                    }
                }
            }

            foreach (int DIR in notOkList)
            {
                if (!okList.Contains(DIR))
                {
                    output[DIR] = 0;
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
                int index = possibleOutputs[i];
                if (index >= 0 && index < output.Length)
                {
                    output[index] = 0;
                }
            }
        }
    }

    override
    public void sendPower(int exludeDir) //exludeDir -1 for no exlude
    {
        int leftTileIndex = spotIndex - 1;
        int rightTileIndex = spotIndex + 1;
        int upTileIndex = spotIndex - GC.length;
        int downTileIndex = spotIndex + GC.length;

        if(exludeDir == -1)
        {
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
                else if (possibleOutputs[i] == UP && upTileIndex >= 0 && exludeDir != UP)
                {
                    powerNeighbour(UP, upTileIndex);
                }
                //Can we send power to down?
                else if (possibleOutputs[i] == DOWN && downTileIndex < GC.tiles.Length && exludeDir != DOWN)
                {
                    powerNeighbour(DOWN, downTileIndex);
                }
            }
        }
        else if (exludeDir == LEFT)
        {
            if (possibleOutputs.Contains(RIGHT) && rightTileIndex < GC.tiles.Length)
            {
                powerNeighbour(RIGHT, rightTileIndex);
            }
        }
        else if (exludeDir == RIGHT)
        {
            if (possibleOutputs.Contains(LEFT) && leftTileIndex >= 0)
            {
                powerNeighbour(LEFT, leftTileIndex);
            }
        }
        else if (exludeDir == UP)
        {
            if (possibleOutputs.Contains(DOWN) && downTileIndex < GC.tiles.Length)
            {
                powerNeighbour(DOWN, downTileIndex);
            }
        }
        else if (exludeDir == DOWN)
        {
            if (possibleOutputs.Contains(UP) && upTileIndex >= 0)
            {
                powerNeighbour(UP, upTileIndex);
            }
        }
    }
}

