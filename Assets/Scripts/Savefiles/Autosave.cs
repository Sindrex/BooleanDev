using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autosave : MonoBehaviour {

    public GameController GC;
    public SystemController SC;

    private bool isPuzzle = false;

	// Use this for initialization
	void Start () {
        bool doAutosave = PlayerPrefs.GetInt(OptionController.genericsOK[4]) == 1 ? true : false;
        int autosaveTimer = PlayerPrefs.GetInt(OptionController.genericsOK[5]);
        if (doAutosave)
        {
            print("Autosave is on, timer: " + autosaveTimer);

            if (GC.ePC.isPuzzle)
            {
                isPuzzle = true;
                print("puzzlename: " + GC.ePC.myPuzzle.name);
            }

            InvokeRepeating("save", autosaveTimer, autosaveTimer);
        }
        else
        {
            print("Not autosave :(");
            //print("Autosave timer: " + autosaveTimer);
        }
    }

    void save()
    {
        print("Autosaving! Puzzle: " + isPuzzle);

        GC.updateData();
        Game.current.tileIDs = GC.tileIDs;
        Game.current.tileDIRs = GC.tileDIRs;
        Game.current.tilePower = GC.tilePower;
        Game.current.tileSetting = GC.tileSetting;
        Game.current.signTexts = GC.signTexts;

        if (!isPuzzle)
        {
            if (SaveLoad.save())
            {
                SC.saveGraphic();
            }
            else
            {
                print("Autosave: Couldnt save!");
            }
        }
        else
        {
            if (SaveLoad.savePuzzle(GC.ePC.myPuzzle, PlayerPrefs.GetInt(SystemController.puzzleSaveSlotKey)))
            {
                SC.saveGraphic();
            }
            else
            {
                print("Autosave: Couldnt save puzzle!");
            }
        }
    }
}
