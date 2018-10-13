using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Linq;

//si no le pongo custom editor aca, me desaparecen los botones + para crear los path
[CustomEditor(typeof(Seed))]
public class SeedEditor : Editor
{
    private Seed _target;
    //public List<GameObject> tilesCreated = new List<GameObject>();

    public float buttonWidth = 130;
    public float buttonPlusWidth = 30;
    public float buttonHeight = 30;

    //private int _selectedIndex;

    private Vector3 _distanceBetweenPaths;

    //private List<Object> _mapItems;// = new List<Object>();
    public List<GameObject> buttonsPosition = new List<GameObject>();
    public List<GameObject> temp = new List<GameObject>();

    public GameObject forwardButton;
    public GameObject backwardButton;

    public AnimBool _isChild;

    public PathConfig pathsSaved;

    float buttonYPos = 100;

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

        //OnPreviewGUI(GUILayoutUtility.GetRect(100, 100), EditorStyles.whiteLabel);
    }

    private void ShowValues()
    {
        pathsSaved = (PathConfig)Resources.Load("PathConfig");

        ConfigurateObjects();

        _target.selectedIndex = EditorGUILayout.Popup("Path to create", _target.selectedIndex, _target.mapItems.Select(x => x.name).ToArray());

        ShowPreview();
    }

    private void FixValues()
    {

    }   

    void OnSceneGUI()
    {
        Handles.BeginGUI();

        var screenPos = Camera.current.WorldToScreenPoint(_target.transform.position);

        RestartMap();

        DeleteLastPath();

        if (GUI.Button(new Rect(screenPos.x - buttonPlusWidth / 2 + 100, Camera.current.pixelHeight - screenPos.y, buttonPlusWidth, buttonHeight), "+"))
        {
            if (!Physics.Raycast(_target.transform.position, _target.transform.forward, 1))
                ButtonSwitch(/*_target.transform.position + _target.transform.forward * addValue,*/ _target.transform.forward, Direction.Forward);
        }

        if (GUI.Button(new Rect(screenPos.x - buttonPlusWidth / 2 - 100, Camera.current.pixelHeight - screenPos.y, buttonPlusWidth, buttonHeight), "+"))
        {
            if (!Physics.Raycast(_target.transform.position, -_target.transform.forward, 1))
                ButtonSwitch(/*_target.transform.position - _target.transform.forward * addValue,*/ -_target.transform.forward, Direction.Backward);
        }

        if (GUI.Button(new Rect(screenPos.x - buttonPlusWidth / 2, Camera.current.pixelHeight - screenPos.y + 100, buttonPlusWidth, buttonHeight), "+"))
        {
            if (!Physics.Raycast(_target.transform.position, _target.transform.right, 1))
                ButtonSwitch(/*_target.transform.position + _target.transform.right * addValue,*/ _target.transform.right, Direction.Right);
        }

        if (GUI.Button(new Rect(screenPos.x - buttonPlusWidth / 2, Camera.current.pixelHeight - screenPos.y - 100, buttonPlusWidth, buttonHeight), "+"))
        {
            if (!Physics.Raycast(_target.transform.position, -_target.transform.right, 1))
                ButtonSwitch(/*_target.transform.position - _target.transform.right * addValue,*/ -_target.transform.right, Direction.Left);
        }
        Handles.EndGUI();
    }

    public void ConfigurateObjects()
    {
        _target.mapItems = Resources.LoadAll("MapItems", typeof(GameObject)).ToList();        

        if(pathsSaved.objectsLoaded != _target.mapItems.Count)
        {
            pathsSaved.objectsLoaded = _target.mapItems.Count;

            foreach (var item in pathsSaved.paths)
            {
                if (item != null && item.GetComponent<Path>() == null)
                    item.AddComponent<Path>();
            }
        }        
    }
    public void ShowPreview()
    {
        var _preview = AssetPreview.GetAssetPreview(_target.mapItems[_target.selectedIndex]);
        if (_preview != null)
        {
            Repaint();
            GUILayout.BeginHorizontal();
            GUI.DrawTexture(GUILayoutUtility.GetRect(150, 150, 150, 150), _preview, ScaleMode.ScaleToFit);
            GUILayout.Label(_target.mapItems[_target.selectedIndex].name);
            GUILayout.Label(AssetDatabase.GetAssetPath(_target.mapItems[_target.selectedIndex]));
            GUILayout.EndHorizontal();
        }
    }     

    public void RestartMap()
    {
        if (GUI.Button(new Rect(20, 20, buttonWidth, buttonHeight), "Restart Map"))
        {
            foreach (var item in pathsSaved.paths)
            {
                DestroyImmediate(item);
            }

            pathsSaved.paths.Clear();

            _target.transform.position = new Vector3(0,0,0);
        }
    }

    public void DeleteLastPath()
    {
        if (GUI.Button(new Rect(20, 60, buttonWidth, buttonHeight), "Delete Last Path"))
        {
            var lastObject = pathsSaved.paths[pathsSaved.paths.Count - 1];

            _target.transform.position = lastObject.transform.position;

            pathsSaved.paths.Remove(lastObject);

            DestroyImmediate(lastObject);            
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
    private void DrawButton(string text, Vector3 position, Vector3 dir)
    {
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

    public void ButtonSwitch(Vector3 dir, Direction direction)
    {
        //Vector3 distance = new Vector3(0, 0, 0);
        GameObject lastObject = null;
        GameObject path = (GameObject)Instantiate(_target.mapItems[_target.selectedIndex]);
        path.transform.position = new Vector3(0, 0, 0);
        if (pathsSaved.paths.Count > 0)
            lastObject = pathsSaved.paths[pathsSaved.paths.Count - 1];
        else
            lastObject = path;

        pathsSaved.paths.Add(path);

        _target.transform.position = GetNextMove(lastObject, direction);

        path.transform.position = GetPathPosition(lastObject, direction);//new Vector3(0,0, _target.transform.position.z + path.GetComponent<Renderer>().bounds.size.z / 2);

        _target.transform.position = path.transform.position;
    }

    public Vector3 GetNextMove(GameObject go, Direction direction)
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

    Vector3 GetDistancePosition2(GameObject go, Direction direction)
    {
        Vector3 DistanceToReturn = new Vector3(0, 0, 0);
        switch (direction)
        {
            case Direction.Forward:
                DistanceToReturn = new Vector3(0, 0, go.GetComponent<Renderer>().bounds.size.z / 2);
                return DistanceToReturn;
            case Direction.Backward:
                DistanceToReturn = new Vector3(0, 0, go.GetComponent<Renderer>().bounds.size.z / 2);
                return DistanceToReturn;
            case Direction.Left:
                DistanceToReturn = new Vector3(go.GetComponent<Renderer>().bounds.size.x / 2, 0, 0);
                return DistanceToReturn;
            case Direction.Right:
                DistanceToReturn = new Vector3(go.GetComponent<Renderer>().bounds.size.x / 2, 0, 0);
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
}
