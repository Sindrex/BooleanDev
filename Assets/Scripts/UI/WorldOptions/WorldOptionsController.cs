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

    public GameObject appliedText;

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
        appliedText.SetActive(false);
        GC.audioMixer.playButtonSFX();
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

    public void justSave()
    {
        miscCon.saveSettings();
        savedFeedback();
    }

    //unused
    public void saveExit()
    {
        openAll();

        //Saving
        miscCon.saveSettings();
        savedFeedback();
        closeAll();

        UtilBools.worldOptions = false;
        UtilBools.pausedBools(Game.current, false);

        gameObject.SetActive(false);
    }

    public void justExit()
    {
        openAll();
        closeAll();

        UtilBools.worldOptions = false;
        UtilBools.pausedBools(Game.current, false);

        gameObject.SetActive(false);
    }

    private void savedFeedback()
    {
        GC.audioMixer.playButtonSFX();
        appliedText.SetActive(true);
        StartCoroutine(waitSavedFeedback());
    }

    IEnumerator waitSavedFeedback()
    {
        yield return new WaitForSeconds(0.5f);
        appliedText.SetActive(false);
    }
}
