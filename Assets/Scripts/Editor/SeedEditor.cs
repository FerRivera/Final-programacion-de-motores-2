using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using System.Linq;

//[CustomEditor(typeof(Seed))]
public class SeedEditor :  Editor
{
    private Seed _target;
    //private AnimBool _isChild;
    //private int _selectedIndex;
    //private List<Object> _mapItems;// = new List<Object>();
    //private Object _pathsData;

    void OnEnable()
    {
        //Almacenamos el objeto antes para no tener que castearlo cada vez que queramos editar una variable
        _target = (Seed)target;

        //_isChild = new AnimBool(false); // Creamos el anim bool y le pasamos por parámetro su valor inicial
        //_isChild.valueChanged.AddListener(Repaint); // Le agregamos un listener para que cuando cambie su valor haga el fade
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
        _target.mapItems = Resources.LoadAll("MapItems", typeof(GameObject)).ToList();

        _target.selectedIndex = EditorGUILayout.Popup("Path to create", _target.selectedIndex, _target.mapItems.Select(x => x.name).ToArray());

        //_pathsData = (PathConfig)EditorGUILayout.ObjectField("Paths Config", _pathsData, typeof(PathConfig), true);
    }

    private void FixValues()
    {
        
    }
}
