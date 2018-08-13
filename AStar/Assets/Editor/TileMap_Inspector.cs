using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditor(typeof(TileMap))]
public class TileMap_Inspector : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Regenerate"))
        {
            TileMap tileMap = (TileMap)target;
            tileMap.CreateMap();
        }
    }
}
