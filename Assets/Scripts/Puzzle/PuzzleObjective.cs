using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PuzzleObjective : MonoBehaviour {

    public GameController GC;
    public GameObject textFather;
    public Text header;
    public Text desc;
    public string displayName;

    private void Start()
    {
        textFather.SetActive(false);
    }

    public void setup(PuzzleCreator puzzle)
    {
        header.text = puzzle.getName();
        desc.text = puzzle.getDesc();
    }
    public void setup(EOTP_PuzzleCreator puzzle)
    {
        header.text = puzzle.getName();
        desc.text = puzzle.getDesc();
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
