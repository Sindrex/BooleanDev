using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizerController : InputBasedWire {

    //When powered: 50/50 output.

    public Sprite spriteOn2;
    public Sprite spriteOn3;
    private bool isOn = false;

    override
    protected void tryPower(bool state)
    {
        if (state)
        {
            //print("beingPowered!");

            if (!isOn)
            {
                isOn = true;

                int random = Random.Range(0, 2);
                print("R: " + random);

                //randomly
                if (random > 0)
                {
                    //power outputs 
                    power();
                    this.GetComponent<SpriteRenderer>().sprite = spriteOn;
                }
                else
                {
                    if(currentInputs.Contains(0) || currentInputs.Contains(3))
                    {
                        if(dir == 0 || dir == 1)
                        {
                            this.GetComponent<SpriteRenderer>().sprite = spriteOn3;
                        }
                        else
                        {
                            this.GetComponent<SpriteRenderer>().sprite = spriteOn2;
                        }
                    }
                    else
                    {
                        if (dir == 0 || dir == 1)
                        {
                            this.GetComponent<SpriteRenderer>().sprite = spriteOn2;
                        }
                        else
                        {
                            this.GetComponent<SpriteRenderer>().sprite = spriteOn3;
                        }
                    }

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
        else //not being powered
        {
            //print("Off!");
            this.GetComponent<SpriteRenderer>().sprite = spriteOff;
            isOn = false;

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

    private void power()
    {
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
                    //print("OutDir: " + outDIR + ", DIR " + DIR);
                    output[outDIR] = 1;
                    okList.Add(outDIR);
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
}
