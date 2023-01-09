using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TutorialAction {

    public string type;
    public int targetID; //tileID, controlID or buttonID
    public int spotIndex; //for place tile
    public TutorialHint hint;
    public TutorialArrow arrow;

    public ActionType getActionType()
    {
        switch (type)
        {
            case "text":
                return ActionType.Text;
            case "place":
                return ActionType.Place;
            case "button":
                return ActionType.Button;
            case "control":
                return ActionType.Control;
        }
        return ActionType.Text;
    }
    public enum ActionType 
    {
        Text = 0,
        Place = 1,
        Button = 2,
        Control = 3
    }

    public enum ButtonIDs
    {
        SELECT = 0,
        MOVE_SELECT = 1,
        DUPE_SELECT = 2,
        PUZZLE_OBJECTIVE = 3,
        PUZZLE_PLAY = 4,
        PUZZLE_STEP_PLAY = 5,
        COMPONENTS = 6
    }

    public enum ControlIDs
    {
        WASD = 0,
        DRAG = 1,
        ZOOM = 2,
        SCROLL = 3,
        ROTATE = 4,
        SPACE = 5,
        DELETE = 6,
        UNDO = 7,
    }
}

[System.Serializable]
public class TutorialArrow
{
    public bool enable;
    public int x;
    public int y;
    public int rotz;
}

[System.Serializable]
public class TutorialHint
{
    public string text;
    public int x;
    public int y;
}