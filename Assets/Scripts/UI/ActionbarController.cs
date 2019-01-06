using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionbarController : MonoBehaviour {

    private readonly string classTag = "ActionbarController: ";

    public GameController GC;
    public TileHub tileHub;
    public GameObject tileFather;

    public GameObject[] actionBars;
    public GameObject selectBar;
    public GameObject[] selectBars;
    public GameObject tiler;

    public int[] startActivePrefabs = new int[4];
    private GameObject[] activePrefabs;
    private GameObject[] otherPrefabs;

    [SerializeField]
    private int selectedA = 0;
    private readonly int maxSelectedA = 5;

    private int selectedS = 0;
    private int maxSelectedS;

    //public bool locked = false;
    //public bool noPlace = false;

    public readonly double MAX_CLOSE_TIMER = 1.5f;
    private double closeTimer = 1.5f;
    private bool runningClose = false;

    public AudioController audioCon;

    public int prevSpotIndex = -1;

    private void Start()
    {
        showNameA();

        activePrefabs = new GameObject[4];
        for (int i = 0; i < activePrefabs.Length; i++)
        {
            activePrefabs[i] = tileHub.getPrefab(startActivePrefabs[i]);
            actionBars[i+1].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = activePrefabs[i].GetComponent<SpriteRenderer>().sprite;
        }

        int dic = tileHub.getCount() - activePrefabs.Length - 1; //minus air
        otherPrefabs = new GameObject[dic];

        int index = 0;
        for(int i = 1; i < tileHub.getCount(); i++)
        {
            bool ok = true;
            for (int j = 0; j < activePrefabs.Length; j++)
            {
                if (activePrefabs[j].Equals(tileHub.getPrefab(i)))
                {
                    ok = false;
                }
            }

            if (ok)
            {
                otherPrefabs[index] = tileHub.getPrefab(i);
                selectBars[index].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = otherPrefabs[index].GetComponent<SpriteRenderer>().sprite;
                index++;
            }
        }

        maxSelectedS = otherPrefabs.Length;
    }

    // Update is called once per frame
    void Update () {
        input();
        actionBarSelect();
        SelectBarSelect();
        updateTiler();
	}

    private void input()
    {
        if (!runningClose)
        {
            runningClose = true;
            StartCoroutine(closeDisplayName());
        }

        if (InputController.getInput(InputPurpose.RESET_ACTIONBAR))
        {
            print(classTag + "Reset");
            selectedA = 0;
        }
        if (InputController.getInput(InputPurpose.ACTIONBAR_RIGHT)) //backward
        {
            if (!UtilBools.actionBarLock)
            {
                selectedA++;
                if(selectedA >= maxSelectedA)
                {
                    selectedA = 0;
                }
                tiler.transform.rotation = Quaternion.Euler(0, 0, 0);
                closeTimer = MAX_CLOSE_TIMER;
                showNameA();
            }
        }
        else if (InputController.tryNumbers() != -1)
        {
            if (!UtilBools.actionBarLock)
            {
                selectedA = InputController.tryNumbers();
                closeTimer = MAX_CLOSE_TIMER;
                showNameA();
            }
        }
        if (InputController.getInput(InputPurpose.ACTIONBAR_LEFT))
        {
            if (!UtilBools.actionBarLock)
            {
                selectedA--;
                if (selectedA < 0)
                {
                    selectedA = maxSelectedA - 1;
                }
                tiler.transform.rotation = Quaternion.Euler(0, 0, 0);
                closeTimer = MAX_CLOSE_TIMER;
                showNameA();
            }
        }

        if (InputController.getInput(InputPurpose.SELECTIONBAR) && selectedA != 0)
        {
            selectBar.SetActive(true);
            if (!runningClose)
            {
                showNameS();
            }
        }
        else
        {
            selectBar.SetActive(false);
            if (!runningClose)
            {
                showNameA();
            }
        }

        if(InputController.getInput(InputPurpose.SELECTIONBAR_RIGHT) && selectedA != 0)
        {
            //print("yo!");
            selectedS++;
            if (selectedS >= maxSelectedS)
            {
                selectedS = 0;
            }
            closeTimer = MAX_CLOSE_TIMER;
            showNameS();
        }
        else if (InputController.getInput(InputPurpose.SELECTIONBAR_LEFT) && selectedA != 0)
        {
            //print("yo!");
            selectedS--;
            if (selectedS < 0)
            {
                selectedS = maxSelectedS - 1;
            }
            closeTimer = MAX_CLOSE_TIMER;
            showNameS();
        }
        else if (InputController.getInput(InputPurpose.SELECTIONBAR) && selectedA != 0)
        {
            if(InputController.tryNumbers() != -1)
            {
                selectedS = InputController.tryNumbers();
                closeTimer = MAX_CLOSE_TIMER;
                showNameS();
            }
        }

        //Rotating tiler
        if (InputController.getInput(InputPurpose.TILE_ROTATE_RIGHT))
        {
            rotate(-1);
        }
        if (InputController.getInput(InputPurpose.TILE_ROTATE_LEFT))
        {
            rotate(1);
        }

        //For placing 
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(tiler.transform.position, new Vector3(0,0,-1), 100.0F);

        for (int i = 0; i < hits.Length; i++)
        {
            //print("got a hit");
            RaycastHit2D hit = hits[i];

            if(hit.transform.GetComponent<TileController>() != null)
            {
                //print("got a hit: " + hit.transform.name);
                if (InputController.getInput(InputPurpose.DELETE_TILE) && !UtilBools.tileLock)
                {
                    print(classTag + "Destroying!");
                    TileController tile = hit.transform.GetComponent<TileController>();
                    int setting = 0;
                    string text = "";
                    if(hit.transform.GetComponent<DelayerController>() != null)
                    {
                        setting = hit.transform.GetComponent<DelayerController>().getSetting();
                    }
                    if (hit.transform.GetComponent<SignController>() != null)
                    {
                        text = hit.transform.GetComponent<SignController>().text;
                    }
                    GC.UC.addUndo(new SingleTileUndo(tile.ID, tile.getDir(), tile.spotIndex, setting, text));
                    //print(GC.UC.stringifyRoot());
                    tile.destroyMe();
                    prevSpotIndex = -1;
                }
            }
            else if(hit.transform.GetComponent<FloorTileController>() != null)
            {
                if (InputController.getInput(InputPurpose.PLACE_TILE) && !UtilBools.noPlaceTile)
                {
                    //print("Placing tile: " + UtilBools.noPlaceTile);
                    spawnTile(hit.transform.gameObject);
                    if (!hit.transform.GetComponent<FloorTileController>().busy)
                    {
                        //print("Hit && !busy!");
                        //spawnTile(hit.transform.gameObject);
                    }
                }
            }
        }
    }

    private void showNameA()
    {
        switch (selectedA)
        {
            case 0:
                GC.changeItemName("Select");
                break;
            default:
                GC.changeItemName(tileHub.getName(activePrefabs[selectedA - 1].GetComponent<TileController>().ID));
                break;
        }
    }

    private void showNameS()
    {
        GC.changeItemName(tileHub.getName(otherPrefabs[selectedS].GetComponent<TileController>().ID));
    }

    private IEnumerator closeDisplayName()
    {
        while(closeTimer > 0)
        {
            yield return new WaitForSeconds(0.1f);
            closeTimer -= 0.1f;
        }

        GC.closeItemName();
        closeTimer = 1;
        runningClose = false;
    }

    private void spawnTile(GameObject hit)
    {
        if (tiler.gameObject.activeSelf && hit.GetComponent<FloorTileController>().spotIndex != prevSpotIndex)
        {
            prevSpotIndex = hit.GetComponent<FloorTileController>().spotIndex;
            audioCon.playTilePlacedSFX();

            GameObject a = Instantiate(activePrefabs[selectedA - 1], tiler.transform.position, Quaternion.identity);
            a.GetComponent<TileController>().place(hit);

            int dir = getDir(actionBars[selectedA].transform.GetChild(0).transform);
            a.GetComponent<TileController>().setDir(dir);

            a.transform.parent = tileFather.transform;

            GC.UC.addUndo(new SingleTileUndo(a.GetComponent<TileController>().spotIndex));
            //print(GC.UC.stringifyRoot());
        }
    }

    private void actionBarSelect()
    {
        actionBars[selectedA].transform.GetChild(1).gameObject.SetActive(true);
        for (int i = 0; i < maxSelectedA; i++)
        {
            if(i != selectedA)
            {
                actionBars[i].transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                actionBars[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    private void SelectBarSelect()
    {
        if (InputController.getInput(InputPurpose.SELECTIONBAR) && selectedA != 0)
        {
            selectBars[selectedS].transform.GetChild(1).gameObject.SetActive(true);

            for (int i = 0; i < maxSelectedS; i++)
            {
                if (i != selectedS)
                {
                    selectBars[i].transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
        if (InputController.getInput(InputPurpose.SELECTIONBAR_UP) && selectedA != 0)
        {
            Sprite tempSprite = actionBars[selectedA].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            actionBars[selectedA].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = selectBars[selectedS].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            selectBars[selectedS].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = tempSprite;

            GameObject temp = otherPrefabs[selectedS];
            otherPrefabs[selectedS] = activePrefabs[selectedA - 1];
            activePrefabs[selectedA - 1] = temp;
        }
    }

    public void rotate(int val) //val should be -1, 0 or 1
    {
        //print("Rotating!");
        actionBars[selectedA].transform.GetChild(0).transform.Rotate(0, 0, 90 * val);
        tiler.transform.Rotate(0, 0, 90 * val);
    }

    private void updateTiler()
    {
        if (UtilBools.noPlaceTile)
        {
            tiler.SetActive(false);
            return;
        }

        if(selectedA != 0)
        {
            tiler.SetActive(true);
            tiler.GetComponent<SpriteRenderer>().sprite = actionBars[selectedA].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            tiler.SetActive(false);
        }

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, -10), 100.0F);

        if (hits.Length <= 0)
        {
            tiler.SetActive(false);
            return;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.transform.GetComponent<FloorTileController>() != null)
            {
                tiler.transform.position = hit.transform.position + new Vector3(0, 0, -2);
            }
        }

        Color temp = tiler.GetComponent<SpriteRenderer>().color;
        temp.a = 0.5f;
        tiler.GetComponent<SpriteRenderer>().color = temp;
    }

    private int getDir(Transform trans)
    {
        if (trans.rotation.eulerAngles.z == 0)
        {
            return 0;
        }
        else if (trans.rotation.eulerAngles.z == 90)
        {
            return 1;
        }
        else if (trans.rotation.eulerAngles.z == 180 || trans.rotation.eulerAngles.z == -180)
        {
            return 2;
        }
        else if (trans.rotation.eulerAngles.z == 270 || trans.rotation.eulerAngles.z == -90)
        {
            return 3;
        }
        return 0;
    }

    public int getSelectedA()
    {
        return selectedA;
    }

    public void setSelectedA(int value)
    {
        selectedA = value;
    }
}
