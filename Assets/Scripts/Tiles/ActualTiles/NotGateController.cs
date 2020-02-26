using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NotGateController : InputBasedTile
{
    override
    protected void Start()
    {
        output = new int[4];
        checkNeighbourIndex = 0;
        neighbours = getNeighbours();
        exludeDirs = new List<int>();

        tryPower(beingPowered); //for notgate
        sendPower(-1); //for notgate
    }

    override
    public void tryPower(bool state)
    {
        if (state)
        {
            //print("beingPowered!");
            this.GetComponent<SpriteRenderer>().sprite = spriteOn;

            //power outputs
            for (int i = 0; i < possibleOutputs.Count; i++)
            {
                int index = possibleOutputs[i];
                if (index >= 0 && index < output.Length)
                {
                    output[index] = 0;
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
                    output[index] = 1;
                }
            }
        }
    }
}

