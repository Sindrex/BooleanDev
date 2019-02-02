using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class POutputController : TileController
{
    public Sprite[] outputSpritesOn;
    public Sprite[] outputSpritesOff;

    public string sign;

    public void setSprite(int index)
    {
        this.spriteOn = outputSpritesOn[index];
        this.spriteOff = outputSpritesOff[index];
    }

    // Update is called once per frame
    void Update()
    {
        if (placed)
        {
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
