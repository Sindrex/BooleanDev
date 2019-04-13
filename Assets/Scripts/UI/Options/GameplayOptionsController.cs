using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayOptionsController : MonoBehaviour {

    //public CamController camController;

    public Slider mouseSlider;
    public Slider moveSlider;
    public InputField undoLimitInput;

    public void saveGameplaySettings()
    {
        PlayerPrefs.SetFloat(OptionController.gameplayOK[0], getMouseSense());
        PlayerPrefs.SetFloat(OptionController.gameplayOK[1], getMoveSense());
        PlayerPrefs.SetInt(OptionController.gameplayOK[2], getUndoLimit());
    }

    public void loadGameplaySettingsUI()
    {
        //Load from playerPrefs
        float mouseSense = PlayerPrefs.GetFloat(OptionController.gameplayOK[0]);
        float moveSense = PlayerPrefs.GetFloat(OptionController.gameplayOK[1]);
        int undoLimit = PlayerPrefs.GetInt(OptionController.gameplayOK[2]);

        mouseSlider.value = mouseSense;
        moveSlider.value = moveSense;
        undoLimitInput.text = "" + undoLimit;
    }

    public void setMouseSense(float vol)
    {
        //do it?
    }
    public void setMoveSense(float vol)
    {
        //camController.speed = vol; //10-30?
    }

    public float getMouseSense()
    {
        return mouseSlider.value;
    }
    public float getMoveSense()
    {
        return moveSlider.value;
    }
    public int getUndoLimit()
    {
        int res = 0;
        int.TryParse(undoLimitInput.text, out res);
        if(res <= 0)
        {
            res = 1;
        }
        return res;
    }
}
