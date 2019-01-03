using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionController : MonoBehaviour {

    //option keys
    public static readonly string[] graphicsOK = new string[] { "ScreenSize", "GraphicsQuality", "WindowedOption" };
    public static readonly string[] gameplayOK = new string[] { "MouseSensitivity", "MovementSensitivity", "UndoLimit" };
    public static readonly string[] controlsOK = new string[] { "K_DeleteTile", "K_PlaceTile", "K_InteractTile", "K_RotateLeftTile",
                                                                "K_RotateRightTile", "K_OpenSelectionBar", "K_OpenPause", "K_Selector",
                                                                "K_DeleteSelected" };
    public static readonly string[] audioOK = new string[] { "MasterVolume", "MusicVolume", "SFXVolume" };

    //Default
    public static readonly int[] defaultAudioSettings = new int[] { 0, -40, -40 };
    public static readonly int[] defaultGameplaySettings = new int[] { 20, 20, 10 };
    public static readonly int[] defaultGraphicsSettings = new int[] { -1 , 4 , 0 };
    public static readonly string[] defaultControlsSettings = { "R", "Mouse0", "Mouse1", "Q", "E", "Space", "Escape", "Mouse0", "R"};

    public GameObject graphics;
    public GameObject gameplay;
    public GameObject controls;
    public GameObject audioObj;

    public GraphicalOptionsController graphicalCon;
    public GameplayOptionsController gameplayCon;
    public ControlsOptionsController controlsCon;
    public AudioOptionsController audioCon;

    public GameController GC;

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

        controlsCon.loadControlsSettingsUI();
        graphicalCon.loadGraphicalSettingsUI();
        gameplayCon.loadGameplaySettingsUI();
        audioCon.loadAudioSettingsUI();

        openGraphics();
    }

    public void openGraphics()
    {
        closeAll();
        graphics.SetActive(true);
    }
    public void openGameplay()
    {
        closeAll();
        gameplay.SetActive(true);
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

    public void saveExit()
    {
        print("OptionController:saveExit()");
        openAll();

        //save all settings in PlayerPrefs
        graphicalCon.saveGraphicalSettings();
        gameplayCon.saveGameplaySettings();
        audioCon.saveAudioSettings();

        //controlsCon.saveControlsSettings(); (are saved continuisly)

        PlayerPrefs.Save();
        loadSettingsUI();
        closeAll();

        UtilBools.options = false;
        GC.togglePaused();

        gameObject.SetActive(false);
    }
    public void saveExitMenu()
    {
        print("OptionController:saveExitMenu()");
        openAll();

        //save all settings in PlayerPrefs
        graphicalCon.saveGraphicalSettings();
        gameplayCon.saveGameplaySettings();
        audioCon.saveAudioSettings();

        //controlsCon.saveControlsSettings();

        PlayerPrefs.Save();
        closeAll();
        openGraphics();
        //loadSettings();
    }
    public void justExit()
    {
        closeAll();
        gameObject.SetActive(false);

        UtilBools.options = false;
        GC.togglePaused();
    }
    private void closeAll()
    {
        graphics.SetActive(false);
        gameplay.SetActive(false);
        controls.SetActive(false);
        audioObj.SetActive(false);
    }
    private void openAll()
    {
        graphics.SetActive(true);
        gameplay.SetActive(true);
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
        for(int i = 0; i < graphicsOK.Length; i++)
        {
            PlayerPrefs.SetInt(graphicsOK[i], defaultGraphicsSettings[i]);
        }
        for (int i = 0; i < gameplayOK.Length; i++)
        {
            PlayerPrefs.SetFloat(gameplayOK[i], defaultGameplaySettings[i]);
        }
        for (int i = 0; i < audioOK.Length; i++)
        {
            PlayerPrefs.SetFloat(audioOK[i], defaultAudioSettings[i]);
        }
        for (int i = 0; i < controlsOK.Length; i++)
        {
            PlayerPrefs.SetString(controlsOK[i], defaultControlsSettings[i]);
        }
    }
}
