using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzleController : MonoBehaviour {

    public enum logic { NULL, AND, OR, NAND, NOR, ONLY_B, ONLY_A, NOT_A_OR_B, A_OR_NOT_B}

    public GameController GC;
    public GameObject lockPrefab;

    public PuzzleCreator myPuzzle;

    public PuzzleObjective PO;
    public PuzzleVerdict PV;

    public void setupPuzzle(PuzzleCreator puzzle)
    {
        print("Setting up puzzle!");

        for (int i = 0; i < puzzle.getLockedTiles().Length; i++)//For each unit in length
        {
            GameObject prefab = GC.spawnSingle(puzzle.getLockedTiles()[i][1], puzzle.getLockedTiles()[i][0], puzzle.getLockedTiles()[i][2]);
            prefab.GetComponent<TileController>().locked = true;
            print("spawning i: " + i + "/" + prefab.name);
            switch (puzzle.getLockedTiles()[i][3])
            {
                case -1:
                    prefab.GetComponent<TileController>().myLabel = TileController.label.OUT;
                    break;
                case 0:
                    prefab.GetComponent<TileController>().myLabel = TileController.label.NULL;
                    break;
                case 1:
                    prefab.GetComponent<TileController>().myLabel = TileController.label.IN;
                    break;
                default:
                    prefab.GetComponent<TileController>().myLabel = TileController.label.NULL;
                    print("something went wrong");
                    break;
            }

            GameObject lockP = Instantiate(lockPrefab, prefab.transform);
            lockP.transform.position += new Vector3(-0.35f, 0.35f, 0);
        }

        //setup input/output

        myPuzzle = puzzle;
        PO.setup(puzzle);
    }

    public void pathfinder()
    {
        List<List<GameObject>> connectedList = new List<List<GameObject>>();
        List<GameObject> input = new List<GameObject>();
        List<GameObject> output = new List<GameObject>();

        //Temporary list of tiles (but just the tiles, not airtiles)
        List<GameObject> tiles = new List<GameObject>();
        foreach(GameObject go in GC.tiles)
        {
            if (go != null && go.GetComponent<InputBasedWire>() != null)
            {
                tiles.Add(go);
                //print(go.name);
            }
        }

        foreach (GameObject go in tiles)
        {
            TileController myTC = go.GetComponent<TileController>();
            if (myTC.myLabel == TileController.label.IN && !input.Contains(go))
            {
                input.Add(go);
            }
            else if (myTC.myLabel == TileController.label.OUT && !output.Contains(go))
            {
                output.Add(go);
            }
        }

        //Add to connectedList
        foreach (GameObject go in tiles)
        {
            foreach (GameObject other in tiles)
            {
                foreach (GameObject neigh in other.GetComponent<InputBasedWire>().neighbours)
                {
                    if(neigh != null)
                    {
                        if (neigh.Equals(go))
                        {
                            bool ok = true;
                            foreach (List<GameObject> list in connectedList)
                            {
                                if ((list[0].Equals(go) && list[1].Equals(other)) || (list[1].Equals(go) && list[0].Equals(other)))
                                {
                                    ok = false;
                                }
                            }
                            if (ok)
                            {
                                List<GameObject> temp = new List<GameObject>();
                                temp.Add(go);
                                temp.Add(other);
                                connectedList.Add(temp);
                            }
                        }
                    }
                }
            }
        }

        //print em'
        /*
        foreach (List<GameObject> a in connectedList)
        {
            if (a.Count > 1)
            {
                print(a[0].name + " - " + a[1].name);
            }
        }
        foreach(GameObject a in input)
        {
            print("Input: " + a.name);
        }
        foreach (GameObject a in output)
        {
            print("Output: " + a.name);
        }
        */

        List<List<GameObject>> discoveredList = new List<List<GameObject>>();
        List<bool> negates = new List<bool>();

        for (int i = 0; i < input.Count; i++)
        {
            //print("Input: " + input[i].name);
            List<GameObject> discovered = new List<GameObject>();
            int negated = 0;

            deepFirstSearch(connectedList, discovered, input[i], false);
        
            foreach (GameObject a in discovered)
            {
                if(a.GetComponent<TileController>().ID == 6)
                {
                    negated++;
                }
            }

            discoveredList.Add(discovered);

            if(negated % 2 == 0)
            {
                negates.Add(false);
            }
            else
            {
                negates.Add(true);
            }
        }

        bool convergance = false; //are the inputs connected?
        GameObject convergObj = null;

        foreach (List<GameObject> discoveryOuter in discoveredList)
        {
            foreach (List<GameObject> discoveryInner in discoveredList)
            {
                if (!discoveryOuter.Equals(discoveryInner))
                {
                    foreach (GameObject goOuter in discoveryOuter)
                    {
                        foreach (GameObject goInner in discoveryInner)
                        {
                            if (goOuter.Equals(goInner))
                            {
                                convergObj = goInner;
                                convergance = true;
                            }
                        }
                    }
                }
            }
        }
        print("Convergance: " + convergance);

        while(convergObj != null)
        {
            List<GameObject> afterConverg = new List<GameObject>();
            int negated = 0;

            deepFirstSearch(connectedList, afterConverg, convergObj, true);

            List<GameObject> removeList = new List<GameObject>();
            foreach (GameObject go in afterConverg)
            {
                foreach(List<GameObject> gList in discoveredList)
                {
                    if (gList.Contains(go))
                    {
                        removeList.Add(go);
                    }
                }
            }
            foreach(GameObject go in removeList)
            {
                afterConverg.Remove(go);
            }

            discoveredList.Add(afterConverg);

            foreach (GameObject a in afterConverg)
            {
                if (a.GetComponent<TileController>().ID == 6)
                {
                    negated++;
                }
            }
            if (negated % 2 == 0)
            {
                negates.Add(false);
            }
            else
            {
                negates.Add(true);
            }

            convergObj = null;
            foreach(GameObject go in afterConverg)
            {
                if(go.GetComponent<TileController>().ID == 3 || go.GetComponent<TileController>().ID == 4)
                {
                    convergObj = go;
                }
            }
        }

        bool connectToOut = false;
        foreach (List<GameObject> gList in discoveredList)
        {
            foreach (GameObject go in gList)
            {
                if (go.GetComponent<TileController>().myLabel == TileController.label.OUT)
                {
                    connectToOut = true;
                }
            }
        }

        int index = 0;
        foreach(List<GameObject> gList in discoveredList)
        {
            print("**New discovered list**");
            print("Negated: " + negates[index]);
            index++;
            foreach(GameObject go in gList)
            {
                print(go.name);
            }
        }

        print("Connected to Output: " + connectToOut);

        logic verdict = logic.NULL;
        if (negates.Count >= 3 && convergance && connectToOut)
        {
            if (negates[0] && negates[1] && negates[2])
            {
                print("Analysis: AND");
                verdict = logic.AND;
            }
            else if (!negates[0] && negates[1] && negates[2])
            {
                print("Analysis: ONLY B");
                verdict = logic.ONLY_B;
            }
            else if (!negates[0] && !negates[1] && negates[2])
            {
                print("Analysis: NOR");
                verdict = logic.NOR;
            }
            else if (!negates[0] && !negates[1] && !negates[2])
            {
                print("Analysis: OR");
                verdict = logic.OR;
            }
            else if (negates[0] && !negates[1] && !negates[2])
            {
                print("Analysis: NOT A OR B");
                verdict = logic.NOT_A_OR_B;
            }
            else if (negates[0] && negates[1] && !negates[2])
            {
                print("Analysis: NAND");
                verdict = logic.NAND;
            }
            else if (negates[0] && !negates[1] && negates[2])
            {
                print("Analysis: ONLY A");
                verdict = logic.ONLY_A;
            }
            else if (!negates[0] && negates[1] && !negates[2])
            {
                print("Analysis: A OR NOT B");
                verdict = logic.A_OR_NOT_B;
            }
        }

        if (GC.paused.activeSelf)
        {
            GC.paused.SetActive(false);
            UtilBools.actionBarLock = false;
        }

        if (verdict == myPuzzle.getWinCondition())
        {
            PV.openVerdict(PuzzleVerdict.verdict.WIN, verdict.ToString());
        }
        else
        {
            PV.openVerdict(PuzzleVerdict.verdict.LOSS, verdict.ToString());
        }

    }

    private void deepFirstSearchNeighbour(List<List<GameObject>> connectedList, List<GameObject> discovered, GameObject vertex)
    {
        discovered.Add(vertex);

        foreach(GameObject edge in vertex.GetComponent<InputBasedWire>().neighbours)
        {
            if (!discovered.Contains(edge) && edge != null)
            {
                List<GameObject> temp = new List<GameObject>();
                temp.Add(vertex);
                temp.Add(edge);
                connectedList.Add(temp);
                deepFirstSearchNeighbour(connectedList, discovered, edge);
            }
        }
    }

    //Does: All gameObjects reachable from vertex is put in discovered list.
    private void deepFirstSearch(List<List<GameObject>> connectedList, List<GameObject> discovered, GameObject vertex, bool isConverg)
    {
        //print("Checking: " + vertex.name);
        discovered.Add(vertex);
        int id = vertex.GetComponent<TileController>().ID;
        if (id == 3 || id == 4)
        {
            if (!isConverg)
            {
                return;
            }
        }
        foreach(GameObject edge in getConnected(connectedList, vertex)) //connected tiles to this vertex
        {
            if (!discovered.Contains(edge))
            {
                if(id == 7)
                {
                    int index = getIndexOf(edge, vertex.GetComponent<InputBasedWire>().neighbours);
                    //print("index: " + index);
                    switch (getIndexOf(discovered[discovered.Count - 2], vertex.GetComponent<InputBasedWire>().neighbours))
                    {
                        case -1:
                            print("You fuked up");
                            break;
                        case 0:
                        case 2:
                            if (index == 0 || index == 2)
                            {
                                deepFirstSearch(connectedList, discovered, edge, false);
                            }
                            break;
                        case 1:
                        case 3:
                            if (index == 1 || index == 3)
                            {
                                deepFirstSearch(connectedList, discovered, edge, false);
                            }
                            break;
                    }
                }
                else
                {
                    deepFirstSearch(connectedList, discovered, edge, false);
                }
            }
        }
    }

    private List<GameObject> getConnected(List<List<GameObject>> connectedList, GameObject vertex)
    {
        //print("Getting connected for: " + vertex.name);
        List<GameObject> res = new List<GameObject>();
        foreach(List<GameObject> list in connectedList)
        {
            if(list[0] == vertex)
            {
                res.Add(list[1]);
            }
            else if (list[1] == vertex)
            {
                res.Add(list[0]);
            }
        }
        //print("Count: " + res.Count);
        return res;
    }

    private int getIndexOf(GameObject neigh, GameObject[] list)
    {
        for(int i = 0; i < list.Length; i++)
        {
            if(list[i] != null && neigh != null)
            {
                //print("YO: " + list[i].name + "/" + neigh.name);
                if (list[i].Equals(neigh))
                {
                    return i;
                }
            }
        }
        return -1;
    }

    private int negDir(int DIR)
    {
        int neg = DIR + 2;
        if (neg == 4)
        {
            neg = 0;
        }
        if (neg > 4)
        {
            neg = 1;
        }
        return neg;
    }
}
