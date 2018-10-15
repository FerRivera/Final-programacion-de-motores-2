using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    private Path _target;

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
        //pathsSaved = (PathConfig)Resources.Load("PathConfig");

        //ConfigurateObjects();

        //_target.selectedIndex = EditorGUILayout.Popup("Path to create", _target.selectedIndex, _target.mapItems.Select(x => x.name).ToArray());

        //ShowPreview();
    }

    private void FixValues()
    {

    }
}
