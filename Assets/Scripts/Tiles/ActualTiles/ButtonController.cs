using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ButtonController : OutputBasedTile
{
    private bool buttonWait = false;
    public float waitTime = 0.5f;

    public GameObject hover;
    
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
                tryPower(beingPowered); //new
                sendPower(-1); //new
                StartCoroutine("buttonWaiter");
            }
        }
    }

    IEnumerator buttonWaiter()
    {
        yield return new WaitForSeconds(waitTime);
        beingPowered = false;
        tryPower(beingPowered); //new
        sendPower(-1); //new
    }
}
