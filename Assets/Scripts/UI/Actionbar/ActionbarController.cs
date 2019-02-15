using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionbarController : MonoBehaviour {

    private readonly string classTag = "ActionbarController: ";

    public GameController GC;
    public TileHub tileHub;
    public GameObject tileFather;

    //Actionbar
    public GameObject selectedTileA;
    public GameObject selectedTileS;
    public GameObject selectionbarPrefab;
    public GameObject selectionbarRowPrefab;
    public float actionTileSize;

    public GameObject actionbar;
    public GameObject[] actionbars;
    public int selectbarMaxItemsPerRow = 10;
    public GameObject selectbar;
    //public GameObject[] selectbars;

    public GameObject[] activePrefabs;

    public int firstUnavailableA = 2;
    public int selectedA = 0;
    public readonly int maxSelectedA = 10;

    public GameObject tiler;

    //Showname
    public readonly double MAX_CLOSE_TIMER = 1.0f;
    private double closeTimer = 1.0f;
    private bool showName = false;

    public AudioController audioCon;

    public int prevSpotIndex = -1;

    private void Start()
    {
        closeTimer = MAX_CLOSE_TIMER;
        showNameA();

        activePrefabs = new GameObject[maxSelectedA - firstUnavailableA];

        //Fill up actionbar
        for(int i = 0; i < activePrefabs.Length; i++)
        {
            int tileId = i + 1; //from 1-10 (exclude air)
            bool useAirPuzzle = puzzleUseAir(tileId);

            bool banned = tileHub.actionbarBanned.Contains(tileId);
            if (!banned)
            {
                //print("Actionbar: Adding tileId: " + tileId);
                //print("b: " + b + ", Length: " + activePrefabs.Length);
                if(useAirPuzzle)
                {
                    activePrefabs[i] = tileHub.getPrefab(0);
                    actionbars[i + firstUnavailableA].GetComponent<Image>().color = Color.black;
                    actionbars[i + firstUnavailableA].GetComponent<ActionBarItem>().tileId = 0;
                }
                else
                {
                    activePrefabs[i] = tileHub.getPrefab(tileId);
                    actionbars[i + firstUnavailableA].GetComponent<Image>().sprite = activePrefabs[i].GetComponent<SpriteRenderer>().sprite;
                    actionbars[i + firstUnavailableA].GetComponent<ActionBarItem>().tileId = tileId;
                }
            }
        }

        //Create and fill selectionbar for each tile there is (except banned ones)
        int index = 0;
        int row = 0;
        Instantiate(selectionbarRowPrefab, selectbar.transform);
        for(int id = 0; id < tileHub.tilePrefabs.Length; id++)
        {
            GameObject tile = tileHub.getPrefab(id);

            bool useAirPuzzle = puzzleUseAir(id);

            bool banned = tileHub.actionbarBanned.Contains(id);
            if (!banned)
            {
                //position
                if (index >= selectbarMaxItemsPerRow)
                {
                    index = 0;
                    row++;
                    GameObject rowPrefab = Instantiate(selectionbarRowPrefab, selectbar.transform);
                    rowPrefab.transform.localPosition += new Vector3(0, actionTileSize * row, 0);
                }

                GameObject prefab = Instantiate(selectionbarPrefab, selectbar.transform);
                prefab.transform.localPosition += new Vector3(actionTileSize * index, actionTileSize * row, 0);
                index++;

                prefab.GetComponent<SelectbarController>().AC = this;
                if (useAirPuzzle)
                {
                    prefab.GetComponent<SelectbarController>().tileId = 0;
                    prefab.GetComponent<Image>().color = Color.black;
                }
                else
                {
                    prefab.GetComponent<SelectbarController>().tileId = id;
                    prefab.GetComponent<Image>().sprite = tile.GetComponent<SpriteRenderer>().sprite;
                }
            }
        }
        selectbar.SetActive(false);
        selectedTileS.SetActive(false);
    }

    private bool puzzleUseAir(int id)
    {
        bool ok = true;
        int[] allowedTiles = tileHub.allowedTiles;
        if (allowedTiles.Length > 0)
        {
            //print("yo");
            for (int k = 0; k < allowedTiles.Length; k++)
            {
                if (id == allowedTiles[k])
                {
                    ok = false;
                }
            }
        }
        else
        {
            ok = false;
        }
        return ok;
    }

    // Update is called once per frame
    void Update () {
        input();
        updateTiler();

        if (showName)
        {
            showName = false;
            StartCoroutine(closeDisplayName());
        }
    }

    private void input()
    {
        if (InputController.getInput(InputPurpose.RESET_ACTIONBAR))
        {
            print(classTag + "Reset");
            selectedA = 0;
            actionBarSelect();
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
                actionBarSelect();
            }
        }
        else if (InputController.getInput(InputPurpose.ACTIONBAR_LEFT))
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
                actionBarSelect();
            }
        }
        else if (InputController.tryNumbers() != -1)
        {
            if (!UtilBools.actionBarLock)
            {
                selectedA = InputController.tryNumbers();
                closeTimer = MAX_CLOSE_TIMER;
                showNameA();
                actionBarSelect();
            }
        }

        if (InputController.getInput(InputPurpose.SELECTIONBAR) && !UtilBools.selectionbarLock)
        {
            toggleSelectionBar(!selectbar.activeSelf);
        }

        //Rotating tiler
        if (InputController.getInput(InputPurpose.TILE_ROTATE_RIGHT))
        {
            rotate(-1);
        }
        else if (InputController.getInput(InputPurpose.TILE_ROTATE_LEFT))
        {
            rotate(1);
        }

        //For placing and deleting
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
                    //print(classTag + "Destroying!");
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

    public void toggleSelectionBar(bool state)
    {
        selectbar.SetActive(state);
        selectedTileS.SetActive(state);
        UtilBools.selectLock = state;
        UtilBools.noPlaceTile = state;
    }

    private void showNameA()
    {
        switch (selectedA)
        {
            case 0:
                GC.changeItemName("Select");
                break;
            case 1:
                GC.changeItemName("Move");
                break;
            default:
                GC.changeItemName(tileHub.getName(activePrefabs[selectedA - firstUnavailableA].GetComponent<TileController>().ID));
                break;
        }
        StopAllCoroutines();
        closeTimer = MAX_CLOSE_TIMER;
        showName = true;
    }

    private IEnumerator closeDisplayName()
    {
        while(closeTimer > 0)
        {
            yield return new WaitForSeconds(0.1f);
            closeTimer -= 0.1f;
        }
        
        GC.closeItemName();
        closeTimer = MAX_CLOSE_TIMER;
        //print("yo!");
    }

    private void spawnTile(GameObject hit)
    {
        if (tiler.gameObject.activeSelf && hit.GetComponent<FloorTileController>().spotIndex != prevSpotIndex)
        {
            if (activePrefabs[selectedA - firstUnavailableA].GetComponent<TileController>().ID == 0)
            {
                return;
            }
            prevSpotIndex = hit.GetComponent<FloorTileController>().spotIndex;
            audioCon.playTilePlacedSFX();

            GameObject a = Instantiate(activePrefabs[selectedA - firstUnavailableA], tiler.transform.position, Quaternion.identity);
            a.GetComponent<TileController>().place(hit);

            int dir = getDir(actionbars[selectedA].transform);
            a.GetComponent<TileController>().setDir(dir);

            a.transform.parent = tileFather.transform;

            GC.UC.addUndo(new SingleTileUndo(a.GetComponent<TileController>().spotIndex));
            //print(GC.UC.stringifyRoot());
        }
    }

    private void actionBarSelect()
    {
        //Position the select right + reset rotation
        selectedTileA.transform.position = actionbars[selectedA].transform.position;
        foreach(GameObject go in actionbars)
        {
            go.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void rotate(int val) //val should be -1, 0 or 1
    {
        //print("Rotating!");
        actionbars[selectedA].transform.Rotate(0, 0, 90 * val);
        tiler.transform.Rotate(0, 0, 90 * val);
    }

    private void updateTiler()
    {
        if (UtilBools.noPlaceTile)
        {
            tiler.SetActive(false);
            return;
        }

        if(selectedA != 0 && selectedA != 1)
        {
            tiler.SetActive(true);
            tiler.GetComponent<SpriteRenderer>().sprite = actionbars[selectedA].GetComponent<Image>().sprite;
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

    //helping method
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

    public bool goArrayContains(GameObject[] array, GameObject item)
    {
        foreach(GameObject i in array)
        {
            if(i.Equals(item))
            {
                return true;
            }
        }
        return false;
    }
}
