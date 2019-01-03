using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class TileController : MonoBehaviour
{
    //Father-class for all tiles
    public static readonly int UP = 0;
    public static readonly int LEFT = 1;
    public static readonly int DOWN = 2;
    public static readonly int RIGHT = 3;

    public bool locked = false;
    public int ID = 0;

    [SerializeField]
    protected int dir = 0; //0 is up, 1 is left, 2 is down, 3 is right (for rotating!)

    [SerializeField]
    protected int[] output = new int[4]; //0 = off, 1 = on
    [SerializeField]
    protected List<int> possibleOutputs = new List<int>();

    public GameController GC;
    public ActionbarController AC;
    public GameObject IOPicker;
    public int spotIndex = -1;

    public bool beingPowered = false;
    public Sprite spriteOn;
    public Sprite spriteOff;

    //For tile-placement and movement
    public bool drag = false;
    public bool placed = false;
    protected GameObject homeObj;

    public label myLabel;

    public GameObject myCompOverlay = null;

    //Undo
    public int prevSpotIndex = -1;
    public bool dragged = false;

    //For analyzing
    public enum label {
        NULL, IN, OUT, CARRY, RESET
    }

    public TileController()
    {
        spotIndex = -1;
    }

    private void Awake()
    {
        GC = GameObject.Find("GameController").GetComponent<GameController>();
        AC = GC.AC;
    }

    // Use this for initialization
    protected virtual void Start () {
        output = new int[4];
    }

    protected abstract void tryPower(bool state);

    protected void OnMouseDrag()
    {
        if (locked || UtilBools.tileLock)
        {
            return;
        }
        //Set drag true, get this' position based on mouseposition!
        drag = true;
        dragged = true;
        /*
         * Old:
        float z = 7.9f;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, z));
        */

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, -10), 100.0F);

        if(hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                if (hit.transform.GetComponent<FloorTileController>() != null)
                {
                    transform.position = hit.transform.position + new Vector3(0, 0, -1);
                }
            }
        }

        beingPowered = false;
        placed = false;

        if (homeObj != null)
        {
            homeObj.GetComponent<FloorTileController>().busy = false;
            GC.tiles[spotIndex] = null;
            prevSpotIndex = spotIndex;
            spotIndex = -1;
            homeObj = null;
        }

        Color temp = GetComponent<SpriteRenderer>().color;
        temp.a = 0.5f;
        GetComponent<SpriteRenderer>().color = temp;

        rotate();

        if (InputController.getInput(InputPurpose.DELETE_TILE))
        {
            destroyMe();
        }
        if (myCompOverlay != null)
        {
            print("Removing Compoverlay!");
            GC.compUI.removeOverlay(myCompOverlay);
        }
    }

    protected void rotate()
    {
        if (locked)
        {
            return;
        }
        if (InputController.getInput(InputPurpose.TILE_ROTATE_LEFT))
        {
            rotateX(1, true);
        }
        else if (InputController.getInput(InputPurpose.TILE_ROTATE_RIGHT))
        {
            rotateX(-1, true);
        }
    }

    public void rotateX(int times, bool rotate)
    {
        if(times < 0)
        {
            times = 3;
        }
        for(int x = 0; x < times; x++)
        {
            //AC.rotate();
            dir += 1;
            if (dir > 3)
            {
                dir = 0;
            }

            rotateInput();

            for (int i = 0; i < possibleOutputs.Count; i++)
            {
                possibleOutputs[i] += 1;
                if (possibleOutputs[i] > 3)
                {
                    possibleOutputs[i] = 0;
                }
            }

            int temp = output[output.Length - 1];
            for (int i = output.Length - 1; i >= 0; i--)
            {
                if (i - 1 >= 0)
                {
                    output[i] = output[i - 1]; //i: 3=2, 2=1, 1=0
                }
                else
                {
                    output[i] = temp; //0=3
                }

            }
            if (rotate)
            {
                transform.Rotate(0, 0, 90);
            }
        }
    }

    protected abstract void rotateInput();

    protected void OnMouseUp() //When mouse button is upped
    {
        //print("Drag false!");
        drag = false;
    }

    protected void OnTriggerStay2D(Collider2D other)
    {
        //print("triggering: " + drag + "/" + placed);
        if (!drag && !placed)
        {
            //print(this.gameObject.name + ": Need home!");
            if (other.GetComponent<FloorTileController>() != null)
            {
                place(other.gameObject);
                if (!other.GetComponent<FloorTileController>().busy)
                {
                    //print(this.gameObject.name + ": Placing");
                    //place(other.gameObject);
                }
            }
        }
    }

    public abstract bool hasOutput(int DIR);

    protected bool Contains(GameObject[] objList, GameObject wanted)
    {
        for (int i = 0; i < objList.Length; i++)
        {
            if(objList[i] != null)
            {
                if (objList[i].Equals(wanted))
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected int negDirection(int DIR)
    {
        int neg = DIR + 2;
        if (neg == 4)
        {
            neg = 0;
        }
        if (neg > 4)
        {
            neg = 1;
        }
        return neg;
    }

    public void setDir(int DIR)
    {
        switch (DIR)
        {
            case 0:
                break;
            case 1:
                rotateX(1, true);
                break;
            case 2:
                rotateX(2, true);
                break;
            case 3:
                rotateX(3, true);
                break;
        }
    }

    public int getDir()
    {
        return dir;
    }

    public void place(GameObject other)
    {
        //print("Placed!");
        transform.position = other.transform.position + new Vector3(0, 0, -1);
        placed = true;
        homeObj = other;
        homeObj.GetComponent<FloorTileController>().busy = true;

        Color temp = GetComponent<SpriteRenderer>().color;
        temp.a = 1f;
        GetComponent<SpriteRenderer>().color = temp;

        spotIndex = homeObj.GetComponent<FloorTileController>().spotIndex;

        if(GC.tiles[spotIndex] != null)
        {
            //print("Placing... Destroying: " + GC.tiles[spotIndex].name);
            if(GC.tiles[spotIndex] != this.gameObject && !GC.tiles[spotIndex].GetComponent<TileController>().locked)
            {
                Destroy(GC.tiles[spotIndex]);
                GC.tiles[spotIndex] = this.gameObject;
            }
        }
        else
        {
            GC.tiles[spotIndex] = this.gameObject;
        }
        if (dragged)
        {
            GC.UC.addUndo(new SingleTileUndo(prevSpotIndex, spotIndex));
            dragged = false;
        }
    }

    public virtual void destroyMe()
    {
        if (locked)
        {
            return;
        }

        //print("DestroyMe: " + this.name + "/" + homeObj.name + "/" + spotIndex);
        for(int i = 0; i < output.Length; i++)
        {
            output[i] = 0;
        }
        if(homeObj != null)
        {
            homeObj.GetComponent<FloorTileController>().busy = false;
            GC.tiles[spotIndex] = null;
        }
        if(myCompOverlay != null)
        {
            print("Removing Compoverlay!");
            GC.compUI.removeOverlay(myCompOverlay);
        }

        Destroy(this.gameObject);
    }

    public void dragAsSelected()
    {
        if (locked || UtilBools.tileLock)
        {
            return;
        }

        drag = true;

        beingPowered = false;
        placed = false;

        if (homeObj != null)
        {
            //print(this.name + " is setting homeObj false");
            homeObj.GetComponent<FloorTileController>().busy = false;

            GC.tiles[spotIndex] = null;
            spotIndex = -1;
            homeObj = null;
        }

        Color temp = GetComponent<SpriteRenderer>().color;
        temp.a = 0.5f;
        GetComponent<SpriteRenderer>().color = temp;
    }

    public virtual bool canHasOutput(int DIR)
    {
        if (possibleOutputs.Contains(DIR))
        {
            return true;
        }
        return false;
    }
}
