using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderLayerPicker : MonoBehaviour {

    //REQUIRES MESHRENDERER

    public string SortingLayerName = "Default";
    public int SortingOrder = 0;

    // Use this for initialization
    void Start () {
        gameObject.GetComponent<MeshRenderer>().sortingLayerName = SortingLayerName;
        gameObject.GetComponent<MeshRenderer>().sortingOrder = SortingOrder;
    }
}
