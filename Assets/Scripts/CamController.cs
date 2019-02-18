using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CamController : MonoBehaviour
{
    public float speed = 20.0f;
    public int maxZoom = 30;
    private readonly int minZoom = 3;

    //drag
    public float dragSpeed = 5f;
    public Vector3 prevPos;

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

        //Drag middle mouse button moving
        if (InputController.getInput(InputPurpose.CAM_MOVEMENT_DRAG)){
            Vector3 dragPanning = Camera.main.ScreenToWorldPoint(prevPos) - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position += dragPanning;
        }
        prevPos = Input.mousePosition;

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