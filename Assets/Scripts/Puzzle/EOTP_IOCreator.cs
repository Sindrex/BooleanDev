using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EOTP_IOCreator {

    [SerializeField]
    public bool input;
    [SerializeField]
    public int spotIndex;
    [SerializeField]
    public int dir;
    [SerializeField]
    public int tileId;
    [SerializeField]
    public string sign;

    //Output for outputs or expected input for inputs (0, 1, 0, 1 etc)
    [SerializeField]
    public int[] signal;

    public EOTP_IOCreator(bool input, int spotIndex, int tileId, int dir, int[] signal)
    {
        this.input = input;
        this.spotIndex = spotIndex;
        this.dir = dir;
        this.tileId = tileId;
        this.signal = signal;
    }
}
