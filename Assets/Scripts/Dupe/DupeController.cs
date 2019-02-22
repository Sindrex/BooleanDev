using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DupeController : MonoBehaviour {

    public GameController GC;

    public Sprite regular;
    public Sprite hover;
    public string displayName;

    private void OnMouseEnter()
    {
        this.GetComponent<SpriteRenderer>().sprite = hover;
    }

    private void OnMouseOver()
    {
        GC.changeItemName(displayName);
    }

    private void OnMouseExit()
    {
        this.GetComponent<SpriteRenderer>().sprite = regular;
        GC.closeItemName();
    }

    private void OnMouseDown()
    {
        this.GetComponent<SpriteRenderer>().sprite = regular;
        GC.duplicateSelected();
    }

    private void Update()
    {
        if (InputController.getInput(InputPurpose.INTERACT_TILE))
        {
            GC.duplicateSelected();
        }
    }
}
