using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public static class SaveLoad{

    public static List<Game> savedGames = new List<Game>();
    public static string savePath = Application.persistentDataPath + "/savedGames.bool";
    //C:/Users/Sindre/AppData/Locallow/Sindrex/Boolean

    public static bool Save()
    {
        //Gonna overwrite if save file already exists
        Game save = alreadySaved(Game.current.gameName);
        //Debug.Log("Save: " + save.gameName);

        if (save != null)
        {
            Debug.Log("Overwriting...");
            int index = savedGames.IndexOf(save);
            savedGames.RemoveAt(index);
            savedGames.Insert(index, Game.current);
        }
        else
        {
            Debug.Log("Adding new save to savedGames!");
            savedGames.Add(Game.current);
        }

        return createFile();
    }

    private static bool createFile()
    {
        Debug.Log("Creating new file...");
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
            //Debug.Log("Path: "+ Application.persistentDataPath);
            FileStream file = File.Create(savePath);
            bf.Serialize(file, SaveLoad.savedGames);
            file.Close();
            return true;
        }
        catch(Exception e)
        {
            Debug.Log(e.StackTrace);
        }
        return false;
    }

    public static bool removeSave(string saveName)
    {
        Game save = alreadySaved(saveName);

        if (savedGames.Contains(save))
        {
            Debug.Log("Removing save...");
            savedGames.Remove(save);
            createFile();
            return true;
        }
        return false;
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.bool"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            SaveLoad.savedGames = (List<Game>) bf.Deserialize(file);
            file.Close();
        }
    }

    private static Game alreadySaved(string saveName)
    {
        Game save = null;
        foreach (Game g in savedGames)
        {
            //Debug.Log("Foreach: " + g.gameName);
            if (g.gameName.Equals(saveName))
            {
                save = g;
                break;
            }
        }
        return save;
    }
}
