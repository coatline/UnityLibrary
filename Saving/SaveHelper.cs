using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveHelper
{
    public static void Save<T>(string fileName, T data, string directory = "")
    {
        if (Directory.Exists(GetFullPath(directory)) == false)
        {
            Debug.Log($"The directory: {GetFullPath(directory)} doesn't exist. Creating it.");
            Directory.CreateDirectory(GetFullPath(directory));
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Path.Combine(GetFullPath(directory), fileName), json);
    }

    public static List<T> LoadList<T>(string directory = "")
    {
        if (DirectoryExists(GetFullPath(directory)) == false) return null;

        List<T> dataList = new List<T>();

        string[] fileNames = Directory.GetFiles(GetFullPath(directory));

        foreach (string fileName in fileNames)
        {
            T data = Load<T>(fileName);

            if (data.GetType() == typeof(T))
                dataList.Add(data);
            else
                Debug.LogWarning($"The type of the data retrieved ({data.GetType().Name}) does not match the target type ({typeof(T).Name})");
        }

        return dataList;
    }

    public static T Load<T>(string fileName, string directory = "")
    {
        if (FileExists(fileName, directory) == false) return default(T);
        //if (File.Exists(GetFullPath(fileName)) == false)
        //{
        //    // File not found
        //    Debug.LogWarning($"The file: '{fileName}' of type '{typeof(T).Name}' does not exist, returning '{default(T)}'");
        //    return default(T);
        //}

        string json = File.ReadAllText(GetFullPath(fileName));
        T data = JsonUtility.FromJson<T>(json);
        return data;
    }

    public static void Delete(string fileName, string directory = "")
    {
        if (FileExists(fileName, directory) == false) return;

        string fullPath = Path.Combine(GetFullPath(directory), fileName);

        //if (Directory.Exists(GetFullPath(directory)) == false)
        //    Debug.LogError($"Couldn't delete data since the directory: '{GetFullPath(directory)}' does not exist!");

        //if (File.Exists(fullPath))
        File.Delete(fullPath);
        //else
        //    Debug.LogError($"Couldn't find {fileName} to delete!");
    }

    public static void Rename(string fileName, string newName, string directory = "")
    {
        if (FileExists(fileName, directory) == false) return;

        string fullPath = Path.Combine(GetFullPath(directory), fileName);
        string newFullPath = Path.Combine(GetFullPath(directory), newName);

        File.Move(fullPath, newFullPath);
    }

    static bool FileExists(string file, string directory = "")
    {
        if (DirectoryExists(directory) == false) return false;

        if (File.Exists(Path.Combine(GetFullPath(directory), file)) == false)
        {
            Debug.LogError($"The file: '{file}' does not exist in directory: {GetFullPath(directory)}!");
            return false;
        }

        return true;
    }

    static bool DirectoryExists(string directory)
    {
        string directoryPath = GetFullPath(directory);

        if (Directory.Exists(directoryPath) == false)
        {
            Debug.LogError($"The directory: '{directoryPath}' does not exist!");
            return false;
        }

        return true;
    }

    public static string GetFullPath(string fileName) => Path.Combine(Application.persistentDataPath, fileName);

    public static string[] GetFileNamesInDir(string directory) => Directory.GetFiles(GetFullPath(directory));

    public static void SaveJson(string json, string fileName, string directory = "")
    {
        if (DirectoryExists(GetFullPath(directory)) == false) return;

        File.WriteAllText(Path.Combine(GetFullPath(directory), fileName), json);
    }

    public static string LoadJson(string fileName)
    {
        if (FileExists(fileName) == false) return "";

        return File.ReadAllText(GetFullPath(fileName));
    }

    public static List<string> LoadJsonList(string directory = "")
    {
        if (DirectoryExists(GetFullPath(directory)) == false) return null;

        List<string> dataList = new List<string>();

        string[] fileNames = GetFileNamesInDir(directory);

        foreach (string fileName in fileNames)
            dataList.Add(LoadJson(fileName));

        return dataList;
    }
}



[System.Serializable]
public struct SerializableKeyValuePair<T, U>
{
    public T key; public U value;
    public SerializableKeyValuePair(T key, U data) { this.key = key; this.value = data; }
}


[System.Serializable]
public struct SerializableDictionary<T, U>
{
    public SerializableKeyValuePair<T, U>[] serializableDict;

    public SerializableDictionary(Dictionary<T, U> dictionary)
    {
        serializableDict = new SerializableKeyValuePair<T, U>[dictionary.Count];

        int index = 0;

        foreach (KeyValuePair<T, U> d in dictionary)
        {
            this.serializableDict[index] = new SerializableKeyValuePair<T, U>(d.Key, d.Value);
            index++;
        }
    }

    public Dictionary<T, U> ToDictionary()
    {
        var d = new Dictionary<T, U>();

        if (serializableDict != null)
            for (int i = 0; i < serializableDict.Length; i++)
                d.Add(serializableDict[i].key, serializableDict[i].value);

        return d;
    }
}