﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Linq;
using System;

//si no le pongo custom editor aca, me desaparecen los botones + para crear los path
[CustomEditor(typeof(Seed))]
public class SeedEditor : Editor
{
    private Seed _target;

    public float buttonWidth = 130;
    public float buttonHeight = 30;
    public float buttonPlusWidth = 30;

    int _buttonMinSize = 45;
    int _buttonMaxSize = 70;

    public List<GameObject> buttonsPosition = new List<GameObject>();

    public PathConfig pathsSaved;

    public bool restartMap = false;

    void OnEnable()
    {
        _target = (Seed)target;
    }

    public override void OnInspectorGUI()
    {
        //Primero mostramos los valores
        ShowValues();

        //Luego arreglamos los valores que tengamos que arreglar
        FixValues();

        //DrawDefaultInspector(); //Dibuja el inspector como lo hariamos normalmente. Sirve por si no queremos rehacher todo el inspector y solamente queremos agregar un par de funcionalidades.

        Repaint(); //Redibuja el inspector
    }

    private void ShowValues()
    {
        pathsSaved = (PathConfig)Resources.Load("PathConfig");

        ConfigurateObjects();

        _target.selectedIndex = EditorGUILayout.Popup("Path to create", _target.selectedIndex, pathsSaved.objectsToInstantiate.Select(x => x.name).ToArray());

        ShowPreview();
    }

    private void FixValues()
    {

    }   

    void OnSceneGUI()
    {
        Handles.BeginGUI();

        var addValue = 30 / Vector3.Distance(Camera.current.transform.position, _target.transform.position);

        var screenPos = Camera.current.WorldToScreenPoint(_target.transform.position);

        RestartMap();

        DeleteLastPath();

        DrawButton("+", _target.transform.position + Camera.current.transform.up * addValue, ButtonType.Add);
        DrawButton("+", _target.transform.position - Camera.current.transform.up * addValue, ButtonType.Add);
        DrawButton("+", _target.transform.position + Camera.current.transform.right * addValue, ButtonType.Add);
        DrawButton("+", _target.transform.position - Camera.current.transform.right * addValue, ButtonType.Add);

        //if (GUI.Button(new Rect(screenPos.x - buttonPlusWidth / 2 + 100, Camera.current.pixelHeight - screenPos.y, buttonPlusWidth, buttonHeight), "+"))
        //{
        //    if (!Physics.Raycast(_target.transform.position, _target.transform.forward, 1))
        //        CreatePath(/*_target.transform.position + _target.transform.forward * addValue,*/ _target.transform.forward, Direction.Forward);
        //}

        //if (GUI.Button(new Rect(screenPos.x - buttonPlusWidth / 2 - 100, Camera.current.pixelHeight - screenPos.y, buttonPlusWidth, buttonHeight), "+"))
        //{
        //    if (!Physics.Raycast(_target.transform.position, -_target.transform.forward, 1))
        //        CreatePath(/*_target.transform.position - _target.transform.forward * addValue,*/ -_target.transform.forward, Direction.Backward);
        //}

        //if (GUI.Button(new Rect(screenPos.x - buttonPlusWidth / 2, Camera.current.pixelHeight - screenPos.y + 100, buttonPlusWidth, buttonHeight), "+"))
        //{
        //    if (!Physics.Raycast(_target.transform.position, _target.transform.right, 1))
        //        CreatePath(/*_target.transform.position + _target.transform.right * addValue,*/ _target.transform.right, Direction.Right);
        //}

        //if (GUI.Button(new Rect(screenPos.x - buttonPlusWidth / 2, Camera.current.pixelHeight - screenPos.y - 100, buttonPlusWidth, buttonHeight), "+"))
        //{
        //    if (!Physics.Raycast(_target.transform.position, -_target.transform.right, 1))
        //        CreatePath(/*_target.transform.position - _target.transform.right * addValue,*/ -_target.transform.right, Direction.Left);
        //}
        Handles.EndGUI();
    }

    public void ConfigurateObjects()
    {
        _target.mapItems = Resources.LoadAll("MapItems", typeof(GameObject)).ToList();

        if (pathsSaved.objectsToInstantiate.Count != _target.mapItems.Count)
        {
            pathsSaved.objectsToInstantiate = _target.mapItems;
        }
    }
    void ShowPreview()
    {
        var _preview = AssetPreview.GetAssetPreview(pathsSaved.objectsToInstantiate[_target.selectedIndex]);
        if (_preview != null)
        {
            Repaint();
            GUILayout.BeginHorizontal();
            GUI.DrawTexture(GUILayoutUtility.GetRect(150, 150, 150, 150), _preview, ScaleMode.ScaleToFit);
            GUILayout.Label(pathsSaved.objectsToInstantiate[_target.selectedIndex].name);
            GUILayout.Label(AssetDatabase.GetAssetPath(pathsSaved.objectsToInstantiate[_target.selectedIndex]));
            GUILayout.EndHorizontal();
        }
    }

