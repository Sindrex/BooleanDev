using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionController : MonoBehaviour {

    //option keys (OK)
    public static readonly string[] genericsOK = new string[] { "Resolution", "WindowedOption", "MovementSensitivity", "UndoLimit", "Autosave", "AutosaveTimer", "Hints", "FPS"};
    public static readonly string[] controlsOK = new string[] { "K_DeleteTile", "K_PlaceTile", "K_InteractTile", "K_RotateLeftTile",
                                                                "K_RotateRightTile", "K_OpenSelectionBar", "K_Selector",
                                                                "K_DeleteSelected" };
    public static readonly string[] audioOK = new string[] { "MasterVolume", "MusicVolume", "SFXVolume" };

    //Default
    public static readonly int[] defaultGenericsSettings = new int[] { -1, 0, 20, 20, 0, 60, 1, 0 };
    public static readonly string[] defaultControlsSettings = { "R", "Mouse0", "Mouse1", "Q", "E", "Space", "Mouse0", "R"};
    public static readonly int[] defaultAudioSettings = new int[] { 0, -40, -40 };

    public GameObject generics;
    public GameObject controls;
    public GameObject audioObj;

    public GenericsOptionsController genericsCon;
    public ControlsOptionsController controlsCon;
    public AudioOptionsController audioOpCon;

    public MainMenu MM;

    public GameObject appliedText;

    private void Update()
    {
        if (InputController.getInput(InputPurpose.UNIVERSAL_BACK))
        {
            justExit();
        }
    }

    public void loadSettingsUI()
    {
        //load settings from PlayerPrefs into UI
        genericsCon.loadSettingsUI();
        controlsCon.loadControlsSettingsUI();
        audioOpCon.loadAudioSettingsUI();
    }

    public void openGenerics()
    {
        closeAll();
        generics.SetActive(true);
    }
    public void openControls()
    {
        closeAll();
        controls.SetActive(true);
    }
    public void openAudio()
    {
        closeAll();
        audioObj.SetActive(true);
    }

    public void justSave()
    {
        //save all settings in PlayerPrefs
        genericsCon.saveGenericsSettings();
        audioOpCon.saveAudioSettings();
        savedFeedback();

        //controlsCon.saveControlsSettings();

        PlayerPrefs.Save();
    }
    public void justExit()
    {
        loadSettingsUI();
        if(audioOpCon.audioCon != null)
        {
            audioOpCon.audioCon.loadAudioPref();
        }
        GenericsOptionsController.loadSettings();

        //closeAll();
        //gameObject.SetActive(false);

        UtilBools.options = false;
    }

    private void savedFeedback()
    {
        appliedText.SetActive(true);
        StartCoroutine(waitSavedFeedback());
    }

    IEnumerator waitSavedFeedback()
    {
        yield return new WaitForSeconds(0.5f);
        appliedText.SetActive(false);
    }

    private void closeAll()
    {
        MM.audioMixer.playButtonSFX();
        generics.SetActive(false);
        controls.SetActive(false);
        audioObj.SetActive(false);
        appliedText.SetActive(false);
    }
    private void openAll()
    {
        generics.SetActive(true);
        controls.SetActive(true);
        audioObj.SetActive(true);
    }

    public string getControlsKey(InputPurpose purpose)
    {
        switch (purpose)
        {
            case InputPurpose.DELETE_TILE:
                return controlsOK[0];
            case InputPurpose.PLACE_TILE:
                return controlsOK[1];
            case InputPurpose.INTERACT_TILE:
                return controlsOK[2];
            case InputPurpose.TILE_ROTATE_LEFT:
                return controlsOK[3];
            case InputPurpose.TILE_ROTATE_RIGHT:
                return controlsOK[4];
            case InputPurpose.SELECTIONBAR:
                return controlsOK[5];
            case InputPurpose.PAUSE_MENU:
                return controlsOK[6];
            case InputPurpose.SELECTOR:
                return controlsOK[7];
            case InputPurpose.DELETE_SELECTED:
                return controlsOK[8];
        }
        return null;
    }

    public static void setDefaultOptions()
    {
        for (int i = 0; i < genericsOK.Length; i++)
        {
            PlayerPrefs.SetFloat(genericsOK[i], defaultGenericsSettings[i]);
            //print(i);
        }
        for (int i = 0; i < audioOK.Length; i++)
        {
            PlayerPrefs.SetFloat(audioOK[i], defaultAudioSettings[i]);
            //print(i);
        }
        for (int i = 0; i < controlsOK.Length; i++)
        {
            PlayerPrefs.SetString(controlsOK[i], defaultControlsSettings[i]);
            //print(i);
        }
    }
}
