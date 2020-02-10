﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {

    public Text myText;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        //rudementary, but if low impact on fps its OK
        int displayFPS = PlayerPrefs.GetInt(OptionController.genericsOK[7]);
        if(displayFPS == 1)
        {
            myText.text = (int)(1.0f / Time.smoothDeltaTime) + " fps";
        }
        else
        {
            myText.text = "";
        }
    }
}
