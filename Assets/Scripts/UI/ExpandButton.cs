using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpandButton : MonoBehaviour {

    //also for expandUI's buttons, tracking ints++

    public GameController GC;
    public GameObject expandUI;

    public int[] dirs = new int[4];

    public Text cDirUP;
    public Text cDirLEFT;
    public Text cDirDOWN;
    public Text cDirRIGHT;

    private void FixedUpdate()
    {
        cDirUP.text = dirs[0].ToString();
        cDirLEFT.text = dirs[1].ToString();
        cDirDOWN.text = dirs[2].ToString();
        cDirRIGHT.text = dirs[3].ToString();
    }

    void OnMouseDown()
    {
        if (expandUI.activeSelf)
        {
            expandUI.SetActive(false);
        }
        else
        {
            expandUI.SetActive(true);
        }
        dirs = new int[4];
    }

    public void startExpand()
    {
        GC.doExpand = new int[4];
        for(int i = 0; i < dirs.Length; i++)
        {
            if(dirs[i] > 0)
            {
                //print(i + "/" + GC.doExpand.Length + "/" + dirs.Length);
                GC.doExpand[i] = dirs[i];
            }
        }
    }

    public void accept()
    {
        expandUI.SetActive(false);
        startExpand();
    }
    public void decline()
    {
        expandUI.SetActive(false);
        dirs = new int[4];
    }

    public void addUP()
    {
        dirs[0]++;
    }
    public void addLEFT()
    {
        dirs[1]++;
    }
    public void addDOWN()
    {
        dirs[2]++;
    }
    public void addRIGHT()
    {
        dirs[3]++;
    }

    public void minUP()
    {
        dirs[0]--;
    }
    public void minLEFT()
    {
        dirs[1]--;
    }
    public void minDOWN()
    {
        dirs[2]--;
    }
    public void minRIGHT()
    {
        dirs[3]--;
    }
}
