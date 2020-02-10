using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class POutputController : OutputBasedTile
{
    public Sprite[] outputSpritesOn;
    public Sprite[] outputSpritesOff;

    public string sign;

    public void setSprite(int index)
    {
        print("Setting sprite: " + index);
        this.spriteOn = outputSpritesOn[index];
        this.spriteOff = outputSpritesOff[index];
        tryPower(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (placed)
        {
            //tryPower(beingPowered);
        }
        else
        {
            //tryPower(false);
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
}
