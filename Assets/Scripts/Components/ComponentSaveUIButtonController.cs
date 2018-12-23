using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComponentSaveUIButtonController : MonoBehaviour {

    public ComponentUI CUI;
    public ComponentSave myComp;

    public Vector3 tipOffset;

    public void setText()
    {
        transform.GetChild(0).GetComponent<Text>().text = myComp.name;
    }

    public void setText(string text)
    {
        transform.GetChild(0).GetComponent<Text>().text = text;
    }

    public void loadComp()
    {
        CUI.loadComp(myComp, transform.position);
    }

    public void removeComp()
    {
        SaveLoadComp.removeSave(myComp);
        CUI.updateUI();
        CUI.tip.SetActive(false);
    }

    public void newComp()
    {
        CUI.openConfirmComp();
    }

    private void OnMouseEnter()
    {
        if(myComp.tileIDs.Length > 0)
        {
            //print("mousing over!");
            CUI.tip.SetActive(true);
            CUI.tip.transform.position = this.transform.position + tipOffset;
            CUI.tipName.text = myComp.name;

            int number = 0;
            foreach(int i in myComp.tileIDs)
            {
                if(i != 0)
                {
                    number++;
                }
            }

            CUI.tipStats.text = myComp.length + "\n" +
                                myComp.height + "\n" +
                                number + "\n" +
                                "yyyy.mm.dd";
        }
    }

    private void OnMouseExit()
    {
        if (myComp.tileIDs.Length > 0)
        {
            CUI.tip.SetActive(false);
        }
    }
}
