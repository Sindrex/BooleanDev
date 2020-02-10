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
        header.text = puzzle.name;
        desc.text = puzzle.desc;
    }

    public void openObjective()
    {
        textFather.SetActive(!textFather.activeSelf);
    }

    public void toggleObjective()
    {
        textFather.SetActive(!textFather.activeSelf);
        GC.audioMixer.playButtonSFX();
    }
}
