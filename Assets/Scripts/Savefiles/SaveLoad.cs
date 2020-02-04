using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public static class SaveLoad{

    public static string postfix = ".save";
    public static string puzzlePrefix = "puzzle_";
    public static string puzzleSlotDefaultInfix = "_0";

    public static string savePath = Application.persistentDataPath + "/saves/";
    public static string puzzleSavePath = Application.persistentDataPath + "/puzzle_saves/";
    
    public static bool save()
    {
        Debug.Log("Saving...!");
        return createFile(Game.current, savePath);
    }
    public static bool savePuzzle(EOTP_PuzzleCreator puzzle) //default
    {
        Debug.Log("Saving puzzle...!");
        Game.current.gameName = puzzlePrefix + puzzle.id + puzzleSlotDefaultInfix;
        return createFile(Game.current, puzzleSavePath);
    }
    public static bool savePuzzle(EOTP_PuzzleCreator puzzle, int slot) //specific
    {
        Debug.Log("Saving puzzle...! Even picked a slot!");
        Game.current.gameName = puzzlePrefix + puzzle.id + "_" + slot;
        return createFile(Game.current, puzzleSavePath);
    }

    public static List<Game> loadSaves()
    {
        return loadAll(savePath);
    }
    public static List<Game> loadPuzzleSaves()
    {
        return loadAll(puzzleSavePath);
    }

    public static Game loadSave(string saveName)
    {
        return loadOne(saveName, savePath);
    }
    public static Game loadPuzzleSave(int puzzleId) //default
    {
        return loadOne(puzzlePrefix + puzzleId + puzzleSlotDefaultInfix, puzzleSavePath);
    }
    public static Game loadPuzzleSave(int puzzleId, int slot) //specific
    {
        return loadOne(puzzlePrefix + puzzleId + "_" + slot, puzzleSavePath);
    }

    public static bool removeSave(Game save)
    {
        return removeFile(save, savePath);
    }
    public static bool removePuzzleSave(Game save)
    {
        return removeFile(save, puzzleSavePath);
    }

    public static bool puzzleSaveExists(int puzzleId, int slot)
    {
        return File.Exists(puzzleSavePath + puzzlePrefix + puzzleId + "_" + slot + postfix);
    }
    public static bool saveExists(string saveName)
    {
        return File.Exists(savePath + saveName + postfix);
    }

    //private methods
    private static bool createFile(Game save, string path) //Save 1 savefile
    {
        Debug.Log("Creating new file: " + save.gameName);
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(path + save.gameName + postfix);
            bf.Serialize(file, save);
            file.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
            return false;
        }
    }

    private static bool removeFile(Game save, string path)
    {
        //Debug.Log("Trying to remove: " + save.gameName);
        try
        {
            Debug.Log("Removing save: " + save.gameName);
            if (File.Exists(path + save.gameName + postfix))
            {
                //Debug.Log("File exists, deleting...");
                File.Delete(path + save.gameName + postfix);
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log(e.StackTrace);
            return false;
        }
    }

    private static List<Game> loadAll(string path)
    {
        List<Game> savedGames = new List<Game>();
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            Directory.CreateDirectory(path);
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*" + postfix);
            foreach (FileInfo fi in files)
            {
                FileStream file = fi.Open(FileMode.Open);
                savedGames.Add((Game)bf.Deserialize(file));
                file.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log(e.StackTrace);
        }
        return savedGames;

        /*
        BinaryFormatter bf = new BinaryFormatter();
        Directory.CreateDirectory(savePath);
        DirectoryInfo di = new DirectoryInfo(savePath);
        FileInfo[] files = di.GetFiles("*.save");
        foreach (FileInfo fi in files)
        {
            FileStream file = fi.Open(FileMode.Open);
            savedGames.Add((Game)bf.Deserialize(file));
        }
        ---
        if (File.Exists(Application.persistentDataPath + "/savedGames.bool"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            SaveLoad.savedGames = (List<Game>) bf.Deserialize(file);
            file.Close();
        }*/
    }

    private static Game loadOne(string saveName, string path)
    {
        Game myGame = null;
        try
        {
            if (File.Exists(path + saveName + postfix))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path + saveName + postfix, FileMode.Open);
                myGame = (Game)bf.Deserialize(file);
                file.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log(e.StackTrace);
        }
        return myGame;
    }
}
