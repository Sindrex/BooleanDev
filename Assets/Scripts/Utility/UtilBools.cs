﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilBools {

    public static bool actionBarLock = false;
    public static bool noPlaceTile = false;
    public static bool selectLock = false;
    public static bool camMoveLock = false;
    public static bool paused = false;
    public static bool options = false;
    public static bool worldOptions = false;
    public static bool tileLock = false;
    public static bool expandLoading = false;

    public static void pausedBools(Game game, bool state)
    {
        actionBarLock = state;
        noPlaceTile = state;
        tileLock = state;
        selectLock = state;
        camMoveLock = state;
        paused = state;

        if (!state)
        {
            setDefaultGame(game);
        }
    }

    public static void setDefaultGame(Game game)
    {
        tileLock = game.lockTiles;
        noPlaceTile = game.noPlaceTiles;
    }

    public static void setAll(bool state)
    {
        actionBarLock = state;
        noPlaceTile = state;
        selectLock = state;
        camMoveLock = state;
        paused = state;
        options = state;
        worldOptions = state;
        tileLock = state;
        expandLoading = state;
    }
}
