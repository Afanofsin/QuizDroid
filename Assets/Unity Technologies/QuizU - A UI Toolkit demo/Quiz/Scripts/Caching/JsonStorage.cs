using System.IO;
using System.IO.Enumeration;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Purchasing.MiniJSON;

public class JsonStorage
{
    public static string GetFilePath(string filename)
    {
        return Path.Combine(Application.persistentDataPath, filename + ".json");
    }

    public static void Save<T>(string filename, T data)
    {
        try
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            string path = Path.Combine(Application.persistentDataPath, filename + ".json");
            File.WriteAllText(path, json);
            Debug.Log($"Saved {filename} to {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save {filename}: {e.Message}");
        }
    }

    public static T Load<T>(string filename) where T : class
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, filename + ".json");

            if (!File.Exists(path))
            {
                Debug.Log($"File {filename} not found");
                return null;
            }

            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load {filename}: {e.Message}");
            return null;
        }
    }
    
}
