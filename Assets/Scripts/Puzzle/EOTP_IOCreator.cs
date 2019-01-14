using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EOTP_IOCreator {

    [SerializeField]
    private bool input;
    [SerializeField]
    private int spotIndex;
    [SerializeField]
    private int dir;
    [SerializeField]
    private int tileId;

    //Output for outputs or expected input for inputs (0, 1, 0, 1 etc)
    [SerializeField]
    private int[] signal;

    public EOTP_IOCreator(bool input, int spotIndex, int tileId, int dir, int[] signal)
    {
        this.input = input;
        this.spotIndex = spotIndex;
        this.dir = dir;
        this.tileId = tileId;
        this.signal = signal;
    }

    public bool isInput()
    {
        return input;
    }
    public int getTileNr()
    {
        return spotIndex;
    }
    public int getDir()
    {
        return dir;
    }
    public int getTileId()
    {
        return tileId;
    }
    public int[] getSignal()
    {
        return signal;
    }
}
