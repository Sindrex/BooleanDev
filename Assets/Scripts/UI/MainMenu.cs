using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public PuzzleArray PA;

    public GameObject MainMenuObj;
    public GameObject Puzzles;

    public GameObject Sandbox;
    public GameObject newWorld;
    public InputField worldNameInput;
    public GameObject loadWorld;
    public GameObject deleteWorld;
    public GameObject cancelPrefab;
    public GameObject savedGamesPrefab;
    public GameObject deleteSavedGamesPrefab;

    public GameObject options;

    private int puzzleIndex = 0;

    public GameObject ScrollContent;
    public GameObject ScrollContentDelete;

    public OptionController optionCon;
    public Camera cam;
    public Vector3 optionsPos;
    public Vector3 mainPos;
    private bool camLerp;
    private float lerpFactor = 0f;
    private bool isOptions;
    public float mainCamSize;
    public float optionsCamSize;

    private readonly string FIRST_TIMER_KEY = "firstTimer"; 

    //Change Cursor (weird)
    /*public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }*/

    public enum Menu {
		MainMenu,
        Sandbox,
        Puzzles,
		NewGame,
		Continue,
        DeleteGame
	}

	public Menu currentMenu;

    private void Start()
    {
        OpenMainMenu();
        SaveLoad.Load();
        setDefaultOptions();
    }

    public void closeAll()
    {
        MainMenuObj.SetActive(false);
        Puzzles.SetActive(false);
        puzzleIndex = 0;

        Sandbox.SetActive(false);
        newWorld.SetActive(false);
        loadWorld.SetActive(false);
        deleteWorld.SetActive(false);

        options.SetActive(false);

        GameObject[] savedGamesPrefabs = GameObject.FindGameObjectsWithTag("saves");
        for(int i = 0; i < savedGamesPrefabs.Length; i++)
        {
            Destroy(savedGamesPrefabs[i]);
        }
        Destroy(GameObject.FindGameObjectWithTag("cancel"));
    }
    public void OpenMainMenu()
    {
        closeAll();
        MainMenuObj.SetActive(true);
    }
    public void OpenPuzzles()
    {
        closeAll();
        Puzzles.SetActive(true);
    }
    public void pickLevel(int index)
    {
        puzzleIndex = index;
    }
    public void playPuzzle()
    {
        if(puzzleIndex > 0)
        {
            if(puzzleIndex < PA.puzzles.Length + 1)
            {
                Game.current = new Game(PA.puzzles[puzzleIndex - 1]);
                SceneManager.LoadScene(0);
            }
            else
            {
                throw new System.Exception("Puzzle not implemented");
            }
        }
    }

    public void OpenSandbox()
    {
        closeAll();
        Sandbox.SetActive(true);
    }
    public void OpenSandboxNewWorld()
    {
        closeAll();
        newWorld.SetActive(true);
        Game.current = new Game(null);
    }
    public void createSandbox()
    {
        if(worldNameInput.text.Length > 0)
        {
            Game.current.gameName = worldNameInput.text;
            //Save the current Game as a new saved Game
            SaveLoad.Save();
            //Move on to game...
            SceneManager.LoadScene(0);
        }
    }
    public void OpenSandboxLoadWorld()
    {
        closeAll();
        loadWorld.SetActive(true);
        destroyButtons(ScrollContent);
        createButtons(savedGamesPrefab, ScrollContent);
    }
    public void OpenSandboxDeleteWorld()
    {
        closeAll();
        deleteWorld.SetActive(true);
        destroyButtons(ScrollContentDelete);
        createButtons(deleteSavedGamesPrefab, ScrollContentDelete);
    }
    private void createButtons(GameObject buttonPrefab, GameObject scroller)
    {
        //Create buttons
        foreach (Game g in SaveLoad.savedGames)
        {
            GameObject prefab = Instantiate(buttonPrefab, scroller.transform);
            prefab.GetComponent<MenuButtonController>().MM = this;
            prefab.GetComponent<MenuButtonController>().myGame = g;
            prefab.GetComponent<MenuButtonController>().setText();
        }
    }
    private void destroyButtons(GameObject scroller)
    {
        for(int i = 0; i < scroller.transform.childCount; i++)
        {
            Destroy(scroller.transform.GetChild(i).gameObject);
        }
    }

    public void OpenOptions()
    {
        //print("MainMenu: OpenOptions(): Options not implemented");
        options.SetActive(true);

        camLerp = true;
        isOptions = false;
        optionCon.gameObject.SetActive(true);
        optionCon.loadSettingsUI();
    }

    private void Update()
    {
        if (camLerp)
        {
            if (!isOptions)
            {
                float x = cam.transform.position.x;
                float y = cam.transform.position.y;
                float z = cam.transform.position.z;
                Vector3 lerpPos = new Vector3(Mathf.Lerp(x, optionsPos.x, lerpFactor), Mathf.Lerp(y, optionsPos.y, lerpFactor), z);
                cam.transform.position = lerpPos;

                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, optionsCamSize, lerpFactor);
                lerpFactor += 0.5f * Time.deltaTime;

                if (cam.transform.position == optionsPos)
                {
                    camLerp = false;
                    isOptions = true;
                    lerpFactor = 0;
                }
            }
            else
            {
                float x = cam.transform.position.x;
                float y = cam.transform.position.y;
                float z = cam.transform.position.z;
                Vector3 lerpPos = new Vector3(Mathf.Lerp(x, mainPos.x, lerpFactor), Mathf.Lerp(y, mainPos.y, lerpFactor), z);
                cam.transform.position = lerpPos;

                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, mainCamSize, lerpFactor);
                lerpFactor += 0.3f * Time.deltaTime;

                if (cam.transform.position == mainPos)
                {
                    camLerp = false;
                    isOptions = false;
                    lerpFactor = 0;
                }
            }
        }
    }

    public void saveExitOptions()
    {
        optionCon.saveExitMenu();
        camLerp = true;
        isOptions = true;
    }

    public void website()
    {
        print("MainMenu: website()");
        Application.OpenURL("http://sindrex.wordpress.com/game-boolean/");
    }

    public void Quit()
    {
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
}
