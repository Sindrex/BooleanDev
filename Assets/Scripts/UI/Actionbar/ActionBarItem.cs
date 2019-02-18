using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarItem : MonoBehaviour {

    public ActionbarController AC;
    public GameObject SelectItemDragPrefab;
    public GameObject myPrefab;

    public int index;
    public int tileId;

    private void OnMouseOver()
    {
        if (UtilBools.actionBarLock)
        {
            return;
        }
        AC.showNameA(index);
    }

    void OnMouseDown()
    {
        if (UtilBools.actionBarLock)
        {
            return;
        }
        AC.selectedA = index;
        AC.actionBarSelect();
        AC.tiler.transform.rotation = Quaternion.Euler(0, 0, 0);
        AC.showNameA(index);
    }

    private void OnMouseExit()
    {
        if (UtilBools.actionBarLock)
        {
            return;
        }
        AC.GC.closeItemName();
    }

    private void OnMouseDrag()
    {
        if (tileId == 0 || myPrefab != null || !AC.selectbar.activeSelf)
        {
            return;
        }
        GameObject prefab = Instantiate(SelectItemDragPrefab, AC.selectbar.transform);
        prefab.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
        prefab.GetComponent<SelectbarItemDrag>().AC = AC;
        prefab.GetComponent<SelectbarItemDrag>().tileId = tileId;
        myPrefab = prefab;

        GetComponent<Image>().color = Color.black;
        tileId = 0;
        AC.activePrefabs[index - AC.firstUnavailableA] = AC.tileHub.getPrefab(0);
    }
}
