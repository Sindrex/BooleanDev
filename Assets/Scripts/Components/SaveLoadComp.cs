using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class SaveLoadComp : MonoBehaviour {

    public static List<ComponentSave> savedComponents = new List<ComponentSave>();

    public static string savePath = Application.persistentDataPath + "/components/"; //remember to add savename

    public static string postfix = ".comp";

    public static void SaveComp(ComponentSave save)
    {
        //Debug.Log("Save: " + save.gameName);

        if (savedComponents.Contains(save))
        {
            Debug.Log("Overwriting...");
            int index = savedComponents.IndexOf(save);
            savedComponents.RemoveAt(index);
            savedComponents.Insert(index, save);
        }
        else
        {
            Debug.Log("Adding new comp to savedComponents!");
            savedComponents.Add(save);
        }
        createFile(save);
    }

    private static void createFile(ComponentSave save)
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        Debug.Log("Comp: Creating new file...");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath + save.name + postfix);
        bf.Serialize(file, save);
        file.Close();
    }

    public static bool removeSave(ComponentSave save)
    {
        if (savedComponents.Contains(save))
        {
            Debug.Log("Removing comp save...");
            savedComponents.Remove(save);
            if(File.Exists(savePath + save.name + postfix))
            {
                File.Delete(savePath + save.name + postfix);
                return true;
            }
            return true;
        }
        return false;
    }

    public static void LoadComp()
    {
        savedComponents = new List<ComponentSave>();
        if (Directory.Exists(savePath))
        {
            DirectoryInfo di = new DirectoryInfo(savePath);
            FileInfo[] files = di.GetFiles("*" + postfix);

            BinaryFormatter bf = new BinaryFormatter();

            foreach(FileInfo f in files)
            {
                FileStream file = f.Open(FileMode.Open);
                savedComponents.Add((ComponentSave) bf.Deserialize(file));
                file.Close();
            }
        }
    }

    public static bool saveExists(string saveName)
    {
        //Debug.Log("Checking for save: " + savePath + saveName + postfix);
        return File.Exists(savePath + saveName + postfix);
    }
}
