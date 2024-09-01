using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GenericSaveData
{
    public const string FILE_NAME_FORMAT = "{0}.json";

    string fileName;

    public LevelSaveData(string fileName)
    {
        this.fileName = fileName;
    }

    public void SaveToFile() => SaveHelper.Save<GenericSaveData>(GetFileName(fileName), this);

    public static GenericSaveData Load(string fileName) => SaveHelper.Load<GenericSaveData>(GetFileName(fileName));

    public static string GetFileName(string fileName) => string.Format(FILE_NAME_FORMAT, fileName);
}
