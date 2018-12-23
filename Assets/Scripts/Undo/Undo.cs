using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Undo {

    //Possible undos: 
    //Single tile: Deleted a tile, place tile, move tile
    //Multiple tiles: deleted a Selected Area (lots of tiles), duplicate, move selected area, perhaps (component)
    public Undo child; //reference to next in stack

    public Undo(){}

    //Remove undo after a certain length
    public bool checkLength(int max, int current)
    {
        Debug.Log("Check: M/C: " + max + "/" + current);
        if(current >= max)
        {
            //Debug.Log("current >= max, child: " + child);
            child = null;
            return false;
        }
        else if(child != null)
        {
            current++;
            return child.checkLength(max, current);
        }
        else
        {
            return true;
        }
    }

    public abstract void undoMe(GameController GC);
    public abstract string stringify();
}
