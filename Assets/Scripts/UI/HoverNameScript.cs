using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverNameScript : MonoBehaviour {

    //NOTE: Requires a Box collider 2d on same object

    public GameController GC;
    public string displayName;

    private void OnMouseOver()
    {
        GC.changeItemName(displayName);
    }

    private void OnMouseExit()
    {
        GC.closeItemName();
    }
}
