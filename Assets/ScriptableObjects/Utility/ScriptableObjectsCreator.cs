using UnityEngine;
using UnityEditor;

public class ScriptableObjectsCreator
{
    [MenuItem("Utilities/Create/PathConfig")]
    public static void CreatePathConfig()
    {
        ScriptableObjectUtility.CreateAsset<PathConfig>();
    }
}