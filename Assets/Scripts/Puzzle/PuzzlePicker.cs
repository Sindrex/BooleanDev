using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzlePicker : MonoBehaviour
{
    public int id;
    public MainMenu MM;

    public Text myName;

    //Completed
    public GameObject myCheckedPrefab;
    private GameObject myChecked;
    public GameObject lockedPrefab;
    private bool locked = true;

    private void Start()
    {
        myChecked = Instantiate(myCheckedPrefab, this.transform);
        myChecked.SetActive(false);
        int done = PlayerPrefs.GetInt(MainMenu.puzzlePrefKey + id);
        //print(MainMenu.puzzlePrefKey + index + ": " + done);
        if(done > 0)
        {
            myChecked.SetActive(true);
            locked = false;
        }

        if(MM.puzzleArray.EOTP_puzzles.Length - 1 < id)
        {
            //print("yo: " + id + " > " + MM.puzzleArray.EOTP_puzzles.Length);
            //Not implemented
            locked = true;
            GetComponent<Image>().color = Color.gray;
            GetComponent<Button>().interactable = false;
            return;
        }
        else if (MM.puzzlesDone >= MM.puzzleArray.EOTP_puzzles[id].minDone)
        {
            locked = false;
        }

        if (locked)
        {
            Instantiate(lockedPrefab, transform);
            GetComponent<Image>().color = Color.red;
            GetComponent<Button>().interactable = false;
        }
        myName.text = MM.puzzleArray.EOTP_puzzles[id].name;
    }

    public void onClick()
    {
        print("Clicked: " + id);
        if (locked)
        {
            print("Locked: " + id);
            return;
        }
        MM.puzzlePickLevel(id);
    }
}
