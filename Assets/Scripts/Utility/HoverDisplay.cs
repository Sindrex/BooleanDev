using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverDisplay : MonoBehaviour {

    public GameObject info;

    private void OnMouseEnter()
    {
        info.SetActive(true);
    }
    private void OnMouseExit()
    {
        info.SetActive(false);
    }
}
