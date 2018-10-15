using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathConfig : ScriptableObject 
{
    public List<GameObject> paths = new List<GameObject>();
    public List<Object> objectsToInstantiate = new List<Object>();
}
