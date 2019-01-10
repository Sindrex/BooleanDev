using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game {

    public static Game current;

    public string gameName;

    public int[] tileIDs;
    public int[] tileDIRs;
    public bool[] tilePower;
    public int[] tileSetting;
    public string[] signTexts;

    //Board
    public int length;
    public int height;

    //Bools
    public bool newSave;
    public EOTP_PuzzleCreator puzzle;

    //WorldOptions
    public bool lockTiles;
    public bool noPlaceTiles;

    //History
    public string dateCreated;
    public string dateLastPlayed;
    public string version = "Alpha 2";

    public Game(EOTP_PuzzleCreator puzzle)
    {
        gameName = "";
        tileIDs = new int[0];
        tileDIRs = new int[0];
        tilePower = new bool[0];
        tileSetting = new int[0];
        signTexts = new string[0];

        //default values
        length = 100;
        height = 80;

        newSave = true;
        this.puzzle = puzzle;
        lockTiles = false;
        noPlaceTiles = false;

        dateCreated = System.DateTime.Today.ToString("dd/MM/yyyy");
    }
}
