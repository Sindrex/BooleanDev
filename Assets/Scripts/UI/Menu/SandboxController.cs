using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SandboxController : MonoBehaviour {

    public MainMenu MM;
    public GameObject scrollContent;
    public GameObject buttonPrefab;

    private Game selectedGame;

    public Button playButton;
    public Button deleteButton;
    public List<GameObject> savedGames;

    public void open()
    {
        print("Opening sandbox menu");
        destroyButtons();
        savedGames = new List<GameObject>();
        createButtons();
        playButton.interactable = false;
        deleteButton.interactable = false;
    }
    public void createButtons()
    {
        //Create buttons
        List<Game> games = SaveLoad.loadSaves();
        games.Sort((x, y) => x.dateLastPlayed.CompareTo(y.dateLastPlayed) == 0 ? x.gameName.CompareTo(y.gameName) : x.dateLastPlayed.CompareTo(y.dateLastPlayed));
        games.Reverse();
        foreach (Game g in games)
        {
            GameObject prefab = Instantiate(buttonPrefab, scrollContent.transform);
            prefab.GetComponent<MenuButtonController>().SC = this;
            prefab.GetComponent<MenuButtonController>().myGame = g;
            prefab.GetComponent<MenuButtonController>().setText();
            savedGames.Add(prefab);
        }
    }
    public void destroyButtons()
    {
        for (int i = savedGames.Count - 1; i >= 0; i--)
        {
            Destroy(savedGames[i]);
        }
    }
    public void selectGame(Game myGame)
    {
        foreach(GameObject go in savedGames)
        {
            if(go.GetComponent<MenuButtonController>().myGame.gameName != myGame.gameName)
            {
                go.GetComponent<MenuButtonController>().unSelect();
            }
        }
        selectedGame = myGame;
        playButton.interactable = true;
        deleteButton.interactable = true;
    }
    public void play()
    {
        MM.audioCon.playButtonSFX();
        Game.current = selectedGame;
        //Move on to game...
        SceneManager.LoadScene("Main");
    }
    public void delete()
    {
        MM.audioCon.playButtonSFX();
        if (SaveLoad.removeSave(selectedGame))
        {
            print("Delete OK");
            open();
        }
        else
        {
            print("Delete not OK");
        }
    }
    public void close()
    {
        MM.openMainMenu();
    }

    public void openSaveFolder()
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
        {
            FileName = Application.persistentDataPath,
            UseShellExecute = true,
            Verb = "open"
        });
        MM.audioCon.playButtonSFX();
    }
}
