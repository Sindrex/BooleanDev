using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzlePicker : MonoBehaviour
{
    public int index;
    public MainMenu MM;

    //Completed
    public GameObject myCheckedPrefab;
    private GameObject myChecked;
    public GameObject lockedPrefab;
    private bool locked = true;

    private void Start()
    {
        myChecked = Instantiate(myCheckedPrefab, this.transform);
        //myChecked.transform.position += new Vector3(3, 0, 0);
        myChecked.SetActive(false);
        int done = PlayerPrefs.GetInt(MainMenu.puzzlePrefKey + index);
        //print(MainMenu.puzzlePrefKey + index + ": " + done);
        if(done > 0)
        {
            myChecked.SetActive(true);
            locked = false;
        }

        int prevDone = PlayerPrefs.GetInt(MainMenu.puzzlePrefKey + (index - 1));
        if(prevDone > 0 || index == 1)
        {
            locked = false;
        }

        if (locked)
        {
            Instantiate(lockedPrefab, transform);
            GetComponent<Image>().color = Color.red;
        }
    }

    public void onClick()
    {
        if (locked)
        {
            return;
        }
        MM.puzzlePickLevel(index);
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("PuzzlePicker"))
        {
            if(go.GetComponent<Image>().color == Color.yellow)
            {
                go.GetComponent<Image>().color = Color.green;
            }
        }
        this.GetComponent<Image>().color = Color.yellow;
    }
}
