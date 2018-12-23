using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DupeController : MonoBehaviour {

    public GameController GC;

    private void OnMouseDown()
    {
        GC.duplicateSelected();
    }
}
