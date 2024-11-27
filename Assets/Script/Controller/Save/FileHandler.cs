#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#endregion

public static class FileHandler
{
    public static void SaveToJSON<T>(List<T> toSave, string filename)
    {
        Debug.Log(GetPath(filename));
        var content = JsonHelper.ToJson(toSave.ToArray());
        WriteFile(GetPath(filename), content);
    }

    public static void SaveToJSON<T>(T toSave, string filename)
    {
        Debug.Log(GetPath(filename));
        var content = JsonUtility.ToJson(toSave);
        WriteFile(GetPath(filename), content);
    }

    public static List<T> ReadListFromJSON<T>(string filename)
    {
        var content = ReadFile(GetPath(filename));

        if (string.IsNullOrEmpty(content) || content == "{}") return new List<T>();

        List<T> res = JsonHelper.FromJson<T>(content).ToList();

        return res;

    }

    public static T ReadFromJSON<T>(string filename)
    {
        var content = ReadFile(GetPath(filename));

        if (string.IsNullOrEmpty(content) || content == "{}") return default;

        var res = JsonUtility.FromJson<T>(content);

        return res;

    }

    static string GetPath(string filename)
    {
        return Application.persistentDataPath + "/" + filename;
    }

    static void WriteFile(string path, string content)
    {
        var fileStream = new FileStream(path, FileMode.Create);

        using (var writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }

    static string ReadFile(string path)
    {
        if (File.Exists(path))
            using (var reader = new StreamReader(path))
            {
                var content = reader.ReadToEnd();
                return content;
            }

        return "";
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    #region Nested type: Wrapper

    [Serializable]
    class Wrapper<T>
    {
        public T[] Items;
    }

    #endregion
}