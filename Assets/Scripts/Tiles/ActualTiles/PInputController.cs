using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PInputController : InputBasedTile
{
    public Sprite[] inputSpritesOn;
    public Sprite[] inputSpritesOff;

    public string sign;

    public void setSprite(int index)
    {
        this.spriteOn = inputSpritesOn[index];
        this.spriteOff = inputSpritesOff[index];
    }
}
