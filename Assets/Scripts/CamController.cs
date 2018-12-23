using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CamController : MonoBehaviour
{
    public float speed = 20.0f;
    public int maxZoom = 30;
    private readonly int minZoom = 3;

    //SIGN writing
    //public bool moveLock = false;

    void Update()
    {
        if (UtilBools.camMoveLock)
        {
            return;
        }

        //WASD moving
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * speed, Input.GetAxis("Vertical") * Time.deltaTime * speed, 0.0f);


        //Zooming
        if (InputController.getInput(InputPurpose.ZOOM_IN) && GetComponent<Camera>().orthographicSize > minZoom) // forward
        {
            GetComponent<Camera>().orthographicSize--;
            speed--;
        }
        else if (InputController.getInput(InputPurpose.ZOOM_OUT) && GetComponent<Camera>().orthographicSize < maxZoom) // backwards
        {
            GetComponent<Camera>().orthographicSize++;
            speed++;
        }
    }
}