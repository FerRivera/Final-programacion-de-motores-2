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

    void OnEnable()
    {
        _target = (Path)target;
    }

    public override void OnInspectorGUI()
    {
        ShowValues();

        FixValues();

        Repaint();
    }

    private void ShowValues()
    {
        pathsSaved = (PathConfig)Resources.Load("PathConfig");

        _target.currentIndex = EditorGUILayout.Popup("Path to create", _target.currentIndex, pathsSaved.objectsToInstantiate.Select(x => x.name).ToArray());

        SwitchType();
        //pathsSaved = (PathConfig)Resources.Load("PathConfig");

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

            DestroyImmediate(pathsSaved.paths[_target.id]);            

            pathsSaved.paths.Remove(pathsSaved.paths[_target.id]);            

            path.GetComponent<Path>().id = pathsSaved.paths.Count;

            //pathsSaved.paths.Insert(_target.id, path);

            pathsSaved.paths.Add(path);
            Selection.activeObject = path;
        }
    }

    private void FixValues()
    {

    }
}
