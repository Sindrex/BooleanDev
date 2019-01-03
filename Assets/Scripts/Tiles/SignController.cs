using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignController : TileController {

    public string text = "";
    public SignManager ui;

    override
    protected void Start()
    {
        ui = GameObject.Find("SignManager").GetComponent<SignManager>();
    }

    private void OnMouseOver()
    {
        if (!ui.pinned)
        {
            if (InputController.getInput(InputPurpose.INTERACT_TILE) && !ui.signHover.activeSelf)
            {
                ui.signHover.SetActive(true);
                ui.signText.text = text;
                ui.signHover.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, ui.signHover.transform.position.z);
                ui.currentSign = this;
            }
            else if(InputController.getInput(InputPurpose.INTERACT_TILE) && ui.signHover.activeSelf)
            {
                ui.signHover.SetActive(false);
                ui.signText.text = "";
                ui.signHover.transform.position = ui.startPos;
            }
        }

        /*
        if (!ui.pinned)
        {
            ui.signHover.SetActive(true);
            ui.signText.text = text;
            ui.signHover.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, ui.signHover.transform.position.z);
            ui.currentSign = this;
        }*/
    }

    private void OnMouseExit()
    {
        /*
        if (!ui.pinned)
        {
            StartCoroutine(waitClose());
        }*/
    }

    private IEnumerator waitClose()
    {
        yield return new WaitForSeconds(0.75f);
        if(!ui.pinned)
        {
            ui.signHover.SetActive(false);
            ui.signText.text = "";
            ui.signHover.transform.position = ui.startPos;
        }
    }

    override
    public void destroyMe()
    {
        if (locked)
        {
            return;
        }

        //print("DestroyMe: " + this.name);
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = 0;
        }
        if (homeObj != null)
        {
            homeObj.GetComponent<FloorTileController>().busy = false;
            GC.tiles[spotIndex] = null;
        }
        Destroy(this.gameObject);
        ui.closeSign();
        StartCoroutine(waitClose());
    }

    override
    protected void rotateInput()
    {
        return;
    }

    override
    public bool hasOutput(int a)
    {
        return false;
    }

    override
    protected void tryPower(bool b)
    {
        return;
    }
}
