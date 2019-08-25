using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverController : MonoBehaviour {

    public bool drag;
    Vector3 prevPos;
    Vector3 currentPos;

    public GameObject selectedFather;
    public GameController GC;

    public Sprite regular;
    public Sprite hover;
    public string displayName;

    private void OnMouseEnter()
    {
        this.GetComponent<SpriteRenderer>().sprite = hover;
    }

    private void OnMouseOver()
    {
        GC.changeItemName(displayName);
    }

    private void OnMouseExit()
    {
        this.GetComponent<SpriteRenderer>().sprite = regular;
        GC.closeItemName();
    }

    private void OnMouseDrag()
    {
        this.GetComponent<SpriteRenderer>().sprite = regular;
        prevPos = currentPos;

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, -10), 100.0F);

        if(hits.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if(hit.transform.GetComponent<FloorTileController>() != null)
            {
                currentPos = hit.transform.position;
            }
        }
        
        if (!drag)
        {
            prevPos = currentPos;
        }

        if (!drag)
        {
            GC.dragSelected();
        }
        drag = true;
        if (prevPos != currentPos)
        {
            selectedFather.transform.position += (currentPos - prevPos);
            //print("Fatherpos: " + selectedFather.transform.position + ", cPos: " + currentPos + ", pPos: " + prevPos);
        }
        
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 7));
    }

    private void OnMouseUp()
    {
        if(GC.cacheList[GC.cacheList.Count - 1] != null && GC.cacheList[GC.cacheList.Count - 1].gameObject.activeSelf) //!= null
        {
            //print("Mover: Cachelist not null & active");
            transform.position = GC.cacheList[GC.cacheList.Count - 1].transform.position + new Vector3(0,0,-2);
        }
        else
        {
            //print("Mover: Cachelist entry null");
            transform.position = prevPos;
            if(transform.position.z != -2)
            {
                //force z = -2
                transform.position = new Vector3(transform.position.x, transform.position.y, -2);
            }
        }
        drag = false;
        GC.unDragSelected();
        //print("Mover Mouseup: " + transform.position);
    }
}
