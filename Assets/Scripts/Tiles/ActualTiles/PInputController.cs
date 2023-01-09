using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PInputController : InputBasedTile
{
    public Sprite[] inputSpritesOn;
    public Sprite[] inputSpritesOff;
    public SpriteRenderer Renderer;

    public string sign;

    public void setSprite(int index) //setup
    {
        this.spriteOn = inputSpritesOn[index];
        this.spriteOff = inputSpritesOff[index];
        Renderer.sprite = this.spriteOff;
    }
}
