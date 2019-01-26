using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzlePicker : MonoBehaviour
{
    public int id;
    public MainMenu MM;

    //Completed
    public GameObject myCheckedPrefab;
    private GameObject myChecked;
    public GameObject lockedPrefab;
    private bool locked = true;

    //Hover
    public Text hoverText;
    private string myText = "";

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

        int prevDone = PlayerPrefs.GetInt(MainMenu.puzzlePrefKey + (id - 1));
        int prev2Done = PlayerPrefs.GetInt(MainMenu.puzzlePrefKey + (id - 2));
        if (prevDone > 0 || prev2Done > 0 || id == 0 || id == 1 || id == 2)
        {
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

        if (locked)
        {
            Instantiate(lockedPrefab, transform);
            GetComponent<Image>().color = Color.red;
        }
        else
        {
            GetComponent<Image>().color = Color.green;
        }
    }

    public void onClick()
    {
        if (locked)
        {
            return;
        }
        MM.puzzlePickLevel(id);
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("PuzzlePicker"))
        {
            if(go.GetComponent<Image>().color == Color.yellow)
            {
                go.GetComponent<Image>().color = Color.green;
            }
        }
        this.GetComponent<Image>().color = Color.yellow;
    }

    private void OnMouseEnter()
    {
        //hoverText.text = myText;
    }

    private void OnMouseExit()
    {
        //hoverText.text = "";
    }
}
