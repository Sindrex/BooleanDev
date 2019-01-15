using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IOController : InputBasedWire
{
    public Sprite inputSprite;
    public Sprite outputSprite;

    public string sign;

    public void setSprite(bool input)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (input)
        {
            renderer.sprite = inputSprite;
        }
        else
        {
            renderer.sprite = outputSprite;
        }
    }
}
