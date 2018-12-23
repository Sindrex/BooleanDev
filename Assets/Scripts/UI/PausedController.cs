using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausedController : MonoBehaviour {

    public GameController GC;
    public GameObject saved;
    public GameObject options;
    public GameObject worldOptions;

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
        SaveLoad.Save();
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
        worldOptions.SetActive(true);
        worldOptions.GetComponent<WorldOptionsController>().loadSettingsUI();
        cancel();

        UtilBools.pausedBools(Game.current, true);
        UtilBools.paused = false;
        UtilBools.worldOptions = true;
    }
    public void exitToMenu()
    {
        SceneManager.LoadScene(1);
    }
    public void exitToDesk()
    {
        Application.Quit();
    }
    public void cancel()
    {
        UtilBools.pausedBools(Game.current, false);
        UtilBools.paused = false;
        this.gameObject.SetActive(false);
    }

    IEnumerator savedOnOff()
    {
        saved.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        saved.SetActive(false);
    }
}
