using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsController : MonoBehaviour {

    public GameController GC;
    public TileHub hub;

    public GameObject listItemPrefab;
    public GameObject listItemContainer;

    public GameObject loading;

    public void createStats()
    {
        //Remove old items
        for(int i = listItemContainer.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(listItemContainer.transform.GetChild(i).gameObject);
            //print(i);
        }

        loading.SetActive(true);

        //Create items
        GameObject prefab = Instantiate(listItemPrefab, listItemContainer.transform);
        prefab.GetComponent<StatsItemController>().myName.text = "All tiles";
        prefab.GetComponent<StatsItemController>().quantity.text = countAll() + "";

        for(int i = 0; i < hub.getCount(); i++)
        {
            //string name = hub.getName(i);
            prefab = Instantiate(listItemPrefab, listItemContainer.transform);
            prefab.GetComponent<StatsItemController>().myName.text = hub.getName(i);
            prefab.GetComponent<StatsItemController>().quantity.text = countTile(i) + "";
        }

        prefab = Instantiate(listItemPrefab, listItemContainer.transform);
        prefab.GetComponent<StatsItemController>().myName.text = "Powered";
        prefab.GetComponent<StatsItemController>().quantity.text = countPowered() + "";

        prefab = Instantiate(listItemPrefab, listItemContainer.transform);
        prefab.GetComponent<StatsItemController>().myName.text = "Unpowered";
        prefab.GetComponent<StatsItemController>().quantity.text = countUnpowered() + "";

        loading.SetActive(false);
    }

    private int countTile(int id)
    {
        int count = 0;
        foreach(GameObject g in GC.tiles)
        {
            if(g != null && g.GetComponent<TileController>().ID == id)
            {
                count++;
            }
        }
        return count;
    }

    private int countAll()
    {
        int count = 0;
        foreach (GameObject g in GC.tiles)
        {
            if(g != null)
            {
                count++;
            }

        }
        return count;
    }

    private int countPowered()
    {
        int count = 0;
        foreach (GameObject g in GC.tiles)
        {
            if (g != null && g.GetComponent<TileController>().beingPowered)
            {
                count++;
            }
        }
        return count;
    }

    private int countUnpowered()
    {
        return countAll() - countPowered();
    }
}
