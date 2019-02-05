using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectbarItemDrag : MonoBehaviour {

    public int tileId;
    public ActionbarController AC;

    private void Update()
    {
        if (!InputController.getInput(InputPurpose.PLACE_TILE))
        {
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(this.transform.position, new Vector3(0, 0, -1), 100.0F);
            foreach(RaycastHit2D hit in hits)
            {
                if (hit.transform.CompareTag("Actionbar"))
                {
                    int index = hit.transform.GetComponent<ActionBarItem>().index;
                    if(index < AC.firstUnavailableA)
                    {
                        break;
                    }
                    AC.activePrefabs[index - AC.firstUnavailableA] = AC.tileHub.getPrefab(tileId);
                    hit.transform.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
                    hit.transform.GetComponent<Image>().color = Color.white;
                    hit.transform.GetComponent<ActionBarItem>().tileId = tileId;
                }
            }
            Destroy(this.gameObject);
        }
        else
        {
            float z = 7.9f;
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, z));
        }
    }
}
