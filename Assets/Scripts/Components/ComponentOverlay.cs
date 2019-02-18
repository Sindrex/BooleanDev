using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentOverlay : MonoBehaviour {

    public ComponentSave myComp;
    public List<GameObject> tiles = new List<GameObject>();
    public int stopIndex = 0;

    public GameObject overlayTilePrefab;
    public GameObject pop;
    public TextMesh nameMesh;
    public int nameOrder;

    private void Start()
    {
        nameMesh.gameObject.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        nameMesh.gameObject.GetComponent<MeshRenderer>().sortingOrder = nameOrder;
    }

    public void setParams(ComponentSave myComp, List<GameObject> tiles, List<GameObject> floorTiles)
    {
        this.myComp = myComp;
        this.tiles = tiles;

        //TileHub hub = GameObject.Find("Manager").GetComponent<TileHub>();

        //float x = //floorTiles[0].transform.position.x - (myComp.length / 2.0f);
        //float y = //floorTiles[0].transform.position.y + (myComp.height / 2.0f);

        //print("x/y: " + x + "/" + y);

        List<GameObject> overlayTiles = new List<GameObject>();
        //Create overlaytiles
        foreach (GameObject go in floorTiles)
        {
            GameObject prefab = Instantiate(overlayTilePrefab, pop.transform);
            prefab.transform.position = go.transform.position + new Vector3(0, 0, -2);
            prefab.GetComponent<OverlayTile>().myTile = go;
            prefab.GetComponent<OverlayTile>().pop = pop;
            prefab.GetComponent<OverlayTile>().myOverlay = this;
            overlayTiles.Add(prefab);
        }

        float x = (overlayTiles[0].transform.localPosition.x + overlayTiles[myComp.length - 1].transform.localPosition.x)/2;//floorTiles[0].transform.position.x - (myComp.length / 2.0f);
        //float x = overlayTiles[floorTiles.Count - 1].transform.position.x + dx;
        float y = (overlayTiles[floorTiles.Count - myComp.length].transform.localPosition.y - overlayTiles[0].transform.localPosition.y)/2;//floorTiles[0].transform.position.y + (myComp.height / 2.0f);
        //float y = overlayTiles[floorTiles.Count - myComp.length].transform.position.y + dy;
        //print(x +"/"+ y);
        transform.localPosition = new Vector3(x, y, -2);
        transform.localScale = new Vector3(myComp.length * 1.6f, myComp.height * 1.6f, 1);

        foreach (GameObject go in tiles)
        {
            if(go != null)
            {
                go.GetComponent<TileController>().myCompOverlay = transform.parent.gameObject;
            }
        }

        nameMesh.text = myComp.name;
        nameMesh.transform.localPosition = new Vector3(x, y, -2);
    }

    /*
    private void OnMouseDrag()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] != null)
            {
                tiles[i].transform.parent = pop.transform;
                tiles[i].GetComponent<TileController>().dragAsSelected();
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
    }*/

    private void OnMouseUp()
    {
        //print("Undragging!");
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] != null)
            {
                tiles[i].transform.parent = GameObject.Find("TileFather").transform;
                tiles[i].GetComponent<TileController>().drag = false;
            }
        }
    }
}
