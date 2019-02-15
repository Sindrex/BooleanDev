using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverTipMenu : MonoBehaviour {

    public GameObject hoverTip;
    public Text hoverText;

    public Vector3 displace;
    public string tip;

    private void OnMouseOver()
    {
        hoverTip.SetActive(true);
        hoverTip.transform.localPosition = this.transform.localPosition + displace;
        hoverText.text = tip;
    }
    private void OnMouseExit()
    {
        hoverTip.SetActive(false);
    }
}
