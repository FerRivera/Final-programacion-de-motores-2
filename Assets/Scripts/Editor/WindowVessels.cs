using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WindowVessels : EditorWindow
{
    int _selectedIndex;
    List<Object> _objects = new List<Object>();
    int _distance;
    LayerMask _vessels;

    [MenuItem("Level options/Create Vessels")]
    static void CreateWindow()
    {
        var window = ((WindowVessels)GetWindow(typeof(WindowVessels)));
        window.Show();
        window.Init();
    }

    public void Init()
    {
        _objects = Resources.LoadAll("Vessels", typeof(GameObject)).ToList();        
    }

    void OnGUI()
    {
        _selectedIndex = EditorGUILayout.Popup("Vessel to create", _selectedIndex, _objects.Select(x => x.name).ToArray());

        _distance = EditorGUILayout.IntField("Min distance between vessels", _distance);

        _vessels = EditorGUILayout.LayerField("Vessel layer",_vessels.value);

        var _preview = AssetPreview.GetAssetPreview(_objects[_selectedIndex]);

        if (_preview != null)
        {
            GUILayout.BeginHorizontal();
            GUI.DrawTexture(GUILayoutUtility.GetRect(150, 150, 150, 150), _preview, ScaleMode.ScaleToFit);
            GUILayout.Label(_objects[_selectedIndex].name);
            GUILayout.Label(AssetDatabase.GetAssetPath(_objects[_selectedIndex]));
            GUILayout.EndHorizontal();
        }        
    }

    void CreateVessel()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0))
            return;

        RaycastHit MousePosHit;
        Ray MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(MouseRay, out MousePosHit, float.MaxValue))
        {            
            var dir = MousePosHit.point + (Camera.main.transform.position - MousePosHit.point).normalized;

            if (!CloserVessels(dir, _distance))
            {
                GameObject path = (GameObject)Instantiate(_objects[_selectedIndex]);

                if (path.GetComponent<Vessels>() == null)
                    path.AddComponent<Vessels>();

                Vector3 pos = new Vector3(dir.x, path.GetComponent<Renderer>().bounds.size.y, dir.z);
                path.transform.position = pos;

                Debug.Log(pos);
            }            
        }
    }

    bool CloserVessels(Vector3 position,float radius)
    {
        var temp = Physics.OverlapSphere(position, radius, _vessels.value);

        if (temp.Count() > 0)
            return true;

        return false;
    }
    //void CreateVessel()
    //{
    //    if (Event.current.type == EventType.MouseDown)
    //    {
    //        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

    //        RaycastHit hit;
    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            Vector3 newTilePosition = hit.point;
    //            //Vector3 pos = new Vector3(dir.x, 0, dir.z);
    //            Debug.Log(newTilePosition);
    //        }


    //        //Ray ray = Camera.main.ScreenPointToRay(Event.current.mousePosition);
    //        //RaycastHit hit = new RaycastHit();
    //        //if (Physics.Raycast(ray, out hit, float.MaxValue))
    //        //{
    //        //    Debug.Log(Event.current.mousePosition);
    //        //    Vector3 newTilePosition = hit.point;
    //        //    //Vector3 pos = new Vector3(dir.x, 0, dir.z);
    //        //    Debug.Log(newTilePosition);
    //        //    //Instantiate(newTile, newTilePosition, Quaternion.identity);
    //        //}
    //    }
    //}    

    private void Update()
    {
        CreateVessel();
    }
}

public enum Layers
{
    Vessels = 8
}