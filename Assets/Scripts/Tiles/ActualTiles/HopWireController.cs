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
    protected void Update()
    {
        //new
        if (checkNeighbourIndex >= 0) //for startup, check 1 neighbour each frame
        {
            checkNeighbour(checkNeighbourIndex);
            checkNeighbourIndex++;
            if (checkNeighbourIndex > 3)
            {
                checkNeighbourIndex = -1;
            }
        }

        if (placed)
        {
            if (needUpdatePower)
            {
                getInput();
                checkPower();
                //tryPower(beingPoweredVert, beingPoweredHoriz);
                sendPower(exludeSendPowerDir); //new
                needUpdatePower = false; //new
                exludeSendPowerDir = -1; //new
            }
        }
        else
        {
            //tryPower(false);
            currentInputs = new List<int>();
        }

        if (homeObj != null)
        {
            spotIndex = homeObj.GetComponent<FloorTileController>().spotIndex;
        }
    }

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
                int DIR = currentInputs[j];
                for (int i = 0; i < possibleOutputs.Count; i++)
                {
                    int outDIR = possibleOutputs[i];
                    if (outDIR >= 0 && outDIR < output.Length && outDIR != DIR)
                    {
                        if((DIR == 0 || DIR == 2) && (outDIR == 0 || outDIR == 2))
                        {
                            //print("OutDir: " + outDIR + ", DIR " + DIR);
                            output[outDIR] = 1;
                            okList.Add(outDIR);
                        }
                        if ((DIR == 1 || DIR == 3) && (outDIR == 1 || outDIR == 3))
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
                int DIR = currentInputs[j];
                for (int i = 0; i < possibleOutputs.Count; i++)
                {
                    int outDIR = possibleOutputs[i];
                    if (outDIR >= 0 && outDIR < output.Length && outDIR != DIR)
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
                int DIR = currentInputs[j];
                for (int i = 0; i < possibleOutputs.Count; i++)
                {
                    int outDIR = possibleOutputs[i];
                    if (outDIR >= 0 && outDIR < output.Length && outDIR != DIR)
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
}

