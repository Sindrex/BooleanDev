using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTileUndo : Undo{

    public bool deleted = false;
    public int tileId;
    public int dir;
    public int spotIndex;

    public int setting;
    public string signText;

    public bool moved = false;
    public int spotIndexTo;

    public SingleTileUndo(int tileId, int dir, int spotIndex, int setting, string signText) : base()
    {
        this.deleted = true;
        this.tileId = tileId;
        this.dir = dir;
        this.spotIndex = spotIndex;
        this.setting = setting;
        this.signText = signText;
    }
    public SingleTileUndo(int spotIndex) : base() //Tile was placed
    {
        this.deleted = false;
        this.spotIndex = spotIndex;
    }
    public SingleTileUndo(int from, int to) : base() //Tile was moved
    {
        this.moved = true;
        this.spotIndex = from;
        this.spotIndexTo = to;
    }

    public override void undoMe(GameController GC)
    {
        if (moved)
        {
            GC.moveSingle(spotIndexTo, spotIndex);
        }
        else if (!deleted)
        {
            //tile was placed, now delete
            GC.deleteSingle(spotIndex);
        }
        else
        {
            //tile was deleted, now redo
            GC.spawnSingle(spotIndex, tileId, dir, false, setting, signText, TileController.label.NULL);
        }
    }

    public override string stringify()
    {
        return "SingleTileUndo: Deleted: " + this.deleted + ", moved: " + moved + ", tileId: " + tileId + ", dir: " + dir + ", index: " + spotIndex + ", setting: " + setting + ", text: " + signText;
    }
}
