using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleArray : MonoBehaviour {

    public PuzzleCreator[] puzzles;
    public EOTP_PuzzleCreator[] EOTP_puzzles;

    // Use this for initialization
    void Start () {
        puzzles = new PuzzleCreator[4];
        EOTP_puzzles = new EOTP_PuzzleCreator[4];
        //puzzleInit();
        EOTP_init();
	}
	
    //@Deprecated
    private void puzzleInit()
    {
        int[][] puzzle1 = new int[][] { 
            new int[]{ 1, 15, 3, 0 },
            new int[]{ 3, 15, 3, 0 },
            new int[]{ 6, 1, 0, 1 },
            new int[]{ 8, 1, 0, 1 },
            new int[]{ 22, 1, 0, -1 }
        };

        int[][] puzzle2 = new int[][] {
            new int[]{ 1, 15, 3, 0 },
            new int[]{ 3, 15, 3, 0 },
            new int[]{ 6, 1, 0, 1 },
            new int[]{ 8, 1, 0, 1 },
            new int[]{ 22, 1, 0, -1 }
        };

        int[][] puzzle3 = new int[][] {
            new int[]{ 1, 15, 3, 0 },
            new int[]{ 3, 15, 3, 0 },
            new int[]{ 6, 1, 0, 1 },
            new int[]{ 8, 1, 0, 1 },
            new int[]{ 27, 1, 0, -1 }
        };

        //name, id, length, heigth, lockedtiles(tileNr, tileID, dir, input/output), winCondition(tileNr)
        puzzles[0] = new PuzzleCreator("TUTORIAL", 1, 5, 5, puzzle1, PuzzleController.logic.OR, "Tutorial.");

        puzzles[0] = new PuzzleCreator("Puzzle 1: OR", 1, 5, 5, puzzle1, PuzzleController.logic.OR, "Create a logic gate " +
                                        "that lets either or both inputs power the output.");

        puzzles[1] = new PuzzleCreator("Puzzle 2: NOR", 2, 5, 5, puzzle2, PuzzleController.logic.NOR, "Create a logic gate " +
                                        "so that the output is on whenever no inputs are on.");

        puzzles[2] = new PuzzleCreator("Puzzle 3: AND", 3, 5, 6, puzzle3, PuzzleController.logic.AND, "Create a logic gate " +
                                         "so that the output is on only when both inputs are on.");
    }

    private void EOTP_init()
    {
        EOTP_IOCreator[] io_0 = new EOTP_IOCreator[]
        {
        };
        EOTP_puzzles[0] = new EOTP_PuzzleCreator("TUTORIAL", 0, 5, 5, io_0, "Tutorial desc.");

        EOTP_IOCreator[] io_1 = new EOTP_IOCreator[]
        {
            //              input/spotIndex/tileId/dir/signal-array
            new EOTP_IOCreator(false, 1, 15, 0, new int[]{ 1, 1, 0, 0 }),
            new EOTP_IOCreator(false, 3, 15, 0, new int[]{ 1, 0, 1, 0 }),
            new EOTP_IOCreator(true, 22, 1, -1, new int[]{ 1, 1, 1, 0 })
        };
        //                                      name, id, length, height, io, desc
        EOTP_puzzles[1] = new EOTP_PuzzleCreator("Puzzle 1", 1, 5, 5, io_1, "My 1st Puzzle: OR");

        EOTP_IOCreator[] io_2 = new EOTP_IOCreator[]
        {
            new EOTP_IOCreator(false, 1, 15, 0, new int[]{ 1, 1, 0, 0 }),
            new EOTP_IOCreator(false, 3, 15, 0, new int[]{ 1, 0, 1, 0 }),
            new EOTP_IOCreator(true, 22, 1, -1, new int[]{ 0, 0, 0, 1 })
        };
        EOTP_puzzles[2] = new EOTP_PuzzleCreator("Puzzle 2", 2, 5, 5, io_2, "My 2nd Puzzle: NOR");

        EOTP_IOCreator[] io_3 = new EOTP_IOCreator[]
        {
            new EOTP_IOCreator(false, 1, 15, 0, new int[] { 1, 1, 0, 0 }),
            new EOTP_IOCreator(false, 3, 15, 0, new int[] { 1, 0, 1, 0 }),
            new EOTP_IOCreator(true, 22, 1, -1, new int[] { 1, 0, 0, 0 })
        };
        EOTP_puzzles[3] = new EOTP_PuzzleCreator("Puzzle 3", 3, 5, 5, io_3, "My 3rd Puzzle: AND");
    }
}
