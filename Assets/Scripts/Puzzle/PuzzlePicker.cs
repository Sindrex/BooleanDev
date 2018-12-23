using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzlePicker : MonoBehaviour
{
    public int index;
    public MainMenu MM;

    public void onClick()
    {
        MM.pickLevel(index);
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
