using UnityEngine;
using UnityEditor;

public class ScriptableObjectsCreator
{
    [MenuItem("Level options/Create/Map configurations")]
    public static void CreatePathConfig()
    {
        ScriptableObjectUtility.CreateAsset<PathConfig>("Resources/PathConfig");
    }
}