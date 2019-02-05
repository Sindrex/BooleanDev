using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour {

    public MainMenu MM;
    public Game myGame;

    public void setText()
    {
        transform.GetChild(0).GetComponent<Text>().text = myGame.gameName;
    }

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
    }
}
