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

    //every frame
    private void Update()
    {
        if (exitDialogue.activeSelf && InputController.getInput(InputPurpose.UNIVERSAL_BACK))
        {
            exitDialogue.SetActive(false);
        }
    }

    public void close()
    {
        worldOptionsButton.interactable = false;
        saveButton.interactable = false;
    }

    public void save()
    {
        GC.updateData();
        Game.current.tileIDs = GC.tileIDs;
        Game.current.tileDIRs = GC.tileDIRs;
        Game.current.tilePower = GC.tilePower;
        Game.current.tileSetting = GC.tileSetting;
        Game.current.signTexts = GC.signTexts;
        if (SaveLoad.Save())
        {
            StartCoroutine("savedOnOff");
        }
        else
        {
            print("Couldnt save!");
        }
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
}
