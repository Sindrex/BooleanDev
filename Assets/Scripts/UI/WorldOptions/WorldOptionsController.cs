using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldOptionsController : MonoBehaviour {

    public GameController GC;

    public ExpandController expandCon;
    public MiscController miscCon;
    public StatsController statsCon;

    public GameObject expand;
    public GameObject misc;
    public GameObject stats;

    private void Update()
    {
        if (InputController.getInput(InputPurpose.UNIVERSAL_BACK))
        {
            justExit();
        }
    }

    public void loadSettingsUI()
    {
        //load saved settings to UI etc
        miscCon.loadSettingsUI();
        closeAll();
        openExpand();
    }

    public void closeAll()
    {
        expand.SetActive(false);
        misc.SetActive(false);
        stats.SetActive(false);
    }
    public void openAll()
    {
        expand.SetActive(true);
        misc.SetActive(true);
        stats.SetActive(true);
    }

    public void openExpand()
    {
        closeAll();
        expand.SetActive(true);
    }
    public void openMisc()
    {
        closeAll();
        misc.SetActive(true);
    }
    public void openStats()
    {
        closeAll();
        stats.SetActive(true);
        stats.GetComponent<StatsController>().createStats();
    }

    public void saveExit()
    {
        openAll();

        //Saving goes here
        miscCon.saveSettings();
        closeAll();

        UtilBools.worldOptions = false;
        GC.togglePaused();

        gameObject.SetActive(false);
    }

    public void justExit()
    {
        closeAll();
        gameObject.SetActive(false);

        UtilBools.worldOptions = false;
        GC.togglePaused();
    }
}
