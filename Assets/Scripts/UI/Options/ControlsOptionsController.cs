using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsOptionsController : MonoBehaviour {
    
    public Button[] controlsButtons;
    private Button myButton;
    private int currentId;

    public void onClick(int id)
    {
        //Button readies for a key
        myButton = controlsButtons[id];
        currentId = id;
        print("Controls-clicked: " + id);
    }

    private void OnGUI()
    {
        if (myButton != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                print("Key-event: " + e.keyCode);
                KeyCode myKeyCode = e.keyCode;
                myButton.transform.GetChild(0).GetComponent<Text>().text = myKeyCode.ToString();

                PlayerPrefs.SetString(OptionController.controlsOK[currentId], myKeyCode.ToString());

                myButton = null;
                currentId = -1;
            }
        }
    }

    public void loadControlsSettingsUI()
    {
        for(int i = 0; i < OptionController.controlsOK.Length; i++)
        {
            string myString = PlayerPrefs.GetString(OptionController.controlsOK[i]);
            if (myString.Trim().Equals(""))
            {
                myString = OptionController.defaultControlsSettings[i];
            }
            controlsButtons[i].transform.GetChild(0).GetComponent<Text>().text = myString;

            //KeyCode myKeyCode = InputController.parseKeyCode(myString);
            //PlayerPrefs.SetString(s, myKeyCode.ToString());
        }
    }

    public void resetControls()
    {
        for (int i = 0; i < OptionController.controlsOK.Length; i++)
        {
            PlayerPrefs.SetString(OptionController.controlsOK[i], OptionController.defaultControlsSettings[i]);
        }
        loadControlsSettingsUI();
    }
}
