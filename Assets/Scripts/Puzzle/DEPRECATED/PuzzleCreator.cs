using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzleCreator {

    private string name;
    private int id;
    private int length; //floor length (row)
    private int height; //floor heigth (nr. of rows)

    //Works like pairs: first nr is tileNr, second nr is tileID, third is dir, fourth is input/output settings (-1 is out, 0 is null, 1 is in).
    private int[][] lockedTiles; //ID of tiles that are laid down already (and locked). 
    private PuzzleController.logic winCondition;
    //Perhaps allowed/enabled tiles (what kind of tiles player is allowed to place)
    private string desc;

    public PuzzleCreator(string name, int id, int length, int height, int[][] lockedTiles, PuzzleController.logic winCondition, string desc)
    {
        this.name = name;
        this.id = id;
        this.length = length;
        this.height = height;
        this.desc = desc;

        this.lockedTiles = lockedTiles;
        this.winCondition = winCondition;
    }

    public string getName()
    {
        return name;
    }
    public int getID()
    {
        return id;
    }
    public int getLength()
    {
        return length;
    }
    public int getHeight()
    {
        return height;
    }
    public int[][] getLockedTiles()
    {
        return lockedTiles;
    }
    public PuzzleController.logic getWinCondition()
    {
        return winCondition;
    }
    public string getDesc()
    {
        return desc;
    }
}
