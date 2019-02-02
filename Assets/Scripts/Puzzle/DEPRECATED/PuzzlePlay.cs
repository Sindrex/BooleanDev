using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzlePlay : MonoBehaviour {

    public GameController GC;
    public string displayName;

    //EOTP testing
    public EOTP_PuzzleController ePC;

    //Hover size
    public Vector3 hoverScale;

    private void OnMouseOver()
    {
        GC.changeItemName(displayName);
        GetComponent<RectTransform>().localScale = hoverScale;
    }

    private void OnMouseExit()
    {
        GC.closeItemName();
        GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
    }

    private void OnMouseDown()
    {
        print(this.GetType().Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + ": EOTP: Play");
        //PC.pathfinder();
        ePC.puzzlePlay();
    }
}
