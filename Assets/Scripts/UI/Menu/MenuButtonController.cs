using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour {

    public SandboxController SC;
    public Text myName;
    public Text myDateCreated;
    public Text myDatePlayed;
    public Text myVersion;
    public Game myGame;

    public Sprite regularSprite;
    public Sprite selectedSprite;

    public void setText()
    {
        myName.text = myGame.gameName;
        myDateCreated.text = myGame.dateCreated;
        myDatePlayed.text = myGame.dateLastPlayed;
        myVersion.text = myGame.version;
    }
    public void selectGame()
    {
        SC.selectGame(myGame);
        GetComponent<Image>().sprite = selectedSprite;
    }
    public void unSelect()
    {
        GetComponent<Image>().sprite = regularSprite;
    }
    /*
    public void loadSave()
    {
        Game.current = myGame;
        //Move on to game...
        SceneManager.LoadScene("Main");
    }

    public void removeSave()
    {
        if (SaveLoad.removeSave(myGame.gameName))
        {
            print("Delete OK");
        }
        Destroy(this.gameObject);
    }

    public void cancel()
    {
        MM.openSandbox();
    }*/
}
