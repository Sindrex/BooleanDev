using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectbarController : MonoBehaviour {

    public int tileId;
    public ActionbarController AC;

    public GameObject SelectItemDragPrefab;
    public GameObject myPrefab;

    private void OnMouseEnter()
    {
        AC.selectedTileS.transform.position = this.transform.position;
        AC.GC.changeItemName(AC.tileHub.getName(tileId));
    }

    private void OnMouseExit()
    {
        AC.GC.closeItemName();
    }

    private void OnMouseDrag()
    {
        if(tileId == 0 || myPrefab != null)
        {
            return;
        }
        GameObject prefab = Instantiate(SelectItemDragPrefab, AC.selectbar.transform);
        prefab.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
        prefab.GetComponent<SelectbarItemDrag>().AC = AC;
        prefab.GetComponent<SelectbarItemDrag>().tileId = tileId;
        myPrefab = prefab;
        AC.GC.audioMixer.playButtonSFX();
    }
}
