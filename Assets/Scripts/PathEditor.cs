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

    static bool canSwitch;

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
        if(_target.lastIndex != _target.currentIndex && canSwitch)
        {
            _target.lastIndex = _target.currentIndex;
            
            GameObject path = (GameObject)Instantiate(pathsSaved.objectsToInstantiate[_target.currentIndex]);
            path.transform.position = pathsSaved.paths[_target.id].transform.position;            
            path.AddComponent<Path>().currentIndex = _target.lastIndex;
            DestroyImmediate(pathsSaved.paths[_target.id]);
            pathsSaved.paths.Insert(_target.id, path);
            Selection.activeObject = path;
        }
    }

    public static void ExitGUI()
    {
        GUIUtility.ExitGUI();
        canSwitch = true;
        throw new ExitGUIException();
    }

    private void FixValues()
    {

    }
}
