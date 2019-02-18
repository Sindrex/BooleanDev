using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentPlacerController : MonoBehaviour {

    public GameController GC;
    public ComponentUI CUI;
    public UndoController UC;
    public ComponentSave myComp;

    public int spotIndex = -1;
    //Vector3 currentPos;

    public GameObject wrapper;
    public GameObject backdrop;

    // Use this for initialization
    void Start () {
        setBackdrop();
        GC.AC.selectedA = 0;
        UtilBools.actionBarLock = true;
        UtilBools.selectLock = true;
    }
	
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
    }

    public void closePlacer()
    {
        UtilBools.actionBarLock = false;
        UtilBools.selectLock = false;
        Destroy(this.gameObject);
    }

    public void setBackdrop()
    {
        //TileHub hub = GameObject.Find("Manager").GetComponent<TileHub>();

        //backdrop.transform.position = this.transform.position;
        backdrop.transform.localScale = new Vector3(myComp.length, myComp.height, 1);

        float x = this.transform.position.x;
        float y = this.transform.position.y;

        x -= 0.5f * ((myComp.length / 2.0f) - 1);
        y += 0.5f * ((myComp.height / 2.0f) - 1);

        backdrop.transform.position = new Vector3(x - (myComp.length / 2.0f), y + (myComp.height / 2.0f), -2);
    }

    private void placeComp()
    {
        //undo
        int[] spotIndexes = new int[myComp.tileIDs.Length];

        //place at currentPos
        int row = 0;
        int column = myComp.length;
        List<GameObject> tiles = new List<GameObject>();
        List<GameObject> floorTiles = new List<GameObject>();

        for (int i = myComp.tileIDs.Length - 1; i >= 0; i--)
        {
            int mySpotIndex = spotIndex - (myComp.length - column) - (GC.length * row);
            spotIndexes[i] = mySpotIndex;

            //print("i: " + i + " - ID: " + comp.tileIDs[i]);
            if (myComp.tileIDs[i] != 0)
            {
                //print("spotIndex: " + spotIndex + ", New: " + mySpotIndex);
                bool ok = true;
                if (UtilBools.isPuzzle)
                {
                    ok = false;
                    foreach (int n in GC.ePC.myPuzzle.allowedTiles)
                    {
                        if(myComp.tileIDs[i] == n)
                        {
                            ok = true;
                        }
                    }
                }
                if (ok)
                {
                    GameObject prefab = GC.spawnSingle(mySpotIndex, myComp.tileIDs[i], myComp.tileDIRs[i], myComp.tilePower[i], myComp.tileSetting[i], myComp.signTexts[i], myComp.tileLabels[i]);

                    tiles.Add(prefab);
                }

                if (row == myComp.height)
                {
                    return;
                }
            }

            floorTiles.Add(GC.floorList[mySpotIndex]);

            column--;
            if (column == 0)
            {
                column = myComp.length;
                row++;
            }
        }
        if(tiles.Count <= 0)
        {
            return;
        }
        CUI.createOverlay(myComp, tiles, floorTiles);
        UC.addUndo(new MultipleTilesUndo(spotIndexes));
    }
}
