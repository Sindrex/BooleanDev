using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloorTileController : MonoBehaviour {

    public int spotIndex = 0;
    public bool busy = false;

    /*
    private void OnCollisionStay2D(Collision2D collision)
    {
        print("Colliding with floor: " + spotIndex + ", by: " + collision.gameObject.name);
    }*/
}
