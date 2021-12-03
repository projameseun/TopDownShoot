using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//어떤 클래스를 다루는지 어뜬스크립트를 다루는지 명시해야된다

[CustomEditor(typeof(MapGenerator))] //MapGenertor 우리사용할 클래스꼭 명시 

public class MapEditor : Editor
{

    public override void OnInspectorGUI()
    {

        MapGenerator map = target as MapGenerator;
        // base.OnInspectorGUI(); 맵이커지면성능이 많이떨어진다 

        if (DrawDefaultInspector()) //bool 을반환한다 
        {

            map.GenerateMap();
        }
       if(GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }

    }
}
