using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverDisplay : MonoBehaviour {

    public GameController GC;
    public GameObject info;

    private void OnMouseEnter()
    {
        info.SetActive(true);
        GC.audioMixer.playButtonSFX();
    }
    private void OnMouseExit()
    {
        info.SetActive(false);
    }
}
