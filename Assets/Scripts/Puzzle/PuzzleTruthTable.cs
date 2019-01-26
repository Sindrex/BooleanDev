using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleTruthTable : MonoBehaviour {

    public GameController GC;
    public GameObject textFather;
    public Text text;
    public string displayName;

    // Use this for initialization
    void Start () {
        textFather.SetActive(false);
    }

    public void setup(EOTP_PuzzleCreator puzzle)
    {
        string res = "";
        for(int i = 0; i < puzzle.IO.Length; i++)
        {
            EOTP_IOCreator myIO = puzzle.IO[i];
            string myRes = myIO.sign;
            for(int j = 0; j < myIO.signal.Length; j++)
            {
                myRes += " | " + myIO.signal[j];
            }
            res += myRes;
            res += "\n";
        }
        text.text = res;
    }

    private void OnMouseOver()
    {
        textFather.SetActive(true);
        GC.changeItemName(displayName);
    }

    private void OnMouseExit()
    {
        textFather.SetActive(false);
        GC.closeItemName();
    }
}
