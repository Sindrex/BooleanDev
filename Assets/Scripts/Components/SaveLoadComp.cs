using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class SaveLoadComp : MonoBehaviour {
    public static List<ComponentSave> savedComponents = new List<ComponentSave>();
    public static string savePath = Application.persistentDataPath + "/components/"; //remember to add savename
    //C:/Users/Sindre/AppData/Locallow/Sindrex/Boolean

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

        string fileName = save.name;

        Debug.Log("Comp: Creating new file...");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath + fileName + ".comp");
        bf.Serialize(file, save);
        file.Close();
    }

    public static bool removeSave(ComponentSave save)
    {
        if (savedComponents.Contains(save))
        {
            Debug.Log("Removing save...");
            savedComponents.Remove(save);
            if(File.Exists(savePath + save.name + ".comp"))
            {
                File.Delete(savePath + save.name + ".comp");
                return true;
            }
        }
        return false;
    }

    public static void LoadComp()
    {
        savedComponents = new List<ComponentSave>();
        if (Directory.Exists(savePath))
        {
            DirectoryInfo di = new DirectoryInfo(savePath);
            FileInfo[] files = di.GetFiles("*.comp");

            BinaryFormatter bf = new BinaryFormatter();

            foreach(FileInfo f in files)
            {
                FileStream file = f.Open(FileMode.Open);
                savedComponents.Add((ComponentSave) bf.Deserialize(file));
                file.Close();
            }
        }
    }
}
