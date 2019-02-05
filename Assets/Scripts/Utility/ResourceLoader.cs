using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public static class ResourceLoader {

    public static readonly string path = Application.streamingAssetsPath;

    public static List<T> loadJsonFolder<T>(string folderpath)
    {
        List<T> myList = new List<T>();

        var info = new DirectoryInfo(path + folderpath);
        var fileInfo = info.GetFiles();
        foreach(FileInfo file in fileInfo)
        {
            if (!file.Name.Contains(".meta"))
            {
                using (StreamReader r = new StreamReader(file.DirectoryName + "/" + file.Name))
                {

                    string json = r.ReadToEnd();
                    T item = JsonUtility.FromJson<T>(json);
                    myList.Add(item);
                }
            }
        }
        return myList;
    }

    public static T loadJson<T>(string filename)
    {
        using (StreamReader r = new StreamReader(path + filename))
        {
            string json = r.ReadToEnd();
            T item = JsonUtility.FromJson<T>(json);
            return item;
        }
    }
}

/*
 * 		"IO":[
			{
				"input": false, 
				"tileId": 15, 
				"spotIndex": 1, 
				"dir": 0, 
				"signal": [1, 1, 0, 0]
			}, 
			{
				"input": true, 
				"tileId": 1, 
				"spotIndex": 1, 
				"dir": 0, 
				"signal": [1, 1, 0, 0]
			}
		]
 * */
