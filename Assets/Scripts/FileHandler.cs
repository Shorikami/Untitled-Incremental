using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileHandler
{
    private string dirPath = "";
    private string fileName = "";

    public FileHandler(string path, string name)
    {
        dirPath = path;
        fileName = name;
    }

    public GameData Load()
    {
        string full = Path.Combine(dirPath, fileName);
        GameData loaded = null;

        if (File.Exists(full))
        {
            try
            {
                // load serialized data from file
                string toLoad = "";
                using (FileStream stream = new FileStream(full, FileMode.Open))
                {
                    using (StreamReader strR = new StreamReader(stream))
                    {
                        toLoad = strR.ReadToEnd();
                    }
                }

                // deserialize data from Json back to objects
                loaded = JsonUtility.FromJson<GameData>(toLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occurred while loading: " + full + "\n" + e);
            }
        }
        return loaded;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dirPath, fileName);

        try
        {
            // create directory if directory doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // serialize game data into Json
            string dataToStore = JsonUtility.ToJson(data, true);

            // write data to file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter wr = new StreamWriter(stream))
                {
                    wr.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occurred while saving: " + fullPath + "\n" + e);
        }
    }
}
