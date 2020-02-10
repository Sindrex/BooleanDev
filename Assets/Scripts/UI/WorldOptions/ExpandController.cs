using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpandController : MonoBehaviour {

    public InputField inputLeft;
    public InputField inputRight;
    public InputField inputUp;
    public InputField inputDown;

    public Button acceptButton;
    public Button clearButton;

    public GameController GC;
    public GameObject loading;

    private void Update()
    {
        loading.SetActive(UtilBools.expandLoading);
    }

    private bool checkInput()
    {
        int num;
        return int.TryParse(inputLeft.text, out num) 
            && int.TryParse(inputRight.text, out num) 
            && int.TryParse(inputUp.text, out num) 
            && int.TryParse(inputDown.text, out num);
    }

    public void test()
    {
        print(checkInput());
    }

    public void acceptExpand()
    {
        //print("Expanding!!");
        GC.audioMixer.playButtonSFX();
        int[] dirs = new int[4];

        int left = int.Parse(inputLeft.text);
        int right = int.Parse(inputRight.text);
        int up = int.Parse(inputUp.text);
        int down = int.Parse(inputDown.text);

        dirs[0] = up;
        dirs[1] = left;
        dirs[2] = down;
        dirs[3] = right;

        GC.doExpand = new int[4];
        for (int i = 0; i < dirs.Length; i++)
        {
            if (dirs[i] > 0)
            {
                //print(i + "/" + GC.doExpand.Length + "/" + dirs.Length);
                GC.doExpand[i] = dirs[i];
            }
        }
        clearAll();
    }

    public void clearAll()
    {
        GC.audioMixer.playButtonSFX();
        inputLeft.text = "0";
        inputRight.text = "0";
        inputUp.text = "0";
        inputDown.text = "0";
    }
}
