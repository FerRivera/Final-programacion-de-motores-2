using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WindowLoadMaps : EditorWindow
{
    List<bool> wantToDeleteList = new List<bool>();
    private Vector2 _scrollPosition;
    public float maxYSize = 500;
    public float maxXSize = 500;

    [MenuItem("Level options/Load maps")]
    static void CreateWindow()
    {
        var window = ((WindowLoadMaps)GetWindow(typeof(WindowLoadMaps)));
        window.Show();
        window.Init();
    }

    public void Init()
    {
        maxSize = new Vector2(maxXSize, maxYSize);

        var asset = AssetDatabase.FindAssets("t:MapsSaved", null);

        for (int i = 0; i < asset.Length; i++)
        {
            bool wantToDelete = false;
            wantToDeleteList.Add(wantToDelete);
        }
    }

    public void LoadMaps()
    {
        List<string> tempPath = new List<string>();

        var asset = AssetDatabase.FindAssets("t:MapsSaved", null);

        EditorGUILayout.BeginVertical(GUILayout.Height(maxYSize));
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, true, true);

        MapsSaved currentMap = null;

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

            if (!wantToDeleteList[i])
            {
                if (!wantToDeleteList[i] && GUILayout.Button("Delete map"))
                {
                    wantToDeleteList[i] = true;
                }
            }
            else
            {
                if (GUILayout.Button("No") && wantToDeleteList[i])
                {
                    wantToDeleteList[i] = false;
                }
                if (GUILayout.Button("Yes") && wantToDeleteList[i])
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }

            if(GUILayout.Button("Load map"))
            {
                currentMap = AssetDatabase.LoadAssetAtPath<MapsSaved>(path);
                LoadMapOnScene(currentMap);
            }

            EditorGUILayout.Space();
        }
        
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    public void LoadMapOnScene(MapsSaved map)
    {
        foreach (var item in map.paths)
        {
            GameObject path = (GameObject)Instantiate(item);
            path.transform.position = item.transform.position;
        }
        
        
    }

    void OnGUI()
    {
        LoadMaps();
    }

}
