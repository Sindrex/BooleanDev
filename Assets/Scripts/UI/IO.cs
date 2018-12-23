using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IO : MonoBehaviour {

    public TileController.label myLabel;
    public IOPicker picker;
    public GameController GC;

    private void OnMouseOver()
    {
        GC.changeItemName(myLabel.ToString());
    }

    private void OnMouseDown()
    {
        picker.changeLabel(myLabel);
    }

    private void OnMouseExit()
    {
        GC.changeItemName("");
    }
}