    void RestartMap()
    {

        if (!restartMap && GUI.Button(new Rect(20, 20, buttonWidth, buttonHeight), "Restart Map"))
        {
            restartMap = true;
        }

        if(restartMap && GUI.Button(new Rect(20, 20, buttonWidth, buttonHeight), "No"))
        {
            restartMap = false;
        }
        if (restartMap && GUI.Button(new Rect(160, 20, buttonWidth, buttonHeight), "Yes"))
        {
            foreach (var item in pathsSaved.paths)
            {
                DestroyImmediate(item);
            }

            pathsSaved.paths.Clear();
            pathsSaved.objectType.Clear();
            pathsSaved.positions.Clear();

            _target.transform.position = new Vector3(0, 0, 0);

            restartMap = false;
        }
    }

    void DeleteLastPath()
    {
        if (GUI.Button(new Rect(20, 60, buttonWidth, buttonHeight), "Delete Last Path"))
        {
            if(pathsSaved.paths.Count > 0)
            {
                var lastObject = pathsSaved.paths.LastOrDefault();

                pathsSaved.objectType.RemoveAt(pathsSaved.paths.Count-1);
                pathsSaved.positions.RemoveAt(pathsSaved.paths.Count-1);
                pathsSaved.paths.Remove(lastObject);

                if (pathsSaved.paths.LastOrDefault() != null)
                    _target.transform.position = pathsSaved.paths.LastOrDefault().transform.position;

                DestroyImmediate(lastObject);
            }
            else
            {
                _target.transform.position = new Vector3(0, 0, 0);
            }
        }
    }

    //de esta forma los botones se escalan bien pero rotan junto con la camara
    public Vector3 CheckDistance(Vector3 pos)
    {
        Vector3 temp = new Vector3(Mathf.Infinity,Mathf.Infinity);
        float floatTemp = Mathf.Infinity; 

        foreach (var item in buttonsPosition)
        {
            if (Vector3.Distance(item.transform.position, temp) < floatTemp)
            {
                Handles.DrawLine(item.transform.position, temp);
                floatTemp = Vector3.Distance(item.transform.position, temp);
                temp = item.transform.position;
            }
        }
        return temp;        
    }

    // como limitar el tamaño de los botones? tengo que hacer scripts aparte e instanciar esos scripts como botones y limitar su max size desde ahi?
    private void DrawButton(string text, Vector3 position, ButtonType typ)
    {
        var p = Camera.current.WorldToScreenPoint(position);

        var size = 700 / Vector3.Distance(Camera.current.transform.position, position);
        size = Mathf.Clamp(size, _buttonMinSize, _buttonMaxSize);

        var r = new Rect(p.x - size / 2, Screen.height - p.y - size, size, size / 2);

        var dirTest = new Vector3(position.x, 0, position.z) - new Vector3(_target.transform.position.x, 0, _target.transform.position.z);

        dirTest = dirTest.normalized;

        var posArray = new Vector3[] { _target.transform.forward, -_target.transform.forward, _target.transform.right, -_target.transform.right };

        var disArray = posArray.OrderBy(x => Vector3.Distance(x, dirTest));

        dirTest = disArray.First();

        Direction dir;

        if (GUI.Button(r, text))
        {
            if (dirTest.x >= 1)
                dir = Direction.Right;
            else if (dirTest.x <= -1)
                dir = Direction.Left;
            else if (dirTest.z >= 1)
                dir = Direction.Forward;
            else
                dir = Direction.Backward;

            CreatePath(_target.transform.position + dirTest, dir);
        }

        //var p = Camera.current.WorldToScreenPoint(position);
        //var size = 700 / Vector3.Distance(Camera.current.transform.position, position);
        //var r = new Rect(p.x - size/2, Screen.height - p.y - size, size, size/2);

        //if (GUI.Button(r, text))
        //{
        //    switch(typ)
        //    {
        //        case ButtonType.Add:
        //            Tile t = (Tile)Instantiate(Resources.Load("TilePrefab", typeof(Tile)));
        //            tilesManager.nodes.Add(t);
        //            t.transform.position = _target.transform.position + dir;
        //            Selection.activeObject = t;
        //            break;
        //        case ButtonType.ChangeType:
        //            switch(_target.currentType)
        //            {
        //                case Tile.TileTypes.Grass:
        //                    _target.currentType = Tile.TileTypes.Ice;
        //                    _target.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Ice", typeof(Material));
        //                    break;
        //                case Tile.TileTypes.Ice:
        //                    _target.currentType = Tile.TileTypes.Lava;
        //                    _target.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Lava", typeof(Material));
        //                    break;
        //                case Tile.TileTypes.Lava:
        //                    _target.currentType = Tile.TileTypes.Water;
        //                    _target.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Water", typeof(Material));
        //                    break;
        //                case Tile.TileTypes.Water:
        //                    _target.currentType = Tile.TileTypes.Grass;
        //                    _target.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Materials/Grass", typeof(Material));
        //                    break;
        //            }
        //            break;
        //    }

        //    foreach (var item in tilesManager.nodes)
        //    {
        //        item.UpdateTiles();
        //    }
        //}    
    }

