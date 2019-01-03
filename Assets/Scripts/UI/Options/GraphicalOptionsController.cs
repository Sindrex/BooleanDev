using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GraphicalOptionsController : MonoBehaviour {

    public Dropdown resolution;
    public Dropdown graphicsQuality;
    public Dropdown displayOption;

    Resolution[] resolutions;

    public void setResolution(int resIndex)
    {
        Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].width, Screen.fullScreen);
    }
    public void setGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void setDisplayMode(int displayIndex)
    {
        Screen.fullScreen = (displayIndex == 0); //no borderless fullscreen unfortunately :c
    }

    public void saveGraphicalSettings()
    {
        PlayerPrefs.SetInt(OptionController.graphicsOK[0], getResolution());
        PlayerPrefs.SetInt(OptionController.graphicsOK[1], getGraphicsQuality());
        PlayerPrefs.SetInt(OptionController.graphicsOK[2], getDisplayOption());
    }

    public static void loadSettings()
    {
        int resIndex = PlayerPrefs.GetInt(OptionController.graphicsOK[0]);
        int qualIndex = PlayerPrefs.GetInt(OptionController.graphicsOK[1]);
        int disIndex = PlayerPrefs.GetInt(OptionController.graphicsOK[2]);

        Resolution[] resolutions = Screen.resolutions;
        if(resIndex < 0)
        {
            Screen.SetResolution(1920, 1080, disIndex == 0);
        }
        else
        {
            Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].height, disIndex == 0);
        }
        QualitySettings.SetQualityLevel(qualIndex);
        Screen.fullScreen = (disIndex == 0);
    }

    public void loadGraphicalSettingsUI()
    {
        int resIndex = PlayerPrefs.GetInt(OptionController.graphicsOK[0]);
        int qualIndex = PlayerPrefs.GetInt(OptionController.graphicsOK[1]);
        int disIndex = PlayerPrefs.GetInt(OptionController.graphicsOK[2]);

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

        //Quality
        string[] qualities = QualitySettings.names;
        graphicsQuality.ClearOptions();
        graphicsQuality.AddOptions(qualities.ToList());
        graphicsQuality.value = qualIndex;
        graphicsQuality.RefreshShownValue();
        
        //Fullscreen/windowed
        displayOption.value = disIndex;
    }

    public int getResolution()
    {
        return resolution.value;
    }
    public int getGraphicsQuality()
    {
        return graphicsQuality.value;
    }
    public int getDisplayOption()
    {
        return displayOption.value;
    }
}
