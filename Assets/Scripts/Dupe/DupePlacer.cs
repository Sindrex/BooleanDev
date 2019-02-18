using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DupePlacer : MonoBehaviour {

    public DupeController DC;
    public GameObject wrapper;

	// Use this for initialization
	void Start () {
        UtilBools.actionBarLock = true;
        UtilBools.selectLock = true;
    }
	/*
	// Update is called once per frame
	void Update () {
        if (InputController.getInput(InputPurpose.PLACE_TILE) || InputController.getInput(InputPurpose.INTERACT_TILE))
        {
            closePlacer();
        }

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, -10), 100.0F);

        if (hits.Length <= 0)
        {
            wrapper.SetActive(false);
            if (InputController.getInput(InputPurpose.PLACE_TILE) || InputController.getInput(InputPurpose.INTERACT_TILE))
            {
                closePlacer();
            }
            return;
        }
        wrapper.SetActive(true);
        if (InputController.getInput(InputPurpose.PLACE_TILE))
        {
            if (spotIndex < 0)
            {
                closePlacer();
            }

            //print("Placing comp!");

            placeComp();
        }

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.transform.GetComponent<FloorTileController>() != null)
            {
                //currentPos = hit.transform.position;
                spotIndex = hit.transform.GetComponent<FloorTileController>().spotIndex;
                transform.position = hit.transform.position + new Vector3(0, 0, -1);
            }
        }
    }*/

    public void closePlacer()
    {
        UtilBools.actionBarLock = false;
        UtilBools.selectLock = false;
        Destroy(this.gameObject);
    }
}