    void CreatePath(Vector3 dir, Direction direction)
    {
        GameObject lastObject = null;
        GameObject path = (GameObject)Instantiate(pathsSaved.objectsToInstantiate[_target.selectedIndex]);
        path.transform.position = new Vector3(0, 0, 0);

        if (path.GetComponent<Path>() == null)
        {
            path.AddComponent<Path>().lastIndex = _target.selectedIndex;
            var temp = path.GetComponent<Path>();
            temp.currentIndex = _target.selectedIndex;
            temp.id = pathsSaved.paths.Count;
        }            

        if (pathsSaved.paths.Count > 0)
            lastObject = pathsSaved.paths[pathsSaved.paths.Count - 1];
        else
            lastObject = path;        

        _target.transform.position = GetNextMove(lastObject, direction);

        path.transform.position = GetPathPosition(lastObject, direction);

        _target.transform.position = path.transform.position;

        pathsSaved.paths.Add(path);
        pathsSaved.objectType.Add(_target.selectedIndex);
        pathsSaved.positions.Add(path.transform.position);
    }

    Vector3 GetNextMove(GameObject go, Direction direction)
    {
        Vector3 DistanceToReturn = new Vector3(0, 0, 0);
        switch (direction)
        {
            case Direction.Forward:
                DistanceToReturn = new Vector3(go.transform.position.x, 0, go.transform.position.z + go.GetComponent<Renderer>().bounds.size.z / 2);
                return DistanceToReturn;
            case Direction.Backward:
                DistanceToReturn = new Vector3(go.transform.position.x, 0, go.transform.position.z - go.GetComponent<Renderer>().bounds.size.z / 2);
                return DistanceToReturn;
            case Direction.Left:
                DistanceToReturn = new Vector3(go.transform.position.x - go.GetComponent<Renderer>().bounds.size.x / 2, 0, go.transform.position.z);
                return DistanceToReturn;
            case Direction.Right:
                DistanceToReturn = new Vector3(go.transform.position.x + go.GetComponent<Renderer>().bounds.size.x / 2, 0, go.transform.position.z);
                return DistanceToReturn;
        }

        return default(Vector3);
    }

    Vector3 GetPathPosition(GameObject go, Direction direction)
    {
        Vector3 DistanceToReturn = new Vector3(0, 0, 0);
        switch (direction)
        {
            case Direction.Forward:
                    DistanceToReturn = new Vector3(_target.transform.position.x, 0, _target.transform.position.z + go.GetComponent<Renderer>().bounds.size.z / 2);
                    return DistanceToReturn;
            case Direction.Backward:
                    DistanceToReturn = new Vector3(_target.transform.position.x, 0, _target.transform.position.z - go.GetComponent<Renderer>().bounds.size.z / 2);
                    return DistanceToReturn;
            case Direction.Left:
                    DistanceToReturn = new Vector3(_target.transform.position.x - go.GetComponent<Renderer>().bounds.size.x / 2, 0, _target.transform.position.z);
                    return DistanceToReturn;
            case Direction.Right:
                    DistanceToReturn = new Vector3(_target.transform.position.x + go.GetComponent<Renderer>().bounds.size.x / 2, 0, _target.transform.position.z);
                    return DistanceToReturn;
        }

        return default(Vector3);
    }

    public enum Direction
    {
        Forward,
        Backward,
        Left,
        Right
    }

    public enum ButtonType
    {
        Add,
        ChangeType
    }

}
