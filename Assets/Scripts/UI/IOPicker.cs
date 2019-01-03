using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOPicker : MonoBehaviour {

    public Camera cam;
    public GameObject IN;
    public GameObject OUT;
    public GameObject NULL;
    public GameObject CARRY;
    public GameObject RESET;

    public GameObject accept;

    GameObject currentObj;

    public bool isPuzzle = true;

    private TileHub hub;

    private void Start()
    {
        hub = GameObject.Find("TileHub").GetComponent<TileHub>();
        closeAll();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isPuzzle)
        {
            //print("yo!");
            bool ok = false;

            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(cam.ScreenToWorldPoint(Input.mousePosition), new Vector3(0,0,-1), 100.0F);

            //print(hits.Length);
            foreach(RaycastHit2D hit in hits)
            {
                if(hit.transform.GetComponent<TileController>() != null)
                {
                    int id = hit.transform.GetComponent<TileController>().ID;
                    if(!hub.IOPickerException(id))
                    {
                        ok = true;
                        transform.position = hit.transform.position;
                        currentObj = hit.transform.gameObject;
                    }
                }
            }

            if (ok && IN.activeSelf)
            {
                closeAll();
                currentObj = null;
            }
            else if (ok)
            {
                IN.SetActive(true);
                OUT.SetActive(true);
                NULL.SetActive(true);
                CARRY.SetActive(true);
                RESET.SetActive(true);
                accept.SetActive(true);
                if(currentObj.GetComponent<TileController>().myLabel == TileController.label.IN)
                {
                    accept.transform.position = IN.transform.position + new Vector3(0, 0, -1);
                }
                else if (currentObj.GetComponent<TileController>().myLabel == TileController.label.OUT)
                {
                    accept.transform.position = OUT.transform.position + new Vector3(0, 0, -1);
                }
                else if (currentObj.GetComponent<TileController>().myLabel == TileController.label.CARRY)
                {
                    accept.transform.position = CARRY.transform.position + new Vector3(0, 0, -1);
                }
                else if (currentObj.GetComponent<TileController>().myLabel == TileController.label.RESET)
                {
                    accept.transform.position = RESET.transform.position + new Vector3(0, 0, -1);
                }
                else
                {
                    accept.transform.position = NULL.transform.position + new Vector3(0, 0, -1);
                }
            }
            else
            {
                closeAll();
                currentObj = null;
            }
        }
        else if (Input.GetMouseButtonDown(0) && !isPuzzle)
        {
            bool ok = false;
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(cam.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, -1), 100.0F);

            //print(hits.Length);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.GetComponent<IO>() != null)
                {
                    ok = true;
                }
            }
            if (!ok)
            {
                closeAll();
            }
        }
    }

    private void closeAll()
    {
        IN.SetActive(false);
        OUT.SetActive(false);
        NULL.SetActive(false);
        CARRY.SetActive(false);
        RESET.SetActive(false);
        accept.SetActive(false);
    }

    public void changeLabel(TileController.label newLabel)
    {
        //print("change!");
        currentObj.GetComponent<TileController>().myLabel = newLabel;
        switch (newLabel)
        {
            case TileController.label.IN:
                accept.transform.position = IN.transform.position + new Vector3(0, 0, -1);
                break;
            case TileController.label.OUT:
                accept.transform.position = OUT.transform.position + new Vector3(0, 0, -1);
                break;
            case TileController.label.NULL:
                accept.transform.position = NULL.transform.position + new Vector3(0, 0, -1);
                break;
            case TileController.label.CARRY:
                accept.transform.position = CARRY.transform.position + new Vector3(0, 0, -1);
                break;
            case TileController.label.RESET:
                accept.transform.position = RESET.transform.position + new Vector3(0, 0, -1);
                break;
        }
    }
}
