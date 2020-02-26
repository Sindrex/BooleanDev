using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignController : TileController {

    public string text = "";
    public SignManager ui;
    public Vector3 offset;

    private void Start()
    {
        ui = GameObject.Find("SignManager").GetComponent<SignManager>();
    }

    private void OnMouseOver()
    {
        //print("Clicked ma boi sign");
        if (!ui.editing)
        {
            //print("Clicked ma boi sign 2");
            if (InputController.getInput(InputPurpose.INTERACT_TILE))
            {
                print("Clicked sign: " + this.gameObject.name);
                AC.audioCon.playTilePlacedSFX();
                if (!ui.signObj.activeSelf)
                {
                    ui.signObj.SetActive(true);
                    ui.signText.text = text;
                    //ui.signHover.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, ui.signHover.transform.position.z);
                    ui.currentSignTile = this;
                }
                else if(ui.currentSignTile != null)
                {
                    ui.signText.text = text;
                    ui.currentSignTile = this;
                }
                else
                {
                    ui.signObj.SetActive(false);
                    ui.signText.text = "";
                    ui.signObj.transform.position = ui.startPos;
                    ui.currentSignTile = null;
                }
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

    private IEnumerator waitClose()
    {
        yield return new WaitForSeconds(0.75f);
        if(!ui.editing)
        {
            ui.signObj.SetActive(false);
            ui.signText.text = "";
            ui.signObj.transform.position = ui.startPos;
        }
    }

    override
    public bool destroyMe(bool addUndo)
    {
        if (locked)
        {
            return false;
        }

        if (homeObj != null)
        {
            homeObj.GetComponent<FloorTileController>().busy = false;
            homeObj.GetComponent<BoxCollider2D>().enabled = true; //new
            GC.tiles[spotIndex] = null;
        }

        if (addUndo)
        {
            int setting = 0;
            string signText = "";
            if (this.GetComponent<DelayerController>() != null)
            {
                setting = this.GetComponent<DelayerController>().setting;
            }
            else if (this.GetComponent<SignController>() != null)
            {
                signText = this.GetComponent<SignController>().text;
            }
            GC.UC.addUndo(new SingleTileUndo(this.ID, this.dir, this.spotIndex, setting, signText));
        }

        Destroy(this.gameObject);
        ui.closeSign();
        StartCoroutine(waitClose());
        return true;
    }
}
