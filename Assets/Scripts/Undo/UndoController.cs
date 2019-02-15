using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoController : MonoBehaviour {

    public Undo root; //Stack: Linked list
    public GameController GC;

    public void addUndo(Undo newUndo)
    {
        //print("UndoController:addUndo(): " + newUndo.stringify());
        if(root != null)
        {
            newUndo.child = root;
        }
        root = newUndo;
        root.checkLength(PlayerPrefs.GetInt(OptionController.gameplayOK[2]), 1);
    }

    public void undo()
    {
        if(root != null)
        {
            GC.AC.prevSpotIndex = -1;
            //print("Undo-ing!");
            root.undoMe(GC);
            if(root.child != null)
            {
                root = root.child;
            }
            else
            {
                root = null;
            }
        }
    }

    public string stringifyRoot()
    {
        return root.stringify();
    }
}
