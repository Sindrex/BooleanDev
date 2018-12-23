using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTilesUndo : Undo {

    public bool deleted;
    public int[] tileIds;
    public int[] dirs;
    public int[] spotIndexes;
    public bool[] tilePowers;
    public int[] settings;
    public string[] signTexts;

    public bool moved = false;
    public int[] spotIndexes2;

    //deleted by selecting
    public MultipleTilesUndo(int[] tileIds, int[] dirs, int[] spotIndexes, bool[] tilePowers, int[] settings, string[] signTexts) : base()
    {
        this.deleted = true;
        this.tileIds = tileIds;
        this.dirs = dirs;
        this.spotIndexes = spotIndexes;
        this.settings = settings;
        this.signTexts = signTexts;
        this.tilePowers = tilePowers;
    }
    public MultipleTilesUndo(int[] spotIndexes) : base() //Place a component / duplicate
    {
        this.deleted = false;
        this.spotIndexes = spotIndexes;
    }
    public MultipleTilesUndo(int[] spotIndexes, int[] spotIndexes2) //move multiple
    {
        this.deleted = false;
        this.moved = true;
        this.spotIndexes = spotIndexes;
        this.spotIndexes2 = spotIndexes2;
    }

    public override void undoMe(GameController GC)
    {
        if (moved)
        {
            GC.moveMultiple(spotIndexes, spotIndexes2);  
        }
        else if (!deleted)
        {
            //tiles was placed (duplicate), now delete
            GC.deleteMultiple(spotIndexes);
        }
        else
        {
            //tile was deleted, now redo
            GC.spawnMultiple(tileIds, dirs, spotIndexes, tilePowers, settings, signTexts);
        }
    }
    public override string stringify()
    {
        return "MultipleTilesUndo: Deleted: " + deleted + ", moved: " + moved;
    }
}
