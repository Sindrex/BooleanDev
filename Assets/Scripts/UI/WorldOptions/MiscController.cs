using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiscController : MonoBehaviour {

    public GameController GC;

    public Toggle lockTilesToggle;
    public Toggle noPlaceToggle;

    public void loadSettingsUI()
    {
        //int lockTiles = PlayerPrefs.GetInt(WorldOptionsController.miscKeys[0]);
        bool lockTiles = Game.current.lockTiles;
        lockTilesToggle.isOn = lockTiles;

        bool noPlace = Game.current.noPlaceTiles;
        noPlaceToggle.isOn = noPlace;
    }

    public void saveSettings()
    {
        Game.current.lockTiles = lockTilesToggle.isOn;
        Game.current.noPlaceTiles = noPlaceToggle.isOn;
    }

    public void toggleLockTiles()
    {
        UtilBools.tileLock = lockTilesToggle.isOn;
    }

    public void toggleNoPlace()
    {
        //print("woo: " + noPlaceToggle.isOn);
        UtilBools.noPlaceTile = noPlaceToggle.isOn;
    }

    public void clickToggle()
    {
        GC.audioMixer.playButtonSFX();
    }
}
