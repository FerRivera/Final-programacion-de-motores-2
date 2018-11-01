using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    private Path _target;

    public PathConfig pathsSaved;

    private Seed _seed;

    void OnEnable()
    {
        _target = (Path)target;

        _seed = GameObject.FindGameObjectWithTag("Seed").GetComponent<Seed>();
    }

    public override void OnInspectorGUI()
    {
        ShowValues();

        FixValues();

        Repaint();
    }

    void OnSceneGUI()
    {
        Handles.BeginGUI();

        SetAsActualPath();

        Handles.EndGUI();
    }

    private void ShowValues()
    {
        pathsSaved = (PathConfig)Resources.Load("PathConfig");

        _target.currentIndex = EditorGUILayout.Popup("Path to create", _target.currentIndex, pathsSaved.objectsToInstantiate.Select(x => x.name).ToArray());

        SwitchType();
        
        //ConfigurateObjects();

        //_target.selectedIndex = EditorGUILayout.Popup("Path to create", _target.selectedIndex, _target.mapItems.Select(x => x.name).ToArray());

        //ShowPreview();
    }

    void SwitchType()
    {
        if(_target.lastIndex != _target.currentIndex)
        {
            //_target.lastIndex = _target.currentIndex;
            
            GameObject path = (GameObject)Instantiate(pathsSaved.objectsToInstantiate[_target.currentIndex]);
            path.transform.position = pathsSaved.paths[_target.id].transform.position;

            path.AddComponent<Path>().currentIndex = _target.currentIndex;
            path.GetComponent<Path>().lastIndex = _target.currentIndex;
            path.GetComponent<Path>().id = _target.id;

            DestroyImmediate(pathsSaved.paths[_target.id]);

            pathsSaved.paths.Remove(pathsSaved.paths[_target.id]);
            pathsSaved.objectType.Remove(pathsSaved.objectType[_target.id]);
            pathsSaved.positions.Remove(pathsSaved.positions[_target.id]);

            pathsSaved.paths.Insert(_target.id, path);
            pathsSaved.objectType.Insert(_target.id, path.GetComponent<Path>().currentIndex);
            pathsSaved.positions.Insert(_target.id, path.transform.position);
            //pathsSaved.paths.Add(path);
            Selection.activeObject = path;
        }
    }

    void SetAsActualPath()
    {
        if (GUI.Button(new Rect(20, 30, 130, 30), "Bring seed"))
        {
            _seed.transform.position = _target.transform.position;
            Swap(pathsSaved.paths, _target.id, pathsSaved.paths.Count-1);
            Swap(pathsSaved.positions, _target.id, pathsSaved.positions.Count-1);
            Swap(pathsSaved.objectType, _target.id, pathsSaved.objectType.Count-1);

            int tempID = _target.id;

            _target.id = pathsSaved.paths.LastOrDefault().GetComponent<Path>().id;
            pathsSaved.paths.LastOrDefault().GetComponent<Path>().id = tempID;
        }
    }

    public void Swap<T>(IList<T> list, int itemToMove, int placeLast)
    {
        T tmp = list[itemToMove];
        list[itemToMove] = list[placeLast];
        list[placeLast] = tmp;
    }

    private void FixValues()
    {

    }
}
