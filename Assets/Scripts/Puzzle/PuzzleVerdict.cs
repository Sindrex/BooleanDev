using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PuzzleVerdict : MonoBehaviour {

    public GameObject verdictObject;
    public Text desc;

    public enum verdict { WIN, LOSS }

	// Use this for initialization
	void Start () {
        verdictObject.SetActive(false);
    }

    public void openVerdict(verdict myVerdict, string winDesc)
    {
        verdictObject.SetActive(true);

        switch (myVerdict)
        {
            case verdict.WIN:
                desc.color = Color.green;
                desc.text = "COMPLETE! \n \n" + winDesc;
                break;
            case verdict.LOSS:
                desc.color = Color.red;
                desc.text = "SOMETHING'S NOT QUITE RIGHT...! \n \n" + winDesc;
                break;
        }
    }

    public void cancel()
    {
        verdictObject.SetActive(false);
    }
}
