using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignManager : MonoBehaviour {

    public GameController GC;
    public CamController CC;

    //Tile: Sign
    public GameObject signObj;
    public Text signText;
    public InputField signInput;
    public GameObject editSignButton;
    public GameObject saveSignButton;
    public SignController currentSignTile;
    public Vector3 startPos;
    public Vector3 offset;
    public bool editing;

    private void Start()
    {
        signInput.gameObject.SetActive(false);
        signObj.SetActive(false);
        editSignButton.SetActive(true);
        saveSignButton.SetActive(false);
        signObj.transform.position = startPos;
    }

    private void Update()
    {
        if(currentSignTile == null || !signObj.activeSelf)
        {
            return;
        }
        if (signObj.transform.position != currentSignTile.transform.position)
        {
            //print(currentSign.transform.localPosition);
            signObj.transform.position = currentSignTile.transform.localPosition + offset;
        }
    }

    public void editSign()
    {
        if (!editing)
        {
            editSignButton.SetActive(false);
            saveSignButton.SetActive(true);
            signInput.text = signText.text;
            signText.gameObject.SetActive(false);
            signInput.gameObject.SetActive(true);
            editing = true;
            UtilBools.playerInteractLock(true);
        }
        else
        {
            editSignButton.SetActive(true);
            saveSignButton.SetActive(false);
            signText.text = signInput.text;
            signText.gameObject.SetActive(true);
            signInput.gameObject.SetActive(false);
            currentSignTile.text = signText.text;
            editing = false;
            UtilBools.playerInteractLock(false);
        }
        GC.audioMixer.playButtonSFX();
    }

    public void closeSign()
    {
        print("Clicked close sign!");
        editSignButton.SetActive(true);
        saveSignButton.SetActive(false);
        signText.text = signInput.text;
        signText.gameObject.SetActive(true);
        signInput.gameObject.SetActive(false);
        currentSignTile.text = signText.text;
        currentSignTile = null;
        editing = false;
        signObj.transform.position = startPos;
        signObj.SetActive(false);
        UtilBools.playerInteractLock(false);
        GC.audioMixer.playButtonSFX();
    }
}
