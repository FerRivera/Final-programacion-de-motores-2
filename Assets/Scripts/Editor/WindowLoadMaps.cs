using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WindowLoadMaps : EditorWindow
{
    [MenuItem("Level options/Load maps")]
    static void CreateWindow()
    {
        var window = ((WindowLoadMaps)GetWindow(typeof(WindowLoadMaps)));
        window.Show();
        window.Init();
    }

    public void Init()
    {
        
    }

    public void CargarMapas()
    {
        List<string> tempPath = new List<string>();

        var asset = AssetDatabase.FindAssets("t:MapsSaved", null);

        for (int i = asset.Length - 1; i >= 0; i--)
        {
            //obtengo todo el path
            string path = AssetDatabase.GUIDToAssetPath(asset[i]);
            //separo las diferentes carpetas por el carcater /
            tempPath = path.Split('/').ToList();
            //obtengo la ultima parte, que seria el nombre con la extension y saco la extension
            var currentMapName = tempPath.LastOrDefault().Split('.');
            //si el nombre que obtuve con el que escribi son iguales entonces uso ese scriptable object
            
            EditorGUI.BeginDisabledGroup(true);
            currentMapName[0] = EditorGUILayout.TextField("Map name", currentMapName[0]);            
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("Delete map"))
            {
                AssetDatabase.DeleteAsset(path);
            }
        }
    }

    void OnGUI()
    {
        CargarMapas();

    }

}
