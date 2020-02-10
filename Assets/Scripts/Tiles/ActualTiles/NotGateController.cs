using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NotGateController : InputBasedTile
{
    public float repeatRate;

    override
    protected void Start()
    {
        output = new int[4];
        tryPower(beingPowered); //for notgate

        checkNeighbourIndex = 0;
        neighbours = getNeighbours();
        sendPower(-1);
    }

    //deprecated
    protected void myUpdate()
    {
        if (placed)
        {
            //getInput();
            sendPower(-1); //new
            checkPower();
            tryPower(beingPowered);
        }
        else
        {
            tryPower(false);
            currentInputs = new List<int>();
        }

        if (homeObj != null)
        {
            spotIndex = homeObj.GetComponent<FloorTileController>().spotIndex;
        }
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

