using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EOTP_PuzzleCreator
{
    [SerializeField]
    private string name;
    [SerializeField]
    private int id;
    [SerializeField]
    private int length; //floor length (row)
    [SerializeField]
    private int height; //floor heigth (nr. of rows)

    [SerializeField]
    private EOTP_IOCreator[] IO; //An array of the Inputs and Outputs for this puzzle

    //Perhaps allowed/enabled tiles (what kind of tiles player is allowed to place)
    private string desc;

    public EOTP_PuzzleCreator(string name, int id, int length, int height, EOTP_IOCreator[] IO, string desc)
    {
        this.name = name;
        this.id = id;
        this.length = length;
        this.height = height;
        this.desc = desc;

        this.IO = IO;
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
    public EOTP_IOCreator[] getIO()
    {
        return IO;
    }
    public string getDesc()
    {
        return desc;
    }
    override
    public string ToString()
    {
        return name + ", " + id + ", " + length + " x " + height + ", " + desc;
    }
}
