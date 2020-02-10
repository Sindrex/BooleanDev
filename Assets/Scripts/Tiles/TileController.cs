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

    public bool infLocked = false;
    public bool locked = false;
    public bool compLocked = false;
    public int ID = 0;

    [SerializeField]
    protected int dir = 0; //0 is up, 1 is left, 2 is down, 3 is right (for rotating!)

    public GameController GC;
    public ActionbarController AC;
    public GameObject IOPicker;     //DEPRECATED
    public int spotIndex = -1;

    public bool beingPowered = false;
    public Sprite spriteOn;
    public Sprite spriteOff;

    //For tile-placement and movement
    public bool drag = false;
    public bool placed = false;
    protected GameObject homeObj;

    //DEPRECATED?
    public label myLabel;

    public GameObject myCompOverlay = null;

    //Undo
    public int prevSpotIndex = -1;
    public bool dragged = false;

    //DEPRECATED?
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

    protected void OnMouseDrag()
    {
        if (locked || UtilBools.tileLock || AC.selectedA != 1)
        {
            return;
        }
        //Set drag true, get this' position based on mouseposition!
        drag = true;
        dragged = true;
        //Old:
        //float z = 7.9f;
        //transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, z));

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
            destroyMe(false);
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

    public virtual void rotateX(int times, bool rotate)
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
            if (rotate)
            {
                transform.Rotate(0, 0, 90);
            }
        }
    }

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
            if(other.gameObject == homeObj)
            {
                print("returning!");
                return;
            }
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

    //help method
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

    public bool place(GameObject other)
    {
        spotIndex = other.GetComponent<FloorTileController>().spotIndex;

        if(GC.tiles[spotIndex] != null)
        {
            //print("Placing... Trying to destroy: " + GC.tiles[spotIndex].name);
            if(GC.tiles[spotIndex] != this.gameObject)
            {
                if (GC.tiles[spotIndex].GetComponent<TileController>().destroyMe(true))
                {
                    GC.tiles[spotIndex] = this.gameObject;
                    AC.audioCon.playTilePlacedSFX();
                }
                else
                {
                    destroyMe(false);
                    return false;
                }
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

        //print("Placed!");
        transform.position = other.transform.position + new Vector3(0, 0, -1);
        placed = true;
        homeObj = other;
        homeObj.GetComponent<FloorTileController>().busy = true;

        Color temp = GetComponent<SpriteRenderer>().color;
        temp.a = 1f;
        GetComponent<SpriteRenderer>().color = temp;

        GetComponent<BoxCollider2D>().isTrigger = true;

        return true;
    }

    public virtual bool destroyMe(bool addUndo)
    {
        if (locked)
        {
            return false;
        }

        //print("DestroyMe: " + this.name + "/" + homeObj.name + "/" + spotIndex);
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
        return true;
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
}
