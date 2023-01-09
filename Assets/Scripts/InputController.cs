using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InputKey
{
    R, Q, LShift, Space, E, LControl, MouseScrollForward, MouseScrollBackward, Mouse0, Mouse1, C
};

public enum InputPurpose
{
    DELETE_TILE, PLACE_TILE, INTERACT_TILE, TILE_ROTATE_LEFT, TILE_ROTATE_RIGHT,
    CAM_MOVEMENT_HORIZ, CAM_MOVEMENT_VERT, CAM_MOVEMENT_DRAG, ZOOM_IN, ZOOM_OUT,
    SELECTIONBAR, SELECTIONBAR_LEFT, SELECTIONBAR_RIGHT, SELECTIONBAR_UP,
    PAUSE_MENU,
    ACTIONBAR_LEFT, ACTIONBAR_RIGHT, RESET_ACTIONBAR,
    SELECTOR, DELETE_SELECTED, DUPLICATE, 
    UNIVERSAL_BACK,
    UNDO,
    ANY
}

public static class InputController {

    public static bool getInput(InputPurpose purpose)
    {
        switch (purpose)
        {
            case InputPurpose.UNIVERSAL_BACK:
                return Input.GetKeyDown(KeyCode.Escape);
            case InputPurpose.DELETE_TILE:
                return Input.GetKey(parseKeyCode(OptionController.controlsOK[0]));
            case InputPurpose.PLACE_TILE:
                return Input.GetKey(parseKeyCode(OptionController.controlsOK[1]));
            case InputPurpose.INTERACT_TILE:
                return Input.GetKeyDown(parseKeyCode(OptionController.controlsOK[2]));
            case InputPurpose.TILE_ROTATE_LEFT:
                return Input.GetKeyDown(parseKeyCode(OptionController.controlsOK[3]));
            case InputPurpose.TILE_ROTATE_RIGHT:
                return Input.GetKeyDown(parseKeyCode(OptionController.controlsOK[4]));
            case InputPurpose.ZOOM_OUT:
                return Input.GetAxis("Mouse ScrollWheel") < 0f && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Space);
            case InputPurpose.ZOOM_IN:
                return Input.GetAxis("Mouse ScrollWheel") > 0f && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Space);
            case InputPurpose.SELECTIONBAR:
                return Input.GetKeyDown(parseKeyCode(OptionController.controlsOK[5]));
            case InputPurpose.SELECTIONBAR_LEFT:
                return Input.GetAxis("Mouse ScrollWheel") > 0f && Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift);
            case InputPurpose.SELECTIONBAR_RIGHT:
                return Input.GetAxis("Mouse ScrollWheel") < 0f && Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift);
            case InputPurpose.SELECTIONBAR_UP:
                return Input.GetKeyUp(parseKeyCode(OptionController.controlsOK[5]));
            case InputPurpose.ACTIONBAR_LEFT:
                return Input.GetAxis("Mouse ScrollWheel") > 0f && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Space);
            case InputPurpose.ACTIONBAR_RIGHT:
                return Input.GetAxis("Mouse ScrollWheel") < 0f && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Space);
            case InputPurpose.RESET_ACTIONBAR:
                return Input.GetKeyDown(KeyCode.C);
            case InputPurpose.SELECTOR:
                return Input.GetKeyDown(parseKeyCode(OptionController.controlsOK[6]));
            case InputPurpose.DELETE_SELECTED:
                return Input.GetKeyDown(parseKeyCode(OptionController.controlsOK[7]));
            case InputPurpose.UNDO:
                return Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Z);
            case InputPurpose.DUPLICATE:
                return Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D);
            case InputPurpose.CAM_MOVEMENT_DRAG:
                return Input.GetMouseButton(2);
            case InputPurpose.CAM_MOVEMENT_HORIZ:
                return Input.GetAxis("Horizontal") != 0f;
            case InputPurpose.CAM_MOVEMENT_VERT:
                return Input.GetAxis("Vertical") != 0f;
        }
        return false;
    }

    public static float getMovementInput(InputPurpose purpose)
    {
        switch (purpose)
        {
            case InputPurpose.CAM_MOVEMENT_HORIZ:
                return Input.GetAxis("Horizontal");
            case InputPurpose.CAM_MOVEMENT_VERT:
                return Input.GetAxis("Vertical");
        }
        return 0;
    }

    public static int tryNumbers()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            return 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            return 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            return 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            return 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            return 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            return 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            return 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            return 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            return 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            return 9;
        }
        return -1;
    }

    public static KeyCode parseKeyCode(string s)
    {
        //Debug.Log("InputController:ParseKeyCode(): " + s);
        string myKey = PlayerPrefs.GetString(s);
        KeyCode myKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), myKey);
        return myKeyCode;
    }
}
