﻿using System.Collections;
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

    public GameObject[] activePrefabs;

    public int firstUnavailableA = 2;
    public int selectedA = 0;
    public readonly int maxSelectedA = 10;

    //placing
    public GameObject tiler;

    //Showname
    public readonly double MAX_CLOSE_TIMER = 1.0f;
    private double closeTimer = 1.0f;
    private bool showName = false;

    public AudioController audioCon;

    public int prevSpotIndex = -1;

    private void Start()
    {
        audioCon = GameObject.Find("AudioMixer").GetComponent<AudioController>();

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
        if (Input.GetMouseButtonUp(0))
        {
            prevSpotIndex = -1;
        }
        if (InputController.getInput(InputPurpose.RESET_ACTIONBAR) && !UtilBools.actionBarLock)
        {
            print(classTag + "Reset");
            selectedA = 0;
            actionBarSelect();
        }

        if (InputController.getInput(InputPurpose.ACTIONBAR_RIGHT)) //backward "scroll"
        {
            if (!UtilBools.actionBarLock)
            {
                selectedA++;
                if(selectedA >= maxSelectedA)
                {
                    selectedA = 0;
                }
                closeTimer = MAX_CLOSE_TIMER;
                showNameA();
                actionBarSelect();
            }
        }
        else if (InputController.getInput(InputPurpose.ACTIONBAR_LEFT)) //forwards "scroll"
        {
            if (!UtilBools.actionBarLock)
            {
                selectedA--;
                if (selectedA < 0)
                {
                    selectedA = maxSelectedA - 1;
                }
                closeTimer = MAX_CLOSE_TIMER;
                showNameA();
                actionBarSelect();
            }
        }
        else if (InputController.tryNumbers() != -1) //using numbers
        {
            if (!UtilBools.actionBarLock)
            {
                selectedA = InputController.tryNumbers();
                closeTimer = MAX_CLOSE_TIMER;
                showNameA();
                actionBarSelect();
            }
        }

        //open selectbar
        if (InputController.getInput(InputPurpose.SELECTIONBAR) && !UtilBools.selectionbarLock)
        {
            toggleSelectionBar(!selectbar.activeSelf); //toggle
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
        RaycastHit2D[] hits = Physics2D.RaycastAll(tiler.transform.position, new Vector3(0,0,-1), 100.0F);
        for (int i = 0; i < hits.Length; i++)
        {
            //print("got a hit");
            RaycastHit2D hit = hits[i];
            TileController tile = hit.transform.GetComponent<TileController>();
            if (tile != null)
            {
                //print("got a hit: " + hit.transform.name);
                if (InputController.getInput(InputPurpose.DELETE_TILE) && !UtilBools.tileLock)
                {
                    //print(classTag + "Destroying!");
                    int setting = 0;
                    string text = "";
                    if(hit.transform.GetComponent<DelayerController>() != null)
                    {
                        setting = hit.transform.GetComponent<DelayerController>().setting;
                    }
                    if (hit.transform.GetComponent<SignController>() != null)
                    {
                        text = hit.transform.GetComponent<SignController>().text;
                    }
                    GC.UC.addUndo(new SingleTileUndo(tile.ID, tile.getDir(), tile.spotIndex, setting, text));
                    //print(GC.UC.stringifyRoot());
                    tile.destroyMe(false);
                    prevSpotIndex = -1;
                }
                else if (InputController.getInput(InputPurpose.PLACE_TILE) && !UtilBools.noPlaceTile)
                {
                    //print("Placing tile: " + UtilBools.noPlaceTile);
                    spawnTile(tile.homeObj);
                }
            }
            else if(hit.transform.GetComponent<FloorTileController>() != null)
            {
                if (InputController.getInput(InputPurpose.PLACE_TILE) && !UtilBools.noPlaceTile)
                {
                    //print("Placing tile: " + UtilBools.noPlaceTile);
                    spawnTile(hit.transform.gameObject);
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
        GC.audioMixer.playButtonSFX();
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

    public void showNameA(int index)
    {
        switch (index)
        {
            case 0:
                GC.changeItemName("Select");
                break;
            case 1:
                GC.changeItemName("Move");
                break;
            default:
                GC.changeItemName(tileHub.getName(activePrefabs[index - firstUnavailableA].GetComponent<TileController>().ID));
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

            GameObject a = Instantiate(activePrefabs[selectedA - firstUnavailableA], tiler.transform.position, Quaternion.identity);
            if (a.GetComponent<TileController>().place(hit))
            {
                audioCon.playTilePlacedSFX();

                int dir = getDir(actionbars[selectedA].transform);
                a.GetComponent<TileController>().setDir(dir);

                a.transform.parent = tileFather.transform;

                GC.UC.addUndo(new SingleTileUndo(a.GetComponent<TileController>().spotIndex));
                //print(GC.UC.stringifyRoot());

                GC.ePC.PT.PlaceAction(prevSpotIndex, a.GetComponent<TileController>().ID);
            }
        }
    }

    public void actionBarSelect()
    {
        //Position the select right + reset rotation
        GC.audioMixer.playButtonSFX();
        tiler.transform.rotation = Quaternion.Euler(0, 0, 0);
        selectedTileA.transform.position = actionbars[selectedA].transform.position;
        foreach(GameObject go in actionbars)
        {
            go.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void rotate(int val) //val should be -1, 0 or 1 (left, center, right)
    {
        //print("Rotating!");
        GC.audioMixer.playButtonSFX();
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
            if (activePrefabs[selectedA - firstUnavailableA].GetComponent<TileController>().ID == 0)
            {
                tiler.SetActive(false);
                return;
            }
            tiler.SetActive(true);
            tiler.GetComponent<SpriteRenderer>().sprite = actionbars[selectedA].GetComponent<Image>().sprite;
        }
        else
        {
            tiler.SetActive(false);
        }

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, -10), 100.0F);

        //did we hit anything?
        if (hits.Length <= 0)
        {
            tiler.SetActive(false);
            return;
        }

        //did we hit tiles/floortiles?
        foreach (RaycastHit2D hit in hits)
        {
            if (!hit.transform.CompareTag("FloorTile") && !hit.transform.CompareTag("Tile"))
            {
                tiler.SetActive(false);
                return;
            }
        }

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            TileController tile = hit.transform.GetComponent<TileController>();
            if (hit.transform.GetComponent<FloorTileController>() != null)
            {
                tiler.transform.position = hit.transform.position + new Vector3(0, 0, -2);
            }
            else if(tile != null)
            {
                if(tile.homeObj != null)
                {
                    tiler.transform.position = tile.homeObj.transform.position + new Vector3(0, 0, -2);
                    //tile.homeObj.GetComponent<BoxCollider2D>().enabled = true;
                }
            }
        }

        //color
        Color temp = tiler.GetComponent<SpriteRenderer>().color;
        temp.a = 0.5f;
        tiler.GetComponent<SpriteRenderer>().color = temp;
    }

    //help method
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

    //help method
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
