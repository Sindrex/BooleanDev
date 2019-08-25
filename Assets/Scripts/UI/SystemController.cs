using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SystemController : MonoBehaviour {

    public GameController GC;

    public GameObject saved;
    public GameObject worldOptions;
    public GameObject exitDialogue;

    public Button worldOptionsButton;
    public Button saveButton;

    public static readonly string puzzleSaveSlotKey = "PuzzleSaveSlot";
    public Button[] puzzleSaveSlots;
    public Sprite[] puzzleSaveSlotHighlighted;

    private void Start()
    {
        //Set right highlight for puzzlesaveslots
        int slot = PlayerPrefs.GetInt(puzzleSaveSlotKey);
        puzzleSaveSlots[slot].GetComponentInParent<Image>().sprite = puzzleSaveSlotHighlighted[slot];
    }

    //every frame
    private void Update()
    {
        if (!exitDialogue.activeSelf && InputController.getInput(InputPurpose.UNIVERSAL_BACK))
        {
            exitDialogue.SetActive(true);
        }
        else if (exitDialogue.activeSelf && InputController.getInput(InputPurpose.UNIVERSAL_BACK))
        {
            //exitToMenu();
            exitDialogue.SetActive(false);
        }
    }

    public void isPuzzle()
    {
        worldOptionsButton.interactable = false;
    }

    public void save()
    {
        GC.updateData();
        Game.current.tileIDs = GC.tileIDs;
        Game.current.tileDIRs = GC.tileDIRs;
        Game.current.tilePower = GC.tilePower;
        Game.current.tileSetting = GC.tileSetting;
        Game.current.signTexts = GC.signTexts;

        if (!GC.isPuzzle)
        {
            if (SaveLoad.save())
            {
                saveGraphic();
            }
            else
            {
                print("Couldnt save!");
            }
        }
        else
        {
            if (SaveLoad.savePuzzle(GC.ePC.myPuzzle, PlayerPrefs.GetInt(puzzleSaveSlotKey)))
            {
                saveGraphic();
            }
            else
            {
                print("Couldnt save puzzle!");
            }
        }
    }

    public void saveGraphic()
    {
        StartCoroutine("savedOnOff");
    }
    IEnumerator savedOnOff()
    {
        saved.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        saved.SetActive(false);
    }

    public void openWorldOptions()
    {
        if (!UtilBools.isPuzzle)
        {
            worldOptions.SetActive(true);
            worldOptions.GetComponent<WorldOptionsController>().loadSettingsUI();

            UtilBools.pausedBools(Game.current, true);
            UtilBools.paused = false;
            UtilBools.worldOptions = true;
        }
        else
        {
            print("is puzzle duh");
        }
    }

    public void toggleExitDialogue()
    {
        exitDialogue.SetActive(!exitDialogue.activeSelf);
    }

    public void exitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void exitToDesk()
    {
        Application.Quit();
    }

    public void loadPuzzleSaveSlot(int slot)
    {
        print("Loading slot: " + slot);
    
        int puzzleId = GC.ePC.myPuzzle.id;
        PlayerPrefs.SetInt(puzzleSaveSlotKey, slot);
        if (SaveLoad.puzzleSaveExists(puzzleId, slot)) //Check if save slot is created
        {
            print("Puzzle save slot exists, loading old one");
            Game.current = SaveLoad.loadPuzzleSave(puzzleId, slot);
        }
        else //if not, create it
        {
            print("No puzzle save slot, creating game from same puzzle");
            Game.current = new Game(GC.ePC.myPuzzle);
        }
        SceneManager.LoadScene("Main");
    }
}
