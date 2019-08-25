using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausedController : MonoBehaviour {

    //@DEPRECATED

    public GameObject pausedContainer;
    public GameController GC;
    public GameObject saved;
    public GameObject options;
    public GameObject worldOptions;
    public GameObject exitDialogue;

    public Button worldOptionsButton;
    public Button saveButton;

    private void Start()
    {
        cancel();
    }

    public void save()
    {
        GC.updateData();
        Game.current.tileIDs = GC.tileIDs;
        Game.current.tileDIRs = GC.tileDIRs;
        Game.current.tilePower = GC.tilePower;
        Game.current.tileSetting = GC.tileSetting;
        Game.current.signTexts = GC.signTexts;
        SaveLoad.save();
        StartCoroutine("savedOnOff");
    }
    public void openOptions()
    {
        options.SetActive(true);
        options.GetComponent<OptionController>().loadSettingsUI();
        cancel();

        UtilBools.pausedBools(Game.current, true);
        UtilBools.paused = false;
        UtilBools.options = true;
    }
    public void openWorldOptions()
    {
        if (!UtilBools.isPuzzle)
        {
            worldOptions.SetActive(true);
            worldOptions.GetComponent<WorldOptionsController>().loadSettingsUI();
            cancel();

            UtilBools.pausedBools(Game.current, true);
            UtilBools.paused = false;
            UtilBools.worldOptions = true;
        }
        else
        {
            print("is puzzle");
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
    public void cancel()
    {
        UtilBools.pausedBools(Game.current, false);
        UtilBools.paused = false;
        pausedContainer.SetActive(false);
    }

    IEnumerator savedOnOff()
    {
        saved.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        saved.SetActive(false);
    }

    private void Update()
    {
        if(exitDialogue.activeSelf && InputController.getInput(InputPurpose.UNIVERSAL_BACK)){
            exitDialogue.SetActive(false);
        }
    }
}
