using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DelayerController : InputBasedTile
{
    private readonly int MAX_SETTING = 3;
    public int setting = 0; //0-3 (count: 4)
    public float delayAmount = 0.2f;

    public bool canPower = false;

    public Sprite[] mySprites = new Sprite[8]; //0-3 is off, 4-7 is on

    override
    public void tryPower(bool state)
    {
        if (state)
        {
            //print("beingPowered!");
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
        else //not being powered
        {
            //de-power outputs
            for (int i = 0; i < possibleOutputs.Count; i++)
            {
                int DIR = possibleOutputs[i];
                if (DIR >= 0 && DIR < output.Length)
                {
                    output[DIR] = 0;
                }
            }
        }
    }

    override
    protected void checkPower()
    {
        canPower = false;
        //All 4 dirs
        for (int i = 0; i < 4; i++)
        {
            if (currentInputs.Contains(i))
            {
                //print("powered!");
                canPower = true;
                StartCoroutine("powerOnDelay");
            }
        }
        if (!canPower)
        {
            StartCoroutine("powerOffDelay");
        }
    }

    IEnumerator powerOnDelay()
    {
        yield return new WaitForSeconds(delayAmount * (1 + setting));
        beingPowered = true;
        tryPower(beingPowered);
        switchSprite(beingPowered);
        needUpdatePower = true;
    }

    IEnumerator powerOffDelay()
    {
        yield return new WaitForSeconds(delayAmount * (1 + setting));
        beingPowered = false;
        tryPower(beingPowered);
        switchSprite(beingPowered);
        needUpdatePower = true;
        //print("done off!");
    }

    private void switchSprite(bool state)
    {
        if (state) //on
        {
            //print("switching sprite: on - " + setting + "/" + (mySprites.Length / 2));
            this.GetComponent<SpriteRenderer>().sprite = mySprites[setting + (mySprites.Length / 2)];
        }
        else //off
        {
            //print("switching sprite: off - " + setting);
            this.GetComponent<SpriteRenderer>().sprite = mySprites[setting];
        }
    }

    private void OnMouseOver()
    {
        //print("yo");
        if (InputController.getInput(InputPurpose.INTERACT_TILE) && placed)
        {
            setting++;
            if(setting > MAX_SETTING)
            {
                setting = 0;
            }
            AC.audioCon.playTilePlacedSFX();
        }
        switchSprite(beingPowered);
    }
}
