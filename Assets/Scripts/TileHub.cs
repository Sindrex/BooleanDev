using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHub : MonoBehaviour {

    public Vector3 tileDimensions;

    [SerializeField]
    private string[] tileNames;

    [SerializeField]
    private GameObject[] tilePrefabs;

    [SerializeField]
    private int[] IOPickerExceptions;

    public List<int> actionbarBanned;

    private void Start()
    {
        if(tileNames.Length != tilePrefabs.Length)
        {
            throw new System.Exception("TileHub: Prefab-list and names-list is not the same!");
        }
    }

    public string getName(int id)
    {
        if(id < tileNames.Length)
        {
            return tileNames[id];
        }
        return "Peekabo!";
    }

    public GameObject getPrefab(int id)
    {
        if (id < tilePrefabs.Length)
        {
            return tilePrefabs[id];
        }
        return tilePrefabs[0];
    }

    public int getCount()
    {
        return tilePrefabs.Length - actionbarBanned.Count;
    }

    public bool IOPickerException(int id)
    {
        for(int i = 0; i < IOPickerExceptions.Length; i++)
        {
            if(IOPickerExceptions[i] == id)
            {
                return true;
            }
        }
        return false;
    }
}
