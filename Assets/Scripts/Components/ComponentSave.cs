using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComponentSave {
    public string name;

    public int[] tileIDs;
    public int[] tileDIRs;
    public bool[] tilePower;
    public int[] tileSetting;
    public string[] signTexts;

    public int length;
    public int height;

    public TileController.label[] tileLabels;
    public string date;

    public ComponentSave()
    {
        //default values
        name = "Component1";
        tileIDs = new int[0];
        tileDIRs = new int[0];
        tilePower = new bool[0];
        tileSetting = new int[0];
        signTexts = new string[0];

        length = 0;
        height = 0;

        tileLabels = new TileController.label[0];

        date = "" + System.DateTime.Today.Day + "." + System.DateTime.Today.Month + "." + System.DateTime.Today.Year;
    }
}
