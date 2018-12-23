using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignManager : MonoBehaviour {

    public GameController GC;
    public CamController CC;

    //Tile: Sign
    public GameObject signHover;
    public Text signText;
    public InputField signInput;
    public Button signButton;
    public SignController currentSign;
    public Vector3 startPos;
    public bool pinned;

    private void Start()
    {
        signInput.gameObject.SetActive(false);
        signHover.SetActive(false);
        signHover.transform.position = startPos;
    }

    public void editSign()
    {
        if (!pinned)
        {
            signButton.transform.GetChild(0).GetComponent<Text>().text = "Confirm";
            signInput.text = signText.text;
            signText.gameObject.SetActive(false);
            signInput.gameObject.SetActive(true);
            pinned = true;
            UtilBools.camMoveLock = true;
        }
        else
        {
            signButton.transform.GetChild(0).GetComponent<Text>().text = "Edit";
            signText.text = signInput.text;
            signText.gameObject.SetActive(true);
            signInput.gameObject.SetActive(false);
            currentSign.text = signText.text;
            pinned = false;
            UtilBools.camMoveLock = false;
        }
    }

    public void closeSign()
    {
        signButton.transform.GetChild(0).GetComponent<Text>().text = "Edit";
        signText.text = signInput.text;
        signText.gameObject.SetActive(true);
        signInput.gameObject.SetActive(false);
        currentSign.text = signText.text;
        pinned = false;
        UtilBools.camMoveLock = false;
        signHover.transform.position = startPos;
    }
}
