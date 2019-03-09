﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public PuzzleArray puzzleArray;

    public GameObject mainMenu;
    public GameObject hoverTip;

    public static readonly string openAtStartPrefKey = "openAtStart";
    public static readonly string justOpenedGame = "justOpened";

    //Puzzles
    public GameObject puzzles;
    public Text puzzleWorldNameObj;
    public GameObject[] puzzleWorlds;
    public string[] puzzleWorldNames;
    private int puzzleId = 0;
    public Button puzzleNext;
    public Button puzzlePrev;
    public static readonly string puzzlePrefKey = "puzzleDone";
    public int puzzlesDone = 0;
    public int[] puzzlesDoneNeeded;
    public Text puzzlesDoneLock;

    //Sandbox
    public SandboxController SC;
    public GameObject sandbox;
    public GameObject newWorld;
    public InputField worldNameInput;
    public InputField worldLengthInput;
    public InputField worldHeightInput;

    //Options
    public GameObject options;
    public OptionController optionCon;
    public Vector3 optionsPos;
    public float optionsCamSize;

    //Credits
    public Vector3 creditsPos;

    //Cam movement
    public Camera cam;
    public Vector3 mainPos;
    private bool camLerp;
    private float lerpFactor = 0f;
    public float mainCamSize;
    private bool forwards;
    public Vector3 currentTarget;
    public float currentTargetCamSize;

    private readonly string FIRST_TIMER_KEY = "firstTimer"; 

    //Change Cursor (weird)
    /*public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }*/

    private void Start()
    {
        SaveLoad.Load();
        setDefaultOptions();

        puzzlesDone = 0;
        for (int i = 0; i < 1000; i++)
        {
            if (PlayerPrefs.GetInt(puzzlePrefKey + i, 0) == 1)
            {
                puzzlesDone++;
            }
        }

        /*
        int start = PlayerPrefs.GetInt(MainMenu.justOpenedGame, 0);
        PlayerPrefs.SetInt(MainMenu.justOpenedGame, 1);

        if (start == 0)
        {
            openMainMenu();
        }

        switch (PlayerPrefs.GetString(MainMenu.openAtStartPrefKey, "menu"))
        {
            case "sandbox":
                openSandbox();
                break;
            case "puzzle":
                puzzleOpen();
                break;
            case "menu":
                openMainMenu();
                break;
            default:
                openMainMenu();
                break;
        }*/
    }

    public void closeAll()
    {
        mainMenu.SetActive(false);
        puzzles.SetActive(false);
        puzzleId = 0;

        sandbox.SetActive(false);
        newWorld.SetActive(false);

        options.SetActive(false);
        hoverTip.SetActive(false);

        GameObject[] savedGamesPrefabs = GameObject.FindGameObjectsWithTag("saves");
        for(int i = 0; i < savedGamesPrefabs.Length; i++)
        {
            Destroy(savedGamesPrefabs[i]);
        }
        Destroy(GameObject.FindGameObjectWithTag("cancel"));
    }
    public void openMainMenu()
    {
        closeAll();
        mainMenu.SetActive(true);
    }

    public void puzzleOpen()
    {
        closeAll();
        puzzles.SetActive(true);
        puzzleWorldNameObj.text = puzzleWorldNames[0];
        puzzleDoneLockUpdate(0);
        for(int i = 0; i < puzzleWorlds.Length; i++)
        {
            puzzleWorlds[i].SetActive(false);
        }
        puzzleWorlds[0].SetActive(true);
        puzzlePrev.interactable = false;
        puzzleNext.interactable = true;
    }
    public void puzzleNextWorld()
    {
        puzzleNext.interactable = true;
        puzzlePrev.interactable = true;
        for (int i = 0; i < puzzleWorlds.Length; i++)
        {
            if (puzzleWorlds[i].activeSelf && (i + 1) < puzzleWorlds.Length)
            {
                puzzleWorlds[i].SetActive(false);
                puzzleWorlds[i + 1].SetActive(true);
                puzzleWorldNameObj.text = puzzleWorldNames[i + 1];
                puzzleDoneLockUpdate(i + 1);
                if (i + 1 == puzzleWorlds.Length - 1)
                {
                    puzzleNext.interactable = false;
                }
                return;
            }
        }
    }
    public void puzzlePrevWorld()
    {
        puzzleNext.interactable = true;
        puzzlePrev.interactable = true;
        for (int i = 0; i < puzzleWorlds.Length; i++)
        {
            if (puzzleWorlds[i].activeSelf && (i - 1) >= 0)
            {
                puzzleWorlds[i].SetActive(false);
                puzzleWorlds[i - 1].SetActive(true);
                puzzleWorldNameObj.text = puzzleWorldNames[i - 1];
                puzzleDoneLockUpdate(i - 1);
                if(i - 1 == 0)
                {
                    puzzlePrev.interactable = false;
                }
                return;
            }
        }
    }
    public void puzzleDoneLockUpdate(int i)
    {
        puzzlesDoneLock.text = puzzlesDone + "/" + puzzlesDoneNeeded[i];
        if (puzzlesDone >= puzzlesDoneNeeded[i])
        {
            puzzlesDoneLock.color = Color.green;
        }
        else
        {
            puzzlesDoneLock.color = Color.red;
        }
    }
    public void puzzlePickLevel(int id)
    {
        puzzleId = id;
        if (puzzleId >= 0)
        {
            if (puzzleId < puzzleArray.EOTP_puzzles.Length + 1)
            {
                //  print(PA.EOTP_puzzles.Length + "/" + PA.EOTP_puzzles[puzzleIndex - 1]);
                Game.current = new Game(puzzleArray.EOTP_puzzles[puzzleId]);
                //print(Game.current.puzzle);
                SceneManager.LoadScene("Main");
            }
            else
            {
                throw new System.Exception("Puzzle not implemented");
            }
        }
    }

    public void openSandbox()
    {
        closeAll();
        sandbox.SetActive(true);
        SC.open();
    }
    public void openSandboxNewWorld()
    {
        closeAll();
        newWorld.SetActive(true);
        Game.current = new Game(null);
    }
    public void createSandbox()
    {
        worldNameInput.gameObject.GetComponent<Image>().color = Color.white;
        worldLengthInput.gameObject.GetComponent<Image>().color = Color.white;
        worldHeightInput.gameObject.GetComponent<Image>().color = Color.white;

        string worldName = worldNameInput.text;
        int worldLength = 0;
        int.TryParse(worldLengthInput.text, out worldLength);
        int worldHeight = 0;
        int.TryParse(worldHeightInput.text, out worldHeight);

        bool ok = true;
        if (worldName.Length <= 0)
        {
            ok = false;
            worldNameInput.gameObject.GetComponent<Image>().color = Color.red;
        }
        if(worldLength <= 0)
        {
            ok = false;
            worldLengthInput.gameObject.GetComponent<Image>().color = Color.red;
        }
        if (worldHeight <= 0)
        {
            ok = false;
            worldHeightInput.gameObject.GetComponent<Image>().color = Color.red;
        }


        if (ok)
        {
            Game.current.gameName = worldNameInput.text;
            Game.current.length = worldLength;
            Game.current.height = worldHeight;
            //Save the current Game as a new saved Game
            SaveLoad.Save();
            //Move on to game...
            SceneManager.LoadScene("Main");
        }
    }

    public void openOptions()
    {
        //print("MainMenu: OpenOptions(): Options not implemented");
        options.SetActive(true);

        camLerp = true;
        forwards = true;
        currentTarget = optionsPos;
        currentTargetCamSize = optionsCamSize;
        optionCon.gameObject.SetActive(true);
        optionCon.loadSettingsUI();
    }
    public void saveExitOptions()
    {
        optionCon.saveExitMenu();
        camLerp = true;
        forwards = false;
    }

    private void Update()
    {
        if (camLerp)
        {
            //print("yo");
            if (forwards)
            {
                doLerp(currentTarget, currentTargetCamSize);
            }
            else
            {
                doLerp(mainPos, mainCamSize);
            }
        }
    }

    public void doLerp(Vector3 target, float camSize)
    {
        //print("yo2");
        float x = cam.transform.position.x;
        float y = cam.transform.position.y;
        float z = cam.transform.position.z;
        Vector3 lerpPos = new Vector3(Mathf.Lerp(x, target.x, lerpFactor), Mathf.Lerp(y, target.y, lerpFactor), z);
        cam.transform.position = lerpPos;

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, camSize, lerpFactor);
        lerpFactor += 0.5f * Time.deltaTime;

        if (cam.transform.position == target)
        {
            camLerp = false;
            lerpFactor = 0;
        }
    }

    public void openCredits()
    {
        camLerp = true;
        forwards = true;
        currentTarget = creditsPos;
        currentTargetCamSize = mainCamSize;
    }
    public void closeCredits()
    {
        camLerp = true;
        forwards = false;
    }

    public void website()
    {
        print("MainMenu: website()");
        Application.OpenURL("http://sindrex.wordpress.com/game-boolean/");
    }

    public void quit()
    {
        //PlayerPrefs.SetInt(MainMenu.justOpenedGame, 0);
        //print("setting justOpenedGame");
        Application.Quit();
    }

    public void setDefaultOptions()
    {
        print("MainMenu: setDefaultOptions()");
        int defaultOption = PlayerPrefs.GetInt(FIRST_TIMER_KEY);
        if(defaultOption <= 0)
        {
            OptionController.setDefaultOptions();
            PlayerPrefs.SetInt(FIRST_TIMER_KEY, 1);
        }
        else
        {
            print("MainMenu: setDefaultOptions(): Not first timer");
        }
    }
    public void resetDefOptions()
    {
        print("MainMenu: resetDefOptions()");
        PlayerPrefs.SetInt(FIRST_TIMER_KEY, 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void resetPuzzles()
    {
        for(int i = 0; i < 1000; i++)
        {
            PlayerPrefs.SetInt(MainMenu.puzzlePrefKey + i, 0);
        }
        SceneManager.LoadScene("Menu");
    }
    public void unlockPuzzles()
    {
        for (int i = 0; i < 1000; i++)
        {
            PlayerPrefs.SetInt(MainMenu.puzzlePrefKey + i, 1);
        }
        SceneManager.LoadScene("Menu");
    }
}
