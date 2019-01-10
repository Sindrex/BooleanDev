using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzlePlay : MonoBehaviour {

    public PuzzleController PC;
    public GameController GC;
    public string displayName;

    //EOTP testing
    public EOTP_PuzzleController ePC;

    private void OnMouseOver()
    {
        GC.changeItemName(displayName);
    }

    private void OnMouseExit()
    {
        GC.closeItemName();
    }

    private void OnMouseDown()
    {
        print(this.GetType().Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + ": EOTP: Play");
        //PC.pathfinder();
        ePC.puzzlePlay();
    }
}
