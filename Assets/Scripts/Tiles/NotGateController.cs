using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NotGateController : InputBasedWire
{
    public float repeatRate;

    override
    protected void Start()
    {
        //GC = GameObject.Find("GameController").GetComponent<GameController>();
        //AC = GameObject.Find("ActionBar").GetComponent<ActionbarController>();
        output = new int[4];

        if (repeatRate <= 0)
        {
            repeatRate = 0.05f;
        }
        InvokeRepeating("myUpdate", 0, repeatRate);
    }

    override
    protected void Update()
    {
        return; //NotGate has its own update (and frequency)
    }

    //override
    protected void myUpdate()
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

        if (homeObj != null)
        {
            spotIndex = homeObj.GetComponent<FloorTileController>().spotIndex;
        }
    }

    override
    protected void tryPower(bool state)
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

