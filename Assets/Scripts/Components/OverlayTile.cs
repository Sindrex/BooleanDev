using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour {

    //for labeling tiles in overlay
    public GameObject myTile; //floortile
    public TextMesh myLabel;

    public GameObject pop;
    public ComponentOverlay myOverlay;

    public CamController CC;
    public GameController GC;

    private void Start()
    {
        myLabel.gameObject.GetComponent<MeshRenderer>().sortingLayerName = "Default";
        myLabel.gameObject.GetComponent<MeshRenderer>().sortingOrder = 5;

        CC = GameObject.Find("Main Camera").GetComponent<CamController>();
        GC = GameObject.Find("GameController").GetComponent<GameController>();
        myLabel.text = "";
    }

    private void Update()
    {
        GameObject tile = GC.tiles[myTile.GetComponent<FloorTileController>().spotIndex];
        if (tile != null)
        {
            if(tile.GetComponent<TileController>() != null)
            {
                myLabel.text = tile.GetComponent<TileController>().myLabel.ToString();
                switch (tile.GetComponent<TileController>().myLabel)
                {
                    case TileController.label.NULL:
                        myLabel.text = "";
                        break;
                    case TileController.label.IN:
                        myLabel.text = "I";
                        break;
                    case TileController.label.OUT:
                        myLabel.text = "O";
                        break;
                    case TileController.label.CARRY:
                        myLabel.text = "C";
                        break;
                    case TileController.label.RESET:
                        myLabel.text = "R";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                print("tile with no controller?");
            }
        }
    }

    /*
    private void OnMouseOver()
    {
        print("Mouseover!");
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(vKey))
            {
                CC.moveLock = true;

                bool allowed = false;
                char[] keyCodeChars = vKey.ToString().ToCharArray();
                if (keyCodeChars.Length == 1 && System.Char.IsLetterOrDigit(keyCodeChars[0]))
                {
                    //print("Is letter or digit!");
                    allowed = true;
                }

                if (vKey == KeyCode.Space)
                {
                    myLabel.text = "";
                }
                else if (allowed)
                {
                    myLabel.text = "";
                    myLabel.text += vKey;
                }
            }
        }
    }*/

    private void OnMouseDrag()
    {
        print("Dragging!");
        for (int i = 0; i < myOverlay.tiles.Count; i++)
        {
            if (myOverlay.tiles[i] != null)
            {
                myOverlay.tiles[i].transform.parent = pop.transform;
                myOverlay.tiles[i].GetComponent<TileController>().dragAsSelected();
            }
        }

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, -10), 100.0F);

        if (hits.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.transform.GetComponent<FloorTileController>() != null)
            {
                //currentPos = hit.transform.position;
                //spotIndex = hit.transform.GetComponent<FloorTileController>().spotIndex;
                pop.transform.position = hit.transform.position + new Vector3(0, 0, -1);
            }
        }
    }

    private void OnMouseUp()
    {
        print("Undragging!");
        UtilBools.camMoveLock = false;
        for (int i = 0; i < myOverlay.tiles.Count; i++)
        {
            if (myOverlay.tiles[i] != null)
            {
                myOverlay.tiles[i].transform.parent = GameObject.Find("TileFather").transform;
                myOverlay.tiles[i].GetComponent<TileController>().drag = false;
            }
        }
    }
}
