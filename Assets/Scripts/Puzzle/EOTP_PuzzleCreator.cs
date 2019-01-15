using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EOTP_PuzzleCreator
{
    [SerializeField]
    public string name;
    [SerializeField]
    public int id;
    [SerializeField]
    public int length; //floor length (row)
    [SerializeField]
    public int height; //floor heigth (nr. of rows)

    [SerializeField]
    public EOTP_IOCreator[] IO; //An array of the Inputs and Outputs for this puzzle
    [SerializeField]
    public int[] allowedTiles;

    [SerializeField]
    public string desc;
    [SerializeField]
    public string winDesc;

    public EOTP_PuzzleCreator(string name, int id, int length, int height, EOTP_IOCreator[] IO, int[] allowedTiles, string desc)
    {
        this.name = name;
        this.id = id;
        this.length = length;
        this.height = height;
        this.desc = desc;
        this.allowedTiles = allowedTiles;

        this.IO = IO;
    }
}
