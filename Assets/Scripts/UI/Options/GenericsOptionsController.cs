using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GenericsOptionsController : MonoBehaviour {

    //Graphics
    Resolution[] resolutions;
    public Dropdown resolution;
    public Dropdown displayOption;

    //Other
    public Slider moveSlider;
    public InputField undoLimitInput;
    public Toggle autosaveToggle;
    public InputField autosaveTimerInput;

    public void setResolution(int resIndex)
    {
        Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].height, Screen.fullScreen);
    }
    public void setDisplayMode(int displayIndex)
    {
        Screen.fullScreen = (displayIndex == 0); //no borderless fullscreen unfortunately :c
    }

    public int getResolution()
    {
        return resolution.value;
    }
    public int getDisplayOption()
    {
        return displayOption.value;
    }

    public float getMoveSense()
    {
        return moveSlider.value;
    }
    public int getUndoLimit()
    {
        int res = 0;
        int.TryParse(undoLimitInput.text, out res);
        if (res <= 0)
        {
            res = 1;
        }
        return res;
    }
    public int getAutosave()
    {
        return autosaveToggle.isOn ? 1 : 0;
    }
    public int getAutosaveTimer()
    {
        int res = 0;
        int.TryParse(autosaveTimerInput.text, out res);
        if (res <= 0)
        {
            res = 1;
        }
        return res;
    }

    public void saveGenericsSettings()
    {
        print("Saving generics!");
        PlayerPrefs.SetInt(OptionController.genericsOK[0], getResolution());
        PlayerPrefs.SetInt(OptionController.genericsOK[1], getDisplayOption());
        PlayerPrefs.SetFloat(OptionController.genericsOK[2], getMoveSense());
        PlayerPrefs.SetInt(OptionController.genericsOK[3], getUndoLimit());
        PlayerPrefs.SetInt(OptionController.genericsOK[4], getAutosave());
        PlayerPrefs.SetInt(OptionController.genericsOK[5], getAutosaveTimer());
    }

    public static void loadSettings()
    {
        print("Loading static generics!");
        int resIndex = PlayerPrefs.GetInt(OptionController.genericsOK[0]);
        int disIndex = PlayerPrefs.GetInt(OptionController.genericsOK[1]);

        Resolution[] resolutions = Screen.resolutions;
        if (resIndex < 0)
        {
            Screen.SetResolution(1920, 1080, disIndex == 0);
        }
        else
        {
            Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].height, disIndex == 0);
        }
        //QualitySettings.SetQualityLevel(qualIndex);
        Screen.fullScreen = (disIndex == 0);
    }

    public void loadGenericsSettingsUI()
    {
        print("Loading generics to UI!");
        int resIndex = PlayerPrefs.GetInt(OptionController.genericsOK[0]);
        int disIndex = PlayerPrefs.GetInt(OptionController.genericsOK[1]);

        //Resolution
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        resolution.ClearOptions();
        List<string> resolutionNames = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string name = resolutions[i].width + " x " + resolutions[i].height;
            resolutionNames.Add(name);
        }
        resolution.AddOptions(resolutionNames);
        resolution.value = resIndex;
        resolution.RefreshShownValue();

        //Fullscreen/windowed
        displayOption.value = disIndex;

        //Other
        float moveSense = PlayerPrefs.GetFloat(OptionController.genericsOK[2]);
        int undoLimit = PlayerPrefs.GetInt(OptionController.genericsOK[3]);
        int autosave = PlayerPrefs.GetInt(OptionController.genericsOK[4]);
        int autosaveTimer = PlayerPrefs.GetInt(OptionController.genericsOK[5]);

        moveSlider.value = moveSense;
        undoLimitInput.text = "" + undoLimit;
        autosaveToggle.isOn = autosave == 1 ? true : false;
        autosaveTimerInput.text = "" + autosaveTimer;
    }
}
