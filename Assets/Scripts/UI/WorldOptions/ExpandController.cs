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

    public ExpandButton depExpand;
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
        depExpand.dirs = new int[4];

        int left = int.Parse(inputLeft.text);
        int right = int.Parse(inputRight.text);
        int up = int.Parse(inputUp.text);
        int down = int.Parse(inputDown.text);

        depExpand.dirs[0] = up;
        depExpand.dirs[1] = left;
        depExpand.dirs[2] = down;
        depExpand.dirs[3] = right;

        depExpand.startExpand();
        clearAll();
    }

    public void clearAll()
    {
        inputLeft.text = "0";
        inputRight.text = "0";
        inputUp.text = "0";
        inputDown.text = "0";
    }
}
