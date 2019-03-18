﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ButtonController : TileController
{
    private bool buttonWait = false;
    public float waitTime = 0.5f;

    public GameObject hover;

    // Update is called once per frame
    void Update()
    {
        if (placed)
        {
            if (!buttonWait)
            {
                beingPowered = false;
            }
            tryPower(beingPowered);
        }
        else
        {
            tryPower(false);
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
                    output[index] = 1;
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

    private void OnMouseOver()
    {
        //hover.SetActive(true);
        //print("yo");
        if (InputController.getInput(InputPurpose.INTERACT_TILE) && placed)
        {
            if (!beingPowered && !buttonWait)
            {
                beingPowered = true;
                AC.audioCon.playTilePlacedSFX();
                StartCoroutine("buttonWaiter");
            }
        }
    }

    private void OnMouseExit()
    {
        //hover.SetActive(false);
    }

    IEnumerator buttonWaiter()
    {
        buttonWait = true;
        yield return new WaitForSeconds(waitTime);
        buttonWait = false;
    }

    override
    public bool hasOutput(int DIR)
    {
        if (output[DIR] == 1)
        {
            return true;
        }
        return false;
    }

    override
    protected void rotateInput()
    {
        return; //Nothing happens for buttons bc they dont have input!
    }
}
