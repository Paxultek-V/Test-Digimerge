using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SerializedResourcedScriptableObject<T> : ScriptableObject where T : SerializedResourcedScriptableObject<T>
{
    public const string Path = "ResourcedScriptalbeObjects/";
    public static T Instance()
    {
        if (_instance != null) return _instance;
        #if UNITY_EDITOR
        string fullPath = $"{Path}{typeof(T).Name}";
        _instance = Resources.Load<T>(fullPath);
        if (_instance == null)
        {
            _instance = CreateInstance<T>();
            Directory.CreateDirectory($"{Application.dataPath}/Resources/{Path}");
            UnityEditor.AssetDatabase.CreateAsset(_instance, $"Assets/Resources/{fullPath}.asset");
            UnityEditor.AssetDatabase.SaveAssets();
        }
        #endif

        _instance = Resources.Load<T>($"{Path}{typeof(T).Name}");
        return _instance;
    }

    private static T _instance;
}
