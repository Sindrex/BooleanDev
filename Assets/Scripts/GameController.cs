using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class GameController : MonoBehaviour {

    private readonly string classTag = "GameController: ";

    public GameObject tileFather;
    public GameObject selectedFather;
    public GameObject floorFather;
    public GameObject floorTilePrefab;

    public int length; //save this
    public int height; //save this
    public Vector2 start;

    //[HideInInspector]
    public GameObject[] tiles; //save this
    [HideInInspector]
    public int[] tileIDs;
    [HideInInspector]
    public int[] tileDIRs;
    [HideInInspector]
    public bool[] tilePower;
    [HideInInspector]
    public int[] tileSetting;
    [HideInInspector]
    public string[] signTexts;
    public List<GameObject> floorList = new List<GameObject>();

    private int index = 0;

    private bool loaded = false;

    //GUI and select
    //public GameObject paused; //@DEPRECATED
    //public PausedController pausedController; //@DEPRECATED
    public SystemController SC;
    public ActionbarController AC;
    public GameObject cam;

    [SerializeField]
    private int startIndex = -1;
    public List<GameObject> selectedTiles = new List<GameObject>();
    public List<GameObject> selectedFloor = new List<GameObject>();
    public GameObject selectorPrefab;
    public GameObject moverPrefab;
    public GameObject selectorBackdrop;
    private GameObject mover;
    public int selectedLength;
    public int selectedHeight;
    private int prevSelectedIndex;
    public List<int> selectedTileIndex = new List<int>();

    public GameObject duperPrefab;
    private GameObject duper;

    //SelectedCache
    public int startCachedSelectedTiles;
    public int cacheMax;
    public GameObject selectedCache;
    public List<GameObject> cacheList;

    //For expanding
    public int[] doExpand = new int[4];
    private bool[] doneExpanding = new bool[4];
    private int expandFrame = 0;
    private int expandTile = 0;

    //UI
    public Text itemName;
    public SignManager signManager;
    public TileHub tileHub;
    public ComponentUI compUI;
    public bool changedLocked;

    //Puzzle
    //public GameObject puzzlePlay;
    //public GameObject puzzleObjectve;
    //public GameObject puzzleHint;
    public GameObject ioPicker;
    //public PuzzleController PC; //@DEPRECATED

    //EOTP puzzle
    public EOTP_PuzzleController ePC;
    //public PuzzleTruthTable puzzleTruth;

    //Undos
    public UndoController UC;
    private int[] startIndexesSelect;
    private bool recentDupe = false;

    public GameController()
    {
        loaded = false;
        index = 0; 
    }

	// Use this for initialization
	void Start () {

        length = 15;
        height = 10;
        bool newSave = false;
        EOTP_PuzzleCreator myPuzzle = null;

        if(Game.current != null)
        {
            length = Game.current.length;
            height = Game.current.height;
            tileIDs = Game.current.tileIDs;
            tileDIRs = Game.current.tileDIRs;
            tilePower = Game.current.tilePower;
            tileSetting = Game.current.tileSetting;
            signTexts = Game.current.signTexts;

            newSave = Game.current.newSave;
            myPuzzle = Game.current.puzzle;

            Game.current.dateLastPlayed = System.DateTime.Today.ToString("dd/MM/yyyy");
            //print(Game.current.dateCreated + " / " + Game.current.dateLastPlayed); 
            setWorldSettings();
        }
        else
        {
            print(classTag + "Start():Game.current is null, creating temp");
            newSave = true;
            Game.current = new Game(null);
            Game.current.length = length;
            Game.current.height = height;
        }

        cacheList = new List<GameObject>();
        doneExpanding = new bool[4];

        //if this isnt a puzzle
        if(myPuzzle == null)
        {
            //Create floor
            if (!newSave)
            {
                print(classTag + "Loading old floor!");
                Vector2 current = start;
                //make floor
                for (int i = 0; i < height; i++)//For each unity in height
                {
                    for (int j = 0; j < length; j++)//For each unit in length
                    {
                        GameObject floor = Instantiate(floorTilePrefab, current, Quaternion.identity);
                        floor.transform.parent = floorFather.transform;
                        floor.GetComponent<FloorTileController>().spotIndex = index;
                        index++;

                        floorList.Add(floor);

                        current += new Vector2(1.6f, 0);
                    }
                    current = start + new Vector2(0, -1.6f) * (i + 1);
                }
                if(tileIDs.Length > 0)
                {
                    print(classTag + "spawning...");
                    spawn();
                }
            }
            else
            {
                if (Game.current != null)
                {
                    print(classTag + "NewSave is now false");
                    Game.current.newSave = false;
                }

                //print("GameController:Start():EOTP-testing: Not making new floor this time!");
                makeNewFloor(length, height);
            }
            ePC.close();
            ioPicker.gameObject.SetActive(false);
            ioPicker.GetComponent<IOPicker>().isPuzzle = false;
            UtilBools.isPuzzle = false;
        }
        else //it is a puzzle
        {
            print(classTag + myPuzzle.name);
            if(myPuzzle.id <= 0)
            {
                //First puzzle
                ePC.close();
            }
            length = myPuzzle.length;
            height = myPuzzle.height;
            makeNewFloor(length, height);
            ePC.GC = this;
            ePC.setupPuzzle(myPuzzle);
            ioPicker.gameObject.SetActive(false);
            tileIDs = new int[length * height];
            UtilBools.isPuzzle = true;
            SC.close();
            //pausedController.worldOptionsButton.interactable = false;
            //pausedController.saveButton.interactable = false;
        }

        //Create the selectedtiles beforehand and put in cache
        createSelectedTiles(startCachedSelectedTiles);

        itemName.gameObject.SetActive(false);
        //Cam in middle
        cam.transform.position = new Vector3(1.6f*(length / 2), -1.6f * (height/2), -10);
    }

    private void setWorldSettings()
    { 
        //print("Utilbools: ABL: " + UtilBools.actionBarLock + ", CML: " + UtilBools.camMoveLock + ", NPT: " + UtilBools.noPlaceTile + ", SL: " + UtilBools.selectLock + ", TL: " + UtilBools.tileLock);
        UtilBools.setAll(false);
        UtilBools.tileLock = Game.current.lockTiles;
        UtilBools.noPlaceTile = Game.current.noPlaceTiles;
        //print("Utilbools: ABL: " + UtilBools.actionBarLock + ", CML: " + UtilBools.camMoveLock + ", NPT: " + UtilBools.noPlaceTile + ", SL: " + UtilBools.selectLock + ", TL: " + UtilBools.tileLock);
    }

    private void makeNewFloor(int newLength, int newHeight)
    {
        print("GameController:makeNewFloor()");

        Vector2 current = start;
        for (int i = 0; i < newHeight; i++)//For each unity in height
        {
            for (int j = 0; j < newLength; j++)//For each unit in length
            {
                GameObject floor = Instantiate(floorTilePrefab, current, Quaternion.identity);
                floor.transform.parent = floorFather.transform;
                //floor.transform.position += new Vector3(0, 0, 0);
                floor.GetComponent<FloorTileController>().spotIndex = index;
                if(j % 2 == 0)
                {
                    //floor.GetComponent<SpriteRenderer>().sprite = floorTileSprite1;
                }
                else
                {
                    //floor.GetComponent<SpriteRenderer>().sprite = floorTileSprite2;
                }
                index++;

                floorList.Add(floor);

                current += new Vector2(1.6f, 0);
            }
            current = start + new Vector2(0, -1.6f) * (i + 1);
        }

        tileDIRs = new int[newLength * newHeight];
        tileIDs = new int[newLength * newHeight];
        tilePower = new bool[newLength * newHeight];
        tileSetting = new int[newLength * newHeight];
        tiles = new GameObject[newLength * newHeight];
        signTexts = new string[newLength * newHeight];
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("Main") && !loaded)
        {
            loaded = true;
            
            //print("Setting father");
            GameObject[] newTiles = GameObject.FindGameObjectsWithTag("Tile");
            foreach (GameObject go in newTiles)
            {
                go.transform.parent = tileFather.transform;
            }
        }

        if (InputController.getInput(InputPurpose.UNDO))
        {
            UC.undo();
        }

        //Select
        if (AC.selectedA == 0 && !UtilBools.selectLock) // && Input.GetKey(KeyCode.LeftShift))
        {
            if (UtilBools.tileLock)
            {
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, -10), 100.0F);
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit2D hit = hits[i];

                    //print("got a hit: " + hit.transform.name);

                    if (hit.transform.GetComponent<MoverController>() != null ||
                        hit.transform.GetComponent<DupeController>() != null ||
                        hit.transform.GetComponent<Button>() != null ||
                        hit.transform.tag.Equals("NonSelectionNegatable")
                        )
                    {
                        return;
                    }
                }

                //print("runnin reset!");
                startIndex = -1;
                resetSelect();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                startIndex = -1;
                resetSelect();
            }
            else if (Input.GetMouseButton(0))
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, -10), 100.0F);
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit2D hit = hits[i];

                    //print("got a hit: " + hit.transform.name);

                    if (hit.transform.tag.Equals("NonSelectionNegatable"))
                    {
                        return;
                    }
                }

                //print("runnin select!");
                selector();
            }
            else if (InputController.getInput(InputPurpose.DELETE_SELECTED)) //Destroy selected
            {
                if (!compUI.confirmComponent.activeSelf)
                {
                    deleteSelected();
                }
            }
            else if (InputController.getInput(InputPurpose.TILE_ROTATE_LEFT)) //Rotate left
            {
                if (!compUI.confirmComponent.activeSelf)
                {
                    rotateSelected();
                }
            }
        }
        else
        {
            //print("Reseting select");
            startIndex = -1;
            resetSelect();
        }

        checkExpand();

        if(UtilBools.tileLock && !changedLocked)
        {
            foreach(GameObject g in tiles)
            {
                if(g != null)
                {
                    g.GetComponent<TileController>().locked = true;
                }
            }
            changedLocked = true;
        }
        else if(!UtilBools.tileLock && changedLocked)
        {
            foreach (GameObject g in tiles)
            {
                if (g != null)
                {
                    g.GetComponent<TileController>().locked = false;
                }
            }
            changedLocked = false;
        }
    }

    public void updateData()
    {
        //print("Gathering IDs and DIRs!");
        if (tileIDs.Length != tiles.Length)
        {
            tileIDs = new int[height * length];
            tileDIRs = new int[height * length];
            tilePower = new bool[length * height];
            tileSetting = new int[length * height];
        }

        signTexts = new string[length * height];

        for (int i = 0; i < tiles.Length; i++)
        {
            if(tiles[i] != null)
            {
                tileIDs[i] = tiles[i].GetComponent<TileController>().ID;
                tileDIRs[i] = tiles[i].GetComponent<TileController>().getDir();
                tilePower[i] = tiles[i].GetComponent<TileController>().beingPowered;
                if (tiles[i].GetComponent<DelayerController>() != null)
                {
                    tileSetting[i] = tiles[i].GetComponent<DelayerController>().getSetting();
                }
                else if(tiles[i].GetComponent<SignController>() != null)
                {
                    signTexts[i] = tiles[i].GetComponent<SignController>().text;
                }
                else
                {
                    tileSetting[i] = 0;
                    signTexts[i] = "";
                }
            }
            else
            {
                tileIDs[i] = 0;
                tileDIRs[i] = 0;
                tilePower[i] = false;
                tileSetting[i] = 0;
                signTexts[i] = "";
            }
        }
    }

    private void resetSelect()
    {
        if (selectedFloor.Count > 0 || mover != null)
        {
            selectorBackdrop.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            selectorBackdrop.SetActive(false);

            for (int i = 0; i < cacheList.Count; i++)
            {
                cacheList[i].SetActive(false);
                cacheList[i].transform.parent = selectedCache.transform;
            }

            //print("Resetting select");
            if (selectedTiles.Count > 0)
            {
                for (int i = 0; i < selectedTiles.Count; i++)
                {

                    if(selectedTiles[i] != null)
                    {
                        //print("Setting selectedTiles' parent");
                        selectedTiles[i].transform.parent = tileFather.transform;
                    }
                }
            }
            selectedFloor = new List<GameObject>();
            selectedTiles = new List<GameObject>();
            selectedTileIndex = new List<int>();
            if(mover != null)
            {
                //print("Killing mover");
                Destroy(mover);
                mover = null;
            }
            if(duper != null)
            {
                Destroy(duper);
                duper = null;
            }
            //print("Resetting selected-father rotation");
            selectedFather.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        
    }

    private void selector()
    {
        if(mover != null)
        {
            if (mover.GetComponent<MoverController>().drag)
            {
                return;
            }
        }

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, -10), 100.0F);

        int currentIndex = startIndex;
        int indexTop = 0; //left/right top point of box (opposite of startIndex)
        int indexDown = 0; //left/right down point of box (opposite of currentIndex)
        for (int i = 0; i < hits.Length; i++)
        {
            //print("got a hit");
            RaycastHit2D hit = hits[i];
            //print("hit: " + hit.transform.name);

            if(hit.transform.GetComponent<TileController>() != null)
            {
                if (hit.transform.GetComponent<TileController>().drag || hit.transform.GetComponent<TileController>().locked)
                {
                    return;
                }
            }
            if(hit.transform.GetComponent<MoverController>() != null 
            || hit.transform.GetComponent<DupeController>() != null
            || hit.transform.GetComponent<ComponentPlacerController>() != null)
            {
                return;
            }

            if (hit.transform.GetComponent<FloorTileController>() != null)
            {
                if (startIndex == -1)
                {
                    startIndex = hit.transform.GetComponent<FloorTileController>().spotIndex;
                }

                currentIndex = hit.transform.GetComponent<FloorTileController>().spotIndex;
            }
        }
        //print("Selecting & reseting!");
        if(prevSelectedIndex == currentIndex)
        {
            return;
        }

        resetSelect();
        prevSelectedIndex = currentIndex;

        if (startIndex < 0)
        {
            return;
        }

        //print("Start/Current: " + startIndex + "/" + currentIndex);

        int boxHeight = 0;

        //dragging downwards
        if (startIndex - currentIndex < 0)
        {
            //print("Dragging down!");

            indexTop = currentIndex;
            while (indexTop > startIndex)
            {
                indexTop -= length;
            }

            indexDown = startIndex;
            while (indexDown < currentIndex)
            {
                indexDown += length;
                boxHeight++;
            }
            selectedHeight = boxHeight;

            //print("indexDown: " + indexDown + ", StartIndex/CurrentIndex: " + startIndex + "/" + currentIndex);

            if (floorList[startIndex].transform.position.x < floorList[currentIndex].transform.position.x) //dragging on right side 
            {
                int boxLength = currentIndex - (indexDown - length);
                selectedLength = boxLength;

                //print("Down right! BoxLength: " + boxLength + ", boxHeigthR: " + boxHeigthR + ", boxHeigthL: " + boxHeigthL);
                for (int k = 0; k < boxHeight; k++)
                {
                    for (int n = 0; n <= boxLength; n++) //horizontal
                    {
                        selectedFloor.Add(floorList[startIndex + (1 * n) + (length * k)]);
                        //selectedTiles.Add(tiles[startIndex + (1 * n) + (length * k)]);
                    }
                }
            }
            else //dragging left side
            {
                int boxLength = indexDown - currentIndex;
                selectedLength = boxLength;

                //print("Down left!");
                //print("Down left: StartIndex/CurrentIndex: " + startIndex + "/" + currentIndex + " | TIT/TID: " + timesIndexTop + "/" + timesIndexDown + " | indexTop / IndexDown: " + indexTop + " / " + indexDown);
                for (int k = 0; k <= boxHeight; k++)
                {
                    for (int n = 0; n <= boxLength; n++) //horizontal
                    {
                        selectedFloor.Add(floorList[startIndex - (1 * n) + (length * k)]);
                        //selectedTiles.Add(tiles[startIndex - (1 * n) + (length * k)]);
                    }
                }
            }
        }
        //dragging upwards
        else if (startIndex - currentIndex > 0)
        {
            //print("Dragging up!");

            indexTop = startIndex;
            while (indexTop > currentIndex)
            {
                indexTop -= length;
            }

            indexDown = currentIndex;
            while (indexDown < startIndex)
            {
                indexDown += length;
                boxHeight++;
            }
            selectedHeight = boxHeight;

            //print("indexDown: " + indexDown + ", StartIndex/CurrentIndex: " + startIndex + "/" + currentIndex);

            if (floorList[startIndex].transform.position.x <= floorList[currentIndex].transform.position.x) //dragging on right side 
            {
                int boxLength = indexDown - startIndex;
                selectedLength = boxLength;

                //print("Up right!");
                for (int k = 0; k <= boxHeight; k++)
                {
                    for (int n = 0; n <= boxLength; n++) //horizontal
                    {
                        selectedFloor.Add(floorList[startIndex + (1 * n) - (length * k)]);
                        //selectedTiles.Add(tiles[startIndex + (1 * n) - (length * k)]);
                    }
                }
            }
            else //dragging left side
            {
                int boxLength = startIndex - (indexDown - length);
                selectedLength = boxLength;

                //print("Up left!");
                for (int k = 0; k < boxHeight; k++)
                {
                    for (int n = 0; n <= boxLength; n++) //horizontal
                    {
                        selectedFloor.Add(floorList[startIndex - (1 * n) - (length * k)]);
                        //selectedTiles.Add(tiles[startIndex - (1 * n) - (length * k)]);
                    }
                }
            }
            /*
                * spotIndex - 1 = tile til venstre
                * spotIndex + 1 = tile til høyre
                * spotIndex - GC.length = tile over
                * spotIndex + GC.length = tile under
                * 
                * spotIndex +- 1*n = tile til n høyre/venstre
                * 
                * spotIndex +- (1*n +- GC.length*k) = tile k under/over og n til høyre/venstre
            */
        }

        //add selected tiles
        for(int i = 0; i < selectedFloor.Count; i++)
        {
            int index = selectedFloor[i].GetComponent<FloorTileController>().spotIndex;
            if (tiles[index] != null)
            {
                if (!tiles[index].GetComponent<TileController>().locked)
                {
                    //print("yo");
                    selectedTiles.Add(tiles[index]);
                    tiles[index].transform.parent = selectedFather.transform;
                    selectedTileIndex.Add(i);
                }
            }
        }
        /*//remove locked
        for(int i = 0; i < selectedTiles.Count; i++)
        {
            if (selectedTiles[i].GetComponent<TileController>().locked)
            {
                selectedTiles.Remove(selectedTiles[i]);
                i--;
            }
            //print(selectedTiles[i].name);
        }*/

        //Cornerpos
        List<Vector3> cornerPos = new List<Vector3>();
        for (int i = 0; i < selectedFloor.Count; i++)
        {
            //print("SLength: " + selectedLength + ", sCount: " + selectedTiles.Count + ", i: " + i);
            if (i == 0 || i == selectedLength || i == (selectedFloor.Count - 1) - selectedLength || i == selectedFloor.Count - 1)
            {
                GameObject go = null;
                if (i == 0)
                {
                    go = cacheList[0];
                }
                else if(i == selectedLength)
                {
                    go = cacheList[1];
                }
                else if(i == (selectedFloor.Count - 1) - selectedLength)
                {
                    go = cacheList[2];
                }
                else if(i == selectedFloor.Count - 1)
                {
                    go = cacheList[3];
                }
                go.transform.position = selectedFloor[i].transform.position + new Vector3(0, 0, -1);
                go.transform.parent = selectedFather.transform;
                go.SetActive(true);

                cornerPos.Add(go.transform.position);
            }
        }
        selectedLength++;
        print("sH: " + selectedHeight + ", sL: " + selectedLength);
        //backdrop
        if (cornerPos.Count > 0)
        {
            if(cornerPos.Count < 3)
            {
                if(cornerPos[0].x != cornerPos[1].x)
                {
                    selectorBackdrop.transform.position = new Vector3((cornerPos[0].x + cornerPos[1].x) / 2, cornerPos[0].y, 0);
                    selectorBackdrop.transform.localScale = new Vector3(selectedLength, selectedHeight, 1);
                }
                else
                {
                    selectorBackdrop.transform.position = new Vector3(cornerPos[0].x, (cornerPos[0].y + cornerPos[1].y) / 2, 0);
                    selectorBackdrop.transform.localScale = new Vector3(selectedLength, selectedHeight + 1, 1);
                }

            }
            else
            {
                selectorBackdrop.transform.position = new Vector3((cornerPos[0].x + cornerPos[1].x) / 2, (cornerPos[0].y + cornerPos[2].y) / 2, 0);
                if (cornerPos[0].x < cornerPos[1].x && cornerPos[0].y < cornerPos[2].y)
                {
                    //print("here1");
                    selectorBackdrop.transform.localScale = new Vector3(selectedLength, selectedHeight + 1, 1);
                }
                else if(cornerPos[0].x > cornerPos[1].x && cornerPos[0].y > cornerPos[2].y)
                {
                    //print("here2");
                    selectorBackdrop.transform.localScale = new Vector3(selectedLength, selectedHeight + 1, 1);
                }
                else
                {
                    //print("here3");
                    selectorBackdrop.transform.localScale = new Vector3(selectedLength, selectedHeight, 1);
                }
            }
            selectorBackdrop.SetActive(true);
        }

        //Dupe and move button
        int myIndex = 0;
        if((selectedFloor.Count - 2) >= 2)
        {
            myIndex = (selectedFloor.Count - 1);
        }
        if(selectedFloor.Count > 0)
        {
            mover = Instantiate(moverPrefab, selectedFloor[myIndex].transform.position + new Vector3(0, 0, -2), Quaternion.identity);
            mover.GetComponent<MoverController>().selectedFather = selectedFather;
            mover.GetComponent<MoverController>().GC = this;
            mover.transform.parent = selectedFather.transform;
        }

        if(selectedFloor.Count > 1)
        {
            myIndex = 1;
            if ((selectedFloor.Count - 3) >= 3)
            {
                myIndex = (selectedFloor.Count - 3);
            }
            duper = Instantiate(duperPrefab, selectedFloor[myIndex].transform.position + new Vector3(0, 0, -2), Quaternion.identity);
            duper.GetComponent<DupeController>().GC = this;
            duper.transform.parent = selectedFather.transform;
        }
    }

    private void createSelectedTiles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(selectorPrefab, selectedCache.transform.position, Quaternion.identity);
            cacheList.Add(go);
            go.transform.parent = selectedCache.transform;
            go.SetActive(false);
        }
        //print("cacheCount: " + cacheList.Count);
    }

    public void dragSelected()
    {
        startIndexesSelect = new int[selectedTiles.Count];
        //print("Dragging selected!");
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            if(selectedTiles[i] != null)
            {
                int spotIndex = selectedTiles[i].GetComponent<TileController>().spotIndex;
                if (spotIndex > 0)
                {
                    startIndexesSelect[i] = selectedTiles[i].GetComponent<TileController>().spotIndex;
                    //print("Drag spotindex: " + startIndexesSelect[i]);
                }
                selectedTiles[i].GetComponent<TileController>().dragAsSelected();
            }
        }
        selectedFloor = new List<GameObject>();
    }

    public void unDragSelected()
    {
        int[] endIndexes = new int[selectedTiles.Count];
        //print("Undragging!");
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            if (selectedTiles[i] != null)
            {
                selectedTiles[i].GetComponent<TileController>().drag = false;
                endIndexes[i] = selectedTiles[i].GetComponent<TileController>().spotIndex;
                //print("unDrag spotIndex: " + selectedTiles[i].GetComponent<TileController>().spotIndex);
            }
        }
        StartCoroutine(afterUnDrag());
    }

    private IEnumerator afterUnDrag()
    {
        yield return new WaitForSeconds(0.1f);
        int[] endIndexes = new int[selectedTiles.Count];
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            if (selectedTiles[i] != null)
            {
                //print("unDrag spotIndex: " + selectedTiles[i].GetComponent<TileController>().spotIndex);
                endIndexes[i] = selectedTiles[i].GetComponent<TileController>().spotIndex;
            }
        }
        //print(startIndexesSelect[0] + "/" + startIndexesSelect[1]);
        if (!recentDupe)
        {
            UC.addUndo(new MultipleTilesUndo(startIndexesSelect, endIndexes));
        }
        else
        {
            recentDupe = false;
            UC.addUndo(new MultipleTilesUndo(endIndexes));
        }
    }

    private void deleteSelected()
    {
        if(selectedTiles.Count <= 0)
        {
            //print(classTag + "deleteSelected(): selectedTiles.Count <= 0");
            return;
        }
        print(classTag + "deleteSelected(): Deleting selected");
        int[] tileIds = new int[selectedTiles.Count];
        int[] dirs = new int[selectedTiles.Count];
        int[] spotIndexes = new int[selectedTiles.Count];
        bool[] tilePowers = new bool[selectedTiles.Count];
        int[] settings = new int[selectedTiles.Count];
        string[] signTexts = new string[selectedTiles.Count];
        for (int i = selectedTiles.Count - 1; i >= 0; i--)
        {
            GameObject go = selectedTiles[i];
            selectedTiles.RemoveAt(i);

            TileController myTile = go.GetComponent<TileController>();
            tileIds[i] = myTile.ID;
            dirs[i] = myTile.getDir();
            spotIndexes[i] = myTile.spotIndex;
            tilePowers[i] = myTile.beingPowered;
            if(go.GetComponent<DelayerController>() != null)
            {
                settings[i] = go.GetComponent<DelayerController>().getSetting();
            }
            else if(go.GetComponent<SignController>() != null)
            {
                signTexts[i] = go.GetComponent<SignController>().text;
            }

            go.GetComponent<TileController>().destroyMe();
        }
        startIndex = -1;
        resetSelect();

        UC.addUndo(new MultipleTilesUndo(tileIds, dirs, spotIndexes, tilePowers, settings, signTexts));
    }

    private void spawn()
    {
        tiles = new GameObject[length * height];
        index = 0;
        Vector2 current = start;
        for (int i = 0; i < height; i++)//For each unit in height
        {
            for (int j = 0; j < length; j++)//For each unit in length
            {
                //print("Index: " + index);
                //print("tileID: " + tileIDs[index]);
                if(tileIDs[index] > 0)
                {
                    GameObject prefab = Instantiate(tileHub.getPrefab(tileIDs[index]), current, Quaternion.identity);
                    prefab.GetComponent<TileController>().spotIndex = index;
                    prefab.GetComponent<TileController>().setDir(tileDIRs[index]);
                    prefab.GetComponent<TileController>().beingPowered = tilePower[index];
                    if(prefab.GetComponent<DelayerController>() != null)
                    {
                        prefab.GetComponent<DelayerController>().setSetting(tileSetting[index]);
                    }
                    else if (prefab.GetComponent<SignController>() != null)
                    {
                        prefab.GetComponent<SignController>().text = signTexts[index];
                        //print(i + ": " + signTexts[index]);
                    }
                    tiles[index] = prefab;
                }
                index++;
                current += new Vector2(1.6f, 0);
            }
            current = start + new Vector2(0, -1.6f) * (i + 1);
        }
        //print("Spawning done!");
    }

    public void spawnMultiple(int[] tileIds, int[] dirs, int[] spotIndexes, bool[] tilePowers, int[] settings, string[] signTexts)
    {
        for (int i = 0; i < tileIds.Length; i++)//For each tileId we have
        {
            //print("Index: " + index);
            //print("tileID: " + tileIDs[index]);
            if (tileIds[i] > 0)
            {
                int spotIndex = spotIndexes[i];

                GameObject prefab = Instantiate(tileHub.getPrefab(tileIds[i]), floorList[spotIndex].transform.position + new Vector3(0, 0, -1), Quaternion.identity);
                prefab.transform.parent = tileFather.transform;
                prefab.GetComponent<TileController>().spotIndex = spotIndex;
                prefab.GetComponent<TileController>().setDir(dirs[i]);
                prefab.GetComponent<TileController>().beingPowered = tilePowers[i];
                if (prefab.GetComponent<DelayerController>() != null)
                {
                    prefab.GetComponent<DelayerController>().setSetting(settings[i]);
                }
                else if (prefab.GetComponent<SignController>() != null)
                {
                    prefab.GetComponent<SignController>().text = signTexts[i];
                    //print(i + ": " + signTexts[index]);
                }
            }         
        }
        //print("Spawning Multiple done!");
    }

    public GameObject spawnSingle(int id, int index, int dir) //for puzzle
    {
        GameObject prefab = Instantiate(tileHub.getPrefab(id), floorList[index].transform.position + new Vector3(0,0,-1), Quaternion.identity);
        prefab.GetComponent<TileController>().spotIndex = index;
        prefab.GetComponent<TileController>().setDir(dir);
        print(classTag + "spawnSingle():index: " + index);
        tiles[index] = prefab;
        return prefab;
    }

    public GameObject spawnSingle(int index, int id, int dir, bool power, int setting, string signText, TileController.label myLabel) //for comp
    {
        GameObject prefab = Instantiate(tileHub.getPrefab(id), floorList[index].transform.position, Quaternion.identity);
        if (prefab.GetComponent<TileController>().place(floorList[index]))
        {
            prefab.transform.parent = tileFather.transform;
            prefab.GetComponent<TileController>().spotIndex = index;
            prefab.GetComponent<TileController>().setDir(dir);
            prefab.GetComponent<TileController>().beingPowered = power;
            prefab.GetComponent<TileController>().myLabel = myLabel;
            tiles[index] = prefab;
            if (prefab.GetComponent<DelayerController>() != null)
            {
                prefab.GetComponent<DelayerController>().setSetting(setting);
            }
            else if (prefab.GetComponent<SignController>() != null)
            {
                prefab.GetComponent<SignController>().text = signText;
            }
            return prefab;
        }
        else
        {
            return null;
        }
    }

    public void deleteSingle(int spotIndex)
    {
        if(tiles[spotIndex] != null)
        {
            tiles[spotIndex].GetComponent<TileController>().destroyMe();
        }
    }

    public void moveSingle(int from, int to)
    {
        print(classTag + "MoveSingle: " + from + "/" + to);
        if(from < tiles.Length)
        {
            GameObject tile = tiles[from];
            if(tile != null)
            {
                tile.GetComponent<TileController>().dragAsSelected();
                tile.transform.position = floorList[to].transform.position + new Vector3(0, 0, -1);
                tile.GetComponent<TileController>().drag = false;
            }
        }
    }

    public void deleteMultiple(int[] spotIndex)
    {
        for(int i = 0; i < spotIndex.Length; i++)
        {
            int spot = spotIndex[i];
            if (tiles[spot] != null)
            {
                tiles[spot].GetComponent<TileController>().destroyMe();
            }
        }
    }

    public void moveMultiple(int[] spotIndexes, int[] spotIndexes2)
    {
        if(spotIndexes.Length != spotIndexes2.Length)
        {
            print(classTag + "moveMultiple(): Index-lists not of same length!");
            return;
        }
        for(int i = 0; i < spotIndexes.Length; i++)
        {
            int from = spotIndexes2[i];
            int to = spotIndexes[i];
            if(from < tiles.Length)
            {
                //print("i: " + i + ", from: " + from + ", to: " + to + ", tiles.length: " + tiles.Length);     
                GameObject tile = tiles[from];
                if(tile != null)
                {
                    //print("Tile: " + tile.name);
                    tile.GetComponent<TileController>().dragAsSelected();
                    tile.transform.position = floorList[to].transform.position + new Vector3(0, 0, -1);
                    tile.GetComponent<TileController>().drag = false;
                }
            }
        }
    }

    private void rotateSelected()
    {
        if (!signManager.pinned)
        {
            if(selectedTiles.Count > 0)
            {
                dragSelected();

                print(classTag + "Trying to rotate, wish me luck");

                int tempInt = selectedHeight - 1;
                selectedHeight = selectedLength + 1;
                selectedLength = tempInt;

                //move selection tiles
                GameObject[] tempuras = new GameObject[selectedFather.transform.childCount];
                for (int i = 0; i < selectedFather.transform.childCount; i++)
                {
                    tempuras[i] = selectedFather.transform.GetChild(i).gameObject;
                }
                for (int i = 0; i < tempuras.Length; i++)
                {
                    if(tempuras[i] != null)
                    {
                        tempuras[i].transform.parent = mover.transform;
                    }
                }

                mover.transform.Rotate(new Vector3(0, 0, -90));

                tempuras = new GameObject[mover.transform.childCount];
                for (int i = 0; i < mover.transform.childCount; i++)
                {
                    tempuras[i] = mover.transform.GetChild(i).gameObject;
                }
                for (int i = 0; i < tempuras.Length; i++)
                {
                    tempuras[i].transform.parent = selectedFather.transform;
                }

                //rotate inputs and outputs!
                for(int i = 0; i < selectedTiles.Count; i++)
                {
                    if(selectedTiles[i] != null)
                    {
                        selectedTiles[i].GetComponent<TileController>().rotateX(-1, false);
                    }
                }

                unDragSelected();
            }
        }
    }

    private void checkExpand()
    {
        if (UtilBools.isPuzzle)
        {
            return;
        }
        //print("checking expand!");
        bool expd = false;
        for(int i = 0; i < doExpand.Length; i++)
        {
            if (doExpand[i] > 0)
            {
                expd = true;
            }
        }
        UtilBools.expandLoading = expd;

        if (doExpand[0] > 0 && !doneExpanding[0])
        {
            //print("Expand up");
            expand(0, doExpand[0]);
        }
        else if (doExpand[1] > 0 && !doneExpanding[1])
        {
            //print("Expand left");
            expand(1, doExpand[1]);
        }
        else if (doExpand[2] > 0 && !doneExpanding[2])
        {
            //print("Expand down");
            expand(2, doExpand[2]);
        }
        else if (doExpand[3] > 0 && !doneExpanding[3])
        {
            //print("Expand right");
            expand(3, doExpand[3]);
        }
        else
        {
            doExpand = new int[4];
            doneExpanding = new bool[4];
        }
    }

    //for expanding playable area
    public void expand(int dir, int count) //dir: 0 = up, 1 = left, 2 = down, 3 = right
    {
        //ExpandFrame goes through the counts (1 for each row/column)
        //ExpandTile goes through each tile on this row/column

        //print("Expanding: " + expandFrame + "/" + count + "/" + dir);
        if (expandFrame < count)
        {
            //print("Expanding- D/C/eF: " + dir + "/" + count + "/" + expandFrame);

            Vector2 current = start;
            Vector2 myStart = start;
            //OK
            if (dir == 0) //UP
            {
                myStart = start + (new Vector2(1.6f, 0) * (length - 1));
                current = myStart + (new Vector2(0, 1.6f)) + (new Vector2(-1.6f, 0)*(expandTile));

                if (expandTile < length)
                {
                    expandTile++;
                    GameObject go = Instantiate(floorTilePrefab, current, Quaternion.identity);
                    go.transform.parent = floorFather.transform;
                    floorList.Insert(0, go);
                    for (int k = 0; k < floorList.Count; k++)
                    {
                        floorList[k].GetComponent<FloorTileController>().spotIndex = k;
                    }
                }
                else
                {
                    height++;
                    expandTile = 0;
                    expandFrame++;
                    start += (new Vector2(0, 1.6f));

                    //Tile-list
                    GameObject[] temp = new GameObject[height * length];
                    int orgIndex = 0;
                    for (int j = 0; j < temp.Length; j++)
                    {
                        if (j <= (length - 1))
                        {
                            //nothing here
                        }
                        else
                        {
                            if(tiles[orgIndex] != null)
                            {
                                temp[j] = tiles[orgIndex];
                                temp[j].GetComponent<TileController>().spotIndex = j;
                            }
                            orgIndex++;
                        }
                    }
                    tiles = temp;
                }
            }
            //OK
            else if (dir == 2) //DOWN
            {
                //print("Expanding down! ExpandTile: " + expandTile);
                myStart = start + (new Vector2(0, -1.6f) * (height));
                current = myStart + (new Vector2(1.6f, 0) * (expandTile));

                if (expandTile < length)
                {
                    expandTile++;
                    GameObject go = Instantiate(floorTilePrefab, current, Quaternion.identity);
                    go.transform.parent = floorFather.transform;
                    floorList.Add(go);

                    for (int k = 0; k < floorList.Count; k++)
                    {
                        floorList[k].GetComponent<FloorTileController>().spotIndex = k;
                    }
                }
                else
                {
                    height++;
                    expandTile = 0;
                    expandFrame++;

                    //Tile-list
                    //print("Heigth/Length: " + height + "/" + length + " | " + expandFrame + "/"+ expandTile);
                    GameObject[] temp = new GameObject[height * length];
                    int orgIndex = 0;
                    for (int j = 0; j < temp.Length; j++)
                    {
                        if (j > tiles.Length - 1)
                        {
                            //oh boy
                        }
                        else
                        {
                            if (tiles[orgIndex] != null)
                            {
                                temp[j] = tiles[orgIndex];
                                temp[j].GetComponent<TileController>().spotIndex = j;
                            }
                            orgIndex++;
                        }
                    }
                    tiles = temp;
                }
            }
            //OK
            else if (dir == 1) //LEFT
            {
                myStart = start;
                current = myStart + (new Vector2(-1.6f, 0)) + (new Vector2(0, -1.6f) * (expandTile));

                if (expandTile < height)
                {
                    GameObject go = Instantiate(floorTilePrefab, current, Quaternion.identity);
                    go.transform.parent = floorFather.transform;
                    floorList.Insert(expandTile * (length+1), go);
                    for (int k = 0; k < floorList.Count; k++) //reset all indexes
                    {
                        floorList[k].GetComponent<FloorTileController>().spotIndex = k;
                    }
                    expandTile++;
                }
                else
                {
                    length++;
                    expandTile = 0;
                    expandFrame++;
                    start += new Vector2(-1.6f, 0);

                    //Tile-list
                    GameObject[] temp = new GameObject[height * length];
                    int orgIndex = 0;
                    for (int j = 0; j < temp.Length; j++)
                    {
                        if (j % (length) == 0) //0*length, 1*length etc..
                        {
                            //cheers
                            //print("Spawning air, j: " + j);
                            //print("Floorlist[j]: " + floorList[j].GetComponent<FloorTileController>().spotIndex);
                        }
                        else
                        {
                            if (tiles[orgIndex] != null)
                            {
                                temp[j] = tiles[orgIndex];
                                temp[j].GetComponent<TileController>().spotIndex = j;
                            }
                            orgIndex++;
                        }
                    }
                    tiles = temp;
                }
            }
            //OK!
            else if (dir == 3) //RIGHT
            {
                myStart = start + (new Vector2(1.6f, 0) * length);
                current = myStart + (new Vector2(0, -1.6f) * (expandTile));

                if (expandTile < height)
                {
                    GameObject go = Instantiate(floorTilePrefab, current, Quaternion.identity);
                    go.transform.parent = floorFather.transform;
                    if((expandTile + 1) * length + expandTile < floorList.Count)
                    {
                        floorList.Insert((expandTile + 1) * length + expandTile, go);
                    }
                    else
                    {
                        floorList.Add(go);
                    }

                    for (int k = 0; k < floorList.Count; k++) //reset all indexes
                    {
                        floorList[k].GetComponent<FloorTileController>().spotIndex = k;
                    }
                    expandTile++;
                }
                else
                {
                    length++;
                    expandTile = 0;
                    expandFrame++;

                    //Tile-list
                    GameObject[] temp = new GameObject[height * length];
                    int orgIndex = 0;
                    for (int j = 0; j < temp.Length; j++)
                    {
                        if (j % (length-1) == 0 && j >= (length - 1)) //0*(length-1), 1*(length-1) etc..
                        {
                            //yo
                            //print("Spawning air: " + j + "/" + floorList[myIndex].GetComponent<TileController>().spotIndex);
                        }
                        else
                        {
                            //print(j + "/" + orgIndex + ", tiles.length: " + tiles.Length + ", temp.length: " + temp.Length);
                            if (tiles[orgIndex] != null)
                            {
                                temp[j] = tiles[orgIndex];
                                temp[j].GetComponent<TileController>().spotIndex = j;
                            }
                            orgIndex++;
                        }
                    }
                    tiles = temp;
                }
            }
        }
        else
        {
            expandFrame = 0;
            expandTile = 0;
            doneExpanding[dir] = true;
            //print("done Expanding in dir: " + dir);
        }
        /*
         * Can add all floors to floorList and have them update their spotindex to their index in floorList. Have the tiles
         * update their spotindex based on their homeObj (their floorTile)
         * 
         */
    }

    public void duplicateSelected()
    {
        //print(classTag + "Trying to dupe");
        if(selectedTiles.Count > 0)
        {
            print(classTag + "Dupin");
            foreach(GameObject go in selectedTiles)
            {
                if(go != null)
                {
                    GameObject dupe = Instantiate(go, go.transform.position, Quaternion.identity);
                    dupe.transform.rotation = go.transform.rotation;
                    tiles[go.GetComponent<TileController>().spotIndex] = dupe;
                    dupe.transform.parent = tileFather.transform;
                    dupe.GetComponent<TileController>().placed = false;

                    go.GetComponent<TileController>().beingPowered = false;
                }
            }
            dragSelected();
            //fselectedFather.transform.position += new Vector3(1.6f, 0, 0);
        }
        recentDupe = true;
    }

    public void changeItemName(string name)
    {
        itemName.text = name;
        itemName.gameObject.SetActive(true);
    }

    public void closeItemName()
    {
        itemName.gameObject.SetActive(false);
    }
}
